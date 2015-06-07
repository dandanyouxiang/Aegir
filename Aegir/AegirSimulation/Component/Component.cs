﻿using AegirLib.Simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AegirLib.Component
{
    public abstract class Component
    {
        private readonly bool browsable;
        private readonly bool removable;

        /// <summary>
        /// Backing field for visibility
        /// </summary>
        private bool isVisible;
        /// <summary>
        /// Share deltatime between update and render, (true will give slightly less accuracy).
        /// </summary>
        protected bool shareDeltatime;
        /// <summary>
        /// Allow only one of this component on an actor
        /// </summary>
        protected bool isUnique;

        
        public bool Browsable { get { return this.browsable; } }
        public bool Removable { get { return this.removable; } }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
            }
        }
        public Component()
        {
            this.browsable = true;
            this.removable = true;
        }
        public Component(bool browsable, bool removable)
        {
            this.browsable = browsable;
            this.removable = removable;
            this.isVisible = true;
        }
        public virtual void Load()
        {

        }
        public virtual void Render()
        {

        }
        public virtual void Update(DeltaTime delta)
        {

        }
        public string GetOrigin() 
        {
            var assembly = this.GetType().Assembly;
            return assembly.FullName;
        }
    }
}
