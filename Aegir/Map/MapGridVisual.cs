﻿using Aegir.Util;
using AegirLib.Scene;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Aegir.Map
{
    public class MapGridVisual : MeshVisual3D
    {
        private const int GridSize = 10;
        private const int ZoomSteps = 200;
        private const double zoomInverseFactor = 1d / ZoomSteps;
        private double snapInverseFactor;
        private double lastZoom = 0;
        private int currentTileX;
        private int currentTileY;
        private int upperZoomThreshold;
        private int lowerZoomThreshold;

        public List<MapTileVisual> Tiles { get; set; }

        private int tileSize;

        private GridMode gridMode;

        public int TileSize
        {
            get { return tileSize; }
            set
            {
                if (value == 0)
                {
                    throw new Exception("Tilesize Cannot Be Zero");
                }

                tileSize = value;
                snapInverseFactor = 1d / value;
            }
        }

        private int mapZoom;

        public int MapZoomLevel
        {
            get { return mapZoom; }
            set
            {
                if (mapZoom != value)
                {
                    Aegir.Util.DebugUtil.LogWithLocation($"Zoom Changed, new value {value}");
                    ZoomGrid(value);
                }
            }
        }

        public int ViewZoomLevel { get; set; }

        public Point3D MapCenter { get; set; }

        public CameraController MapCamera
        {
            get { return (CameraController)GetValue(MapCameraProperty); }
            set { SetValue(MapCameraProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapCamera.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapCameraProperty =
            DependencyProperty.Register(nameof(MapCamera), typeof(CameraController), typeof(MapGridVisual));

        public MapTileGenerator TileGenerator
        {
            get { return (MapTileGenerator)GetValue(TileGeneratorProperty); }
            set { SetValue(TileGeneratorProperty, value); }
        }

        private bool translateOnZoom;

        public bool TranslateOnZoom
        {
            get { return translateOnZoom; }
            set
            {
                translateOnZoom = value;
                Aegir.Util.DebugUtil.LogWithLocation($"TranslateONZoomChanged to {value}");
            }
        }

        // Using a DependencyProperty as the backing store for TileGenerator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileGeneratorProperty =
            DependencyProperty.Register(nameof(TileGenerator), typeof(MapTileGenerator), typeof(MapGridVisual), new PropertyMetadata(new MapTileGenerator()));

        public MapGridVisual()
        {
            //Compiler will complain about ifstatement unreachable code due to gridsize constant
            //But we want it handle changes by dev to the constant
#pragma warning disable CS0162
            if (GridSize % 2 == 0)
            {
                gridMode = GridMode.Even;
            }
            else
            {
                gridMode = GridMode.Odd;
            }
#pragma warning restore CS0162

            Tiles = new List<MapTileVisual>();
            TileSize = 32;
            mapZoom = 18;
            translateOnZoom = true;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            //Application.Current.MainWindow.Loaded += MainWindow_Loaded;
            InitGrid();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitGrid();
        }

        public void InitGrid()
        {
            int midNum = (int)Math.Ceiling(GridSize / 2d);
            MapCenter = new Point3D(0d, 0d, 0d);
            Random ran = new Random();

            currentTileX = 0;
            currentTileY = 0;

            OSMWorldScale scale = new OSMWorldScale();

            double mapCenterNormalizedX = scale.NormalizeX(TileService.xTileOffset);
            double mapCenterNormalizedY = scale.NormalizeY(TileService.yTileOffset);

            double maxTiles = Math.Pow(2, mapZoom);
            double tileNumX = mapCenterNormalizedX * maxTiles;
            double tileNumY = mapCenterNormalizedY * maxTiles;

            //X and y are flipped
            double fracX = tileNumX - Math.Floor(tileNumX);
            double fracY = tileNumY - Math.Floor(tileNumY);

            double posFracX = fracX;
            double posFracY = fracY;

            if (posFracX <= 0.5)
            {
                posFracX += 1;
            }

            if (posFracY >= 0.5)
            {
                posFracY -= 1;
            }
            if (mapZoom == 15 || mapZoom == 12)
            {
                posFracY -= 1;
            }

            double tileTranslateX = scale.GetTileXTranslateFix(mapCenterNormalizedX, mapZoom, TileSize);//TileSize * fracX;
            double tileTranslateY = scale.GetTileYTranslateFix(mapCenterNormalizedY, mapZoom, TileSize); //TileSize * fracY;

            Debug.WriteLine($"TranslateFixes: {tileTranslateX} / {tileTranslateY} TileSize {TileSize}");
            Debug.WriteLine($"TranslateFixesWOLessTHanFixes: {TileSize * fracX} / {TileSize * fracY}");
            Debug.WriteLine($"TranslateFixesPositive: {posFracX} / {posFracY}");

            //double fooTest2 =
            //double fooTest2 = scale.GetTileXTranslateFix(mapCenterNormalizedX, mapZoom, TileSize);

            BoundingBoxWireFrameVisual3D box = new BoundingBoxWireFrameVisual3D();
            box.Color = Colors.Yellow;
            double bSize = 1 * TileSize / 32;
            box.Thickness = 1;
            box.BoundingBox = new Rect3D((tileTranslateX) - bSize / 2, tileTranslateY - bSize / 2, 0, bSize, bSize, bSize);
            this.Children.Add(box);

            BoundingBoxWireFrameVisual3D box2 = new BoundingBoxWireFrameVisual3D();
            box2.Color = Colors.LimeGreen;
            double b2Size = 1 * TileSize / 32;
            box2.Thickness = 1;
            box2.BoundingBox = new Rect3D((posFracX * TileSize * -1) - b2Size / 2, (posFracY * TileSize) - b2Size / 2, 0, b2Size, b2Size, b2Size);
            this.Children.Add(box2);

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    int gridPosX = ((x + 1) - midNum);
                    int gridPosY = ((y + 1) - midNum);

                    MapTileVisual tile = new MapTileVisual();

                    tile.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)ran.Next(30, 255), (byte)ran.Next(32, 255), (byte)ran.Next(28, 255)));

                    TranslateTransform3D position = new TranslateTransform3D();

                    position.OffsetX = (gridPosX * TileSize) - TileSize / 2;
                    position.OffsetY = (gridPosY * TileSize) - TileSize / 2;
                    if (TranslateOnZoom)
                    {
                        position.OffsetX -= tileTranslateX;
                        position.OffsetY -= tileTranslateY;
                    }

                    tile.Transform = position;

                    tile.Width = TileSize;
                    tile.Length = TileSize;

                    tile.TileX = gridPosX * -1;
                    tile.TileY = gridPosY;
                    tile.TileZoom = MapZoomLevel;

                    this.Children.Add(tile);
                    Tiles.Add(tile);

                    if (TileGenerator != null)
                    {
                        //log.DebugFormat("Requesting Image for x/y/zoom {0} / {1} / {2}", tile.TileX, tile.TileY, mapZoom);
                        TileGenerator.LoadTileImageAsync(tile,
                                     tile.TileX,
                                     tile.TileY,
                                     MapZoomLevel);
                    }
                    else
                    {
                        Aegir.Util.DebugUtil.LogWithLocation("Could not load image, TileGeneratorWas Null");
                    }
                    tile.UpdateDebugLabels();
                }
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DoCameraMove();
        }

        private void DoCameraMove()
        {
            if (MapCamera != null)
            {
                //Get camera distance from map
                double cameraTargetDistance = 0;
                Point3D position = MapCamera.CameraTarget;

                if (MapCamera.CameraMode == CameraMode.Inspect)
                {
                    double deltaX = MapCamera.CameraPosition.X - MapCamera.CameraTarget.X;
                    double deltaY = MapCamera.CameraPosition.Y - MapCamera.CameraTarget.Y;
                    double deltaZ = MapCamera.CameraPosition.Z - MapCamera.CameraTarget.Z;

                    cameraTargetDistance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
                }
                else if (MapCamera.CameraMode == CameraMode.WalkAround)
                {
                    position = new Point3D();
                }
                if (lastZoom != cameraTargetDistance)
                {
                    //log.DebugFormat("Camera Distance {0} ", cameraTargetDistance);

                    int snappedCameraDistance = (int)Math.Floor((cameraTargetDistance - 100) * zoomInverseFactor) * 200 + 100;
                    double zoomFactor = cameraTargetDistance / 200d;
                    int baseNum = (gridMode == GridMode.Even ? 2 : 3);
                    int SnappedZoomFactor = (int)Math.Ceiling(Math.Log(zoomFactor) / Math.Log((double)baseNum));

                    int zoomLevel = 18 - Math.Max(0, Math.Min(9, SnappedZoomFactor));
                    if (zoomLevel != MapZoomLevel)
                    {
                        MapZoomLevel = zoomLevel;
                    }

                    //log.DebugFormat("Zoom snapped distance/zoom level: {0} / {1}", cameraTargetDistance, SnappedZoomFactor);
                }
                lastZoom = cameraTargetDistance;

                //Camera Calculations done, let's check them.
                //Check if we need to zoom out map

                //No Zoom needed, but check if we are outside our current tile and need to add new ones
                //This is done after zooming as zooming to a new level might increase size of current tile
                //making us still inside the current tile with a new zoom level

                double cameraDeltaX = position.X - MapCenter.X;
                double cameraDeltaY = position.Y - MapCenter.Y;

                int tileX = ((int)(cameraDeltaX * snapInverseFactor)) * -1;
                int tileY = (int)(cameraDeltaY * snapInverseFactor);

                if (tileX != currentTileX || tileY != currentTileY)
                {
                    int panDeltaX = ClampPan(tileX - currentTileX);
                    int panDeltaY = ClampPan(tileY - currentTileY);

                    Aegir.Util.DebugUtil.LogWithLocation($"Panning Grid CameraTile (x/y) {tileX} / {tileY}  CurrentTile (x/y) {currentTileX} / {currentTileY} Delta (x/y) {panDeltaX} / {panDeltaY}");

                    PanGrid(panDeltaX, panDeltaY);
                }
            }
        }

        private void ZoomGrid(int value)
        {
            mapZoom = value;

            TileSize = GetTileSize(value);
            Children.Clear();
            foreach (MapTileVisual tile in Tiles)
            {
                tile.Fill = null;
                tile.Dispose();
            }
            Tiles.Clear();
            InitGrid();
        }

        private int GetTileSize(int ZoomLevel)
        {
            int baseNum = (gridMode == GridMode.Even ? 2 : 3);
            return (int)Math.Max(32 * Math.Pow(baseNum, 18 - ZoomLevel), 32);
        }

        /// <summary>
        /// Pans the Grid Tiles the given amount (only supports one square for now)
        /// </summary>
        /// <param name="panAmountX"></param>
        /// <param name="panAmountY"></param>
        private void PanGrid(int panAmountX, int panAmountY)
        {
            PerfStopwatch ps = PerfStopwatch.StartNew("Pan Grid");
            if (panAmountX == 0 && panAmountY == 0)
            {
                return;
            }

            //If positive move tiles from left edge to right edge
            int xTileIndexToFind = GetXTileEdge(panAmountX);
            int yTileIndexToFind = GetYTileEdge(panAmountY);

            List<MapTileVisual> tilesToMove = new List<MapTileVisual>();

            if (panAmountX != 0)
            {
                tilesToMove.AddRange(Tiles.Where(t => t.TileX == xTileIndexToFind));
            }
            if (panAmountY != 0)
            {
                tilesToMove.AddRange(Tiles.Where(t => t.TileY == yTileIndexToFind));
            }

            OSMWorldScale scale = new OSMWorldScale();

            double mapCenterNormalizedX = scale.NormalizeX(TileService.xTileOffset);
            double mapCenterNormalizedY = scale.NormalizeY(TileService.yTileOffset);

            double tileTranslateX = scale.GetTileXTranslateFix(mapCenterNormalizedX, mapZoom, TileSize);//TileSize * fracX;
            double tileTranslateY = scale.GetTileYTranslateFix(mapCenterNormalizedY, mapZoom, TileSize); //TileSize * fracY;

            foreach (MapTileVisual tile in tilesToMove)
            {
                if (tile.TileX == xTileIndexToFind)
                {
                    tile.TileX += GridSize * panAmountX;
                }
                if (tile.TileY == yTileIndexToFind)
                {
                    tile.TileY += GridSize * panAmountY;
                }

                TranslateTransform3D transform = tile.Transform as TranslateTransform3D;
                //Apply this as a transformation as well
                if (transform != null)
                {
                    transform.OffsetX = ((tile.TileX * TileSize * -1) - TileSize / 2) - tileTranslateX;
                    transform.OffsetY = ((tile.TileY * TileSize) - TileSize / 2) - tileTranslateY;
                }

                tile.UpdateDebugLabels();

                //Send of a request to update the tile
                TileGenerator.LoadTileImageAsync(tile,
                                             tile.TileX,
                                             tile.TileY,
                                             MapZoomLevel);
            }

            currentTileX += panAmountX;
            currentTileY += panAmountY;
            ps.Stop();
        }

        private int GetXTileEdge(int panAmount)
        {
            if (panAmount > 0)
            {
                //we need to find all tiles on left edge. Their index will be currentTileX - gridsize/2 (rounded down)
                return currentTileX - GridSize / 2;
            }
            else if (panAmount < 0)
            {
                return currentTileX + GridSize / 2;
            }

            return 0;
        }

        private int GetYTileEdge(int panAmount)
        {
            //Inverse y axis
            if (panAmount < 0)
            {
                //we need to find all tiles on left edge. Their index will be currentTileX - gridsize/2 (rounded down)
                return currentTileY + GridSize / 2;
            }
            else if (panAmount > 0)
            {
                return (currentTileY - GridSize / 2) + 1;
            }

            return 0;
        }

        private int ClampPan(int pan)
        {
            if (pan > 1)
            {
                return 1;
            }
            if (pan < -1)
            {
                return -1;
            }
            return pan;
        }
    }
}