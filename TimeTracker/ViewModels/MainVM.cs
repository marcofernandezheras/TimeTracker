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

namespace TimeTracker.ViewModels 
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Status { get; set; } = "Iniciando...";

        #region Projects
        private readonly ObservableCollection<Project> _projects = new ObservableCollection<Project>();
        public IEnumerable<Project> Projects { get { return _projects; } }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get { return _selectedProject; }
            set {
                _selectedProject = value;
                UpdateProyectcommand.OnCanExecuteChanged();
                DeleteProyectcommand.OnCanExecuteChanged();
            }
        }

        //ADD
        private string _newProject = "";
        public string NewProject {
            get => _newProject;
            set
            {
                if (SelectedProject == null && !string.IsNullOrEmpty(value))
                {
                    var dialog = new ProjectWindow(value);
                    dialog.ShowDialog();

                    var vm = dialog.DataContext as ProjectVM;
                    if (vm.Saved)
                    {
                        SelectedProject = vm.Project;
                        _projects.Add(vm.Project);
                        Status = "Proyecto añadido correctamente";
                    }
                }                
                _newProject = SelectedProject?.Name;
            }
        }

        //UPDATE
        public Command UpdateProyectcommand { get; set; }
        private void UpdateSelectedProject()
        {
            var dialog = new ProjectWindow(SelectedProject);
            dialog.ShowDialog();

            var vm = dialog.DataContext as ProjectVM;
            if (vm.Saved)
            {
                NewProject = SelectedProject.Name;
                Status = "Proyecto actualizado correctamente";
            }
        }

        //DELETE
        public Command DeleteProyectcommand { get; set; }
        private async Task<bool> DeleteSelectedProject()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("¿Estas seguro de borrar el proyecto?", "confirmación de borrado", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using(var db = new TimeTrackerModel())
                {
                    db.Projects.Attach(SelectedProject);
                    db.Entry(SelectedProject).State = EntityState.Deleted;
                    var rows = await db.SaveChangesAsync();
                    if(rows > 0)
                    {
                        _projects.Remove(SelectedProject);
                        Status = "Proyecto borrado correctamente";
                    }
                    return rows > 0;
                }
            }
            return false;
        }
        #endregion


        /// <summary>
        /// Load Proyects
        /// </summary>
        /// <returns>true</returns>
        public async Task<bool> Init()
        {

            #region Init Comands
            //Init Proyect Commands
            UpdateProyectcommand = new Command(
                (_) => UpdateSelectedProject(),
                (_) => SelectedProject != null
            );

            //Init Proyect Commands
            DeleteProyectcommand = new Command(
                async (_) => await DeleteSelectedProject(),
                (_) => SelectedProject != null
            );
            #endregion

            //Init Data
            using (TimeTrackerModel db = new TimeTrackerModel())
            {
                Status = "Cargando proyectos...";
                var proyects = await db.Projects.ToListAsync();
                proyects.ForEach(p => _projects.Add(p));



                Status = "Modelo iniciado correctamente!";
                return true;
            }
        }
    }
}
