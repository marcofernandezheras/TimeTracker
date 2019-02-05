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
        public ProjectManager ProjectMan { get; private set; }

        public TaskManager<Project, ProjectManager> TaskMan { get; private set; }

        public TaskManager<Models.Task, TaskManager<Project, ProjectManager>> SubTaskMan { get; private set; }
        #endregion

        public string NewTime { get; set; }

        /// <summary>
        /// Load Managers
        /// </summary>
        /// <returns>true</returns>
        public async Task<bool> Init()
        {
            var ok = true;

            ProjectMan = new ProjectManager();
            ProjectMan.PropertyChanged += Manager_PropertyChanged;
            ok &= await ProjectMan.Init();

            TaskMan = new TaskManager<Project, ProjectManager>(ProjectMan);
            TaskMan.PropertyChanged += Manager_PropertyChanged;
            ok &= await TaskMan.Init();

            SubTaskMan = new TaskManager<Models.Task, TaskManager<Project, ProjectManager>>(TaskMan);
            SubTaskMan.PropertyChanged += Manager_PropertyChanged;
            ok &= await SubTaskMan.Init();

            NewTime = DateTime.Now.ToString("HH:mm");

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
