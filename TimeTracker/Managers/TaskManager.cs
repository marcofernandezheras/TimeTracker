using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Models;
using TimeTracker.Util;
using System.Data.Entity;
using TimeTracker.Views;
using TimeTracker.ViewModels;
using System.Windows;

namespace TimeTracker.Managers
{
    public class TaskManager<Q, T> : AbstractManager<Models.Task> where T : AbstractManager<Q>  where Q : class
    {
        private readonly T _parentManager;
        private Q _parent = null;

        public TaskManager(T parent)
        {
            _parentManager = parent;
            _parentManager.PropertyChanged += Parent_SelectedChanged;
        }

        protected override void CreateItem(string value)
        {
            if (_parent == null) return;

            var task = new Models.Task
            {
                Id = 0,
                Name = value
            };

            if(_parent is Project)
            {
                var proj = _parent as Project;
                task.IdProject = proj.Id;
            }
            else if (_parent is Models.Task)
            {
                var tParent = _parent as Models.Task;
                task.IdProject = tParent.IdProject;
                task.IdParent = tParent.Id;
            }

            var dialog = new TaskWindow(task);
            dialog.ShowDialog();

            var vm = dialog.DataContext as TaskVM;
            if (vm.Saved)
            {
                SelectedItem = vm.Task;
                _items.Add(vm.Task);
                Status = SelectedItem.IdParent != null ? "SubTarea añadida correctamente" : "Tarea añadida correctamente";
            }
        }

        protected override async Task<bool> DeleteItem()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("¿Estas seguro de borrar la tarea?", "Confirmación de borrado", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (var db = new TimeTrackerModel())
                {
                    try
                    {
                        db.Tasks.Attach(SelectedItem);
                        db.Entry(SelectedItem).State = EntityState.Deleted;

                        var rows = await db.SaveChangesAsync();
                        if (rows > 0)
                        {
                            Status = SelectedItem.IdParent != null ? "Subtarea borrada correctamente" : "Tarea borrada correctamente";
                            _items.Remove(SelectedItem);
                        }
                        return rows > 0;
                    } catch (Exception ex)
                    {
                        this.Status = "Error al borrar Tarea";
                    }
                }
            }
            return false;
        }

        protected override void UpdateItem()
        {
            var dialog = new TaskWindow(SelectedItem);
            dialog.ShowDialog();

            var vm = dialog.DataContext as TaskVM;
            if (vm.Saved)
            {
                NewItem = SelectedItem.Name;
                Status = "Tarea actualizada correctamente";
            }
        }

        private async Task<bool> LoadTasks(Q parent)
        {
            this._parent = parent;
            _items.Clear();
            using(var db = new TimeTrackerModel())
            {
                if(parent is Project)
                {
                    var project = parent as Project;
                    var mainTasks = await (from t in db.Tasks
                                           where t.IdProject == project.Id && t.IdParent == null select t).ToListAsync();
                    mainTasks.ForEach(t => _items.Add(t));
                }
                else if (parent is Models.Task)
                {
                    var task = parent as Models.Task;
                    var subTasks = await (from t in db.Tasks
                                           where t.IdParent == task.Id
                                           select t).ToListAsync();
                    subTasks.ForEach(t => _items.Add(t));
                }
            }
            return true;
        }

        private async void Parent_SelectedChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("SelectedItem".Equals(e.PropertyName))
            {
                await LoadTasks(_parentManager.SelectedItem);
            }
        }
    }
}
