﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Aegir.View.Rendering
{
    public class ManipulatorGizmoTransformTarget
    {
        public Transform3D Transform { get; set; }

        private double translateValueX;
        private double translateValueY;
        private double translateValueZ;
        private Point3D gizmoPos;

        public double TranslateValueX
        {
            get { return translateValueX; }
            set { translateValueX = value; }
        }

        public double TranslateValueY
        {
            get { return translateValueY; }
            set { translateValueY = value; }
        }


        public double TranslateValueZ
        {
            get { return translateValueZ; }
            set { translateValueZ = value; }
        }


        public Point3D GizmoPosition
        {
            get { return gizmoPos; }
            set { gizmoPos = value; }
        }


        public ManipulatorGizmoTransformTarget()
        {

        }
    }
}
