﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Data;

namespace Aegir.View.Rendering.Tool
{

    public class ManipulatorGizmo
    {
        private EventableBindableTranslateManipulator XTranslator;
        private EventableBindableTranslateManipulator YTranslator;
        private EventableBindableTranslateManipulator ZTranslator;

        private EventableBindableRotateManipulator XRotation;
        private EventableBindableRotateManipulator YRotation;
        private EventableBindableRotateManipulator ZRotation;

        private CombinedManipulator translationManipulator;

        public TransformMode Mode { get; set; }
        public ManipulatorGizmoTransformHandler Target { get; set; }
        public HelixViewport3D Viewport { get; private set; }

        public ManipulatorGizmo(HelixViewport3D viewport, ManipulatorGizmoTransformHandler target)
        {

            Target = target;
            //target.Transform.Changed += Transform_Changed;
            CubeVisual3D cube = new CubeVisual3D();
            cube.Visible = false;
            cube.Fill = new SolidColorBrush(Color.FromRgb(200, 0, 50));

            viewport.Children.Add(cube);
            translationManipulator = new CombinedManipulator();
            Binding transformBinding = new Binding();
            transformBinding.Source = cube;
            transformBinding.Mode = BindingMode.TwoWay;
            transformBinding.NotifyOnSourceUpdated = true;
            transformBinding.NotifyOnTargetUpdated = true;
            transformBinding.Path = new PropertyPath(nameof(CubeVisual3D.Transform));
            BindingOperations.SetBinding(translationManipulator, CombinedManipulator.TargetTransformProperty, transformBinding);
            //translationManipulator.TargetTransform = target.Transform;
            Viewport = viewport;

            viewport.Children.Add(translationManipulator);
        }

        private void Transform_Changed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Transform Changed");
        }
    }
}