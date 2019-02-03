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

namespace TimeTracker.Managers
{
    public class TaskManager<Q, T> : AbstractManager<Models.Task> where T : AbstractManager<Q>  where Q : class
    {
        private readonly T _parentManager;

        public TaskManager(T parent)
        {
            _parentManager = parent;
            _parentManager.PropertyChanged += Parent_SelectedChanged;
        }

        protected override void CreateItem(string value)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> DeleteItem()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateItem()
        {
            throw new NotImplementedException();
        }

        private async Task<bool> LoadTasks(Q parent)
        {
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
