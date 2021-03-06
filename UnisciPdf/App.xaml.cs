﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UnisciPdf
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            ConventionManager.ApplyValidation = (binding, viewModelType, property) =>
                {
                    binding.ValidatesOnExceptions = true;
                };
        }
    }
}
