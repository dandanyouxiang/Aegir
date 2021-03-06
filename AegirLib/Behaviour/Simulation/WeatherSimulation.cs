﻿using AegirLib.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AegirLib.Behaviour.Simulation
{
    public class WeatherSimulation : BehaviourComponent
    {
        public WeatherSimulation(Entity parent)
            : base(parent) { }

        public override void Deserialize(XElement data)
        {
        }

        public override XElement Serialize()
        {
            return new XElement(this.GetType().Name);
        }
    }
}