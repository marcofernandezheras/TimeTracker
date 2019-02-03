using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Models;
using System.Data.Entity;
using System.Threading;
using System.Windows;
using TimeTracker.Views;
using TimeTracker.Util;
using TimeTracker.Managers;

namespace TimeTracker.ViewModels 
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Status { get; set; } = "Iniciando...";

        #region Managers
        public ProjectManager ProjManager { get; private set; }
        #endregion


        /// <summary>
        /// Load Managers
        /// </summary>
        /// <returns>true</returns>
        public async Task<bool> Init()
        {
            var ok = true;

            ProjManager = new ProjectManager();
            ProjManager.PropertyChanged += Manager_PropertyChanged;
            ok &= await ProjManager.Init();

            Status = ok ? "MainVM iniciado correctamente" : "Error al iniciar MainVM";
            return ok;
        }

        private void Manager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("Status".Equals(e.PropertyName))
            {
                var status = sender as IStatus;
                this.Status = status?.Status;
            }
        }
    }
}
