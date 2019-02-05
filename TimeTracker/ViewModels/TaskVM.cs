using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeTracker.Models;
using TimeTracker.Util;

namespace TimeTracker.ViewModels
{
    public class TaskVM
    {
        public Models.Task Task { get; set; }
        public bool Saved { get; set; } = false;

        private IClosable window;

        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }

        private TaskVM(IClosable window)
        {
            this.window = window;
            SaveCommand = new Command(
                async (_) => await Save(),
                (_) => this.CanSave()
            );

            CancelCommand = new Command( (_) => window?.Close(), null);
        }

        public TaskVM(Models.Task task, IClosable window) : this(window)
        {
            this.Task = task;
        }

        
        public bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Task.Name);
        }

        public async Task<bool> Save()
        {
            using(var db = new TimeTrackerModel())
            {
                if(Task.Id == 0)
                {
                    db.Tasks.Add(Task);
                } else
                {
                    db.Tasks.Attach(Task);
                    db.Entry(Task).State = EntityState.Modified;
                }

                var rows = await db.SaveChangesAsync();
                Saved = rows == 1;

                window?.Close();

                return Saved;
            }
        }
    }
}
