﻿using AegirCore.Behaviour.Mesh;
using AegirCore.Behaviour.Simulation;
using AegirCore.Behaviour.Vessel;
using AegirCore.Mesh;
using AegirCore.Mesh.Loader;
using AegirCore.Scene;
using AegirCore.Simulation.Water;
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

        public Vessel(WaterMesh water)
        {
            this.Name = "Vessel";
            MeshBehaviour RenderMesh = new MeshBehaviour(this);
            this.AddComponent(RenderMesh);

            VesselNavigationBehaviour navBehavour = new VesselNavigationBehaviour(this);
            this.AddComponent(navBehavour);

            FloatingMesh mesh = new FloatingMesh(this);

            this.AddComponent(mesh);
        }
        private void UpdateHullModel(MeshData data)
        {
            if (data == hullModel) return;

            MeshBehaviour renderBehaviour = GetComponent<MeshBehaviour>();
            if(renderBehaviour != null)
            {
                renderBehaviour.Mesh = data;
            }
        }
    }
}