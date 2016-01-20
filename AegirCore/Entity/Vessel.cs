﻿using AegirCore.Behaviour.Rendering;
using AegirCore.Behaviour.Simulation;
using AegirCore.Behaviour.Vessel;
using AegirCore.Mesh;
using AegirCore.Mesh.Loader;
using AegirCore.Scene;
using log4net;
using System;

namespace AegirCore.Entity
{
    public class Vessel : Node
    {
        private MeshData vesselModel;
        private MeshData hullModel;

        public MeshData VesselModel
        {
            get { return vesselModel; }
            set { vesselModel = value; }
        }

        public MeshData HullModel
        {
            get { return hullModel; }
            set { UpdateHullModel(value); }
        }

        public Vessel()
        {
            this.Name = "Vessel";
            RenderMeshBehaviour RenderMesh = new RenderMeshBehaviour(this);
            this.AddComponent(RenderMesh);

            VesselNavigationBehaviour navBehavour = new VesselNavigationBehaviour(this);
            this.AddComponent(navBehavour);

            WaterSimulation water = new WaterSimulation(this);
            FloatingMesh mesh = new FloatingMesh(this,water.waterMesh);

            this.AddComponent(mesh);
        }
        private void UpdateHullModel(MeshData data)
        {
            if (data == hullModel) return;

            RenderMeshBehaviour renderBehaviour = GetComponent<RenderMeshBehaviour>();
            if(renderBehaviour == null)
            {
                renderBehaviour.Mesh = data;
            }
        }
    }
}