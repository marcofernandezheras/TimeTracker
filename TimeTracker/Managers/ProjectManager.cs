using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TimeTracker.Models;
using TimeTracker.Util;
using TimeTracker.ViewModels;
using TimeTracker.Views;

namespace TimeTracker.Managers
{
    public class ProjectManager : AbstractManager<Project>
    {       
        //INIT
        public override async Task<bool> Init()
        {
            await base.Init();

            //Init Data
            using (TimeTrackerModel db = new TimeTrackerModel())
            {
                Status = "Cargando proyectos...";
                var proyects = await db.Projects.ToListAsync();
                proyects.ForEach(p => _items.Add(p));

                Status = "ProjectManager iniciado correctamente!";
                return true;
            }
        }

        //ADD
        protected override void CreateItem(string value)
        {
            var dialog = new ProjectWindow(value);
            dialog.ShowDialog();

            var vm = dialog.DataContext as ProjectVM;
            if (vm.Saved)
            {
                SelectedItem = vm.Project;
                _items.Add(vm.Project);
                Status = "Proyecto añadido correctamente";
            }
        }

        //UPDATE
        protected override void UpdateItem()
        {
            var dialog = new ProjectWindow(SelectedItem);
            dialog.ShowDialog();

            var vm = dialog.DataContext as ProjectVM;
            if (vm.Saved)
            {
                NewItem = SelectedItem.Name;
                Status = "Proyecto actualizado correctamente";
            }
        }

        //DELETE
        protected override async Task<bool> DeleteItem()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("¿Estas seguro de borrar el proyecto?", "confirmación de borrado", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (var db = new TimeTrackerModel())
                {
                    db.Projects.Attach(SelectedItem);
                    db.Entry(SelectedItem).State = EntityState.Deleted;
                    var rows = await db.SaveChangesAsync();
                    if (rows > 0)
                    {
                        _items.Remove(SelectedItem);
                        Status = "Proyecto borrado correctamente";
                    }
                    return rows > 0;
                }
            }
            return false;
        }
    }
}
