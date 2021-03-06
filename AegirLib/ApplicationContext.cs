﻿using AegirLib.Asset;
using AegirLib.Messages;
using AegirLib.Persistence;
using AegirLib.Persistence.Data;
using AegirLib.Persistence.Persisters;
using AegirLib.Project;
using AegirLib.Scene;
using AegirLib.Simulation;
using System;
using System.Xml.Linq;
using TinyMessenger;

namespace AegirLib
{
    public class ApplicationContext
    {
        public SimulationEngine Engine { get; private set; }
        public PersistenceHandler SaveLoadHandler { get; private set; }
        public ITinyMessengerHub MessageHub { get; private set; }
        public SceneGraph Scene { get; private set; }

        public ApplicationContext()
        {
            //Setting up asset cache
            AssetCache.DefaultInstance = new AssetCache();
            MessageHub = new TinyMessengerHub();
            Scene = new SceneGraph();
            Scene.Messenger = MessageHub;
            SaveLoadHandler = new PersistenceHandler();
            //Adding specific persisters
            SaveLoadHandler.AddPersister(new ScenePersister() { Graph = Scene });
            //Set up Engine
            Engine = new SimulationEngine(Scene);
            Engine.Messenger = MessageHub;
        }

        public void Init()
        {
            Scene.Init();

            SaveLoadHandler.LoadDefault();

            //Call changed scenegraph
            MessageHub.Publish<ScenegraphChanged>(new ScenegraphChanged(Scene, this));

            if (!Engine.IsStarted)
            {
                Engine.Start();
            }

            //Init the scenegraph
        }
    }
}