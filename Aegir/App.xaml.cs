﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Aegir
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Shell appShell;
        public App()
        {
            var foo = new ViewModel.ViewModelLocator();
            appShell = new Shell();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            appShell.ShellLoaded();
        }

    }
}