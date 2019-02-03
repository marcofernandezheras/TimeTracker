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
    public class ProjectVM
    {
        public Project Project { get; set; }
        public bool Saved { get; set; } = false;

        private IClosable window;

        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }

        private ProjectVM(IClosable window)
        {
            this.window = window;
            SaveCommand = new Command(
                async (_) => await Save(),
                (_) => this.CanSave()
            );

            CancelCommand = new Command( (_) => window?.Close(), null);
        }

        public ProjectVM(string projectName, IClosable window) : this(window)
        {
            this.Project = new Project
            {
                Id = 0,
                Name = projectName
            };
        }

        public ProjectVM(Project project, IClosable window) : this(window)
        {
            this.Project = project;
        }

        
        public bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Project.Name);
        }

        public async Task<bool> Save()
        {
            using(var db = new TimeTrackerModel())
            {
                if(Project.Id == 0)
                {
                    db.Projects.Add(Project);
                } else
                {
                    db.Projects.Attach(Project);
                    db.Entry(Project).State = EntityState.Modified;
                }

                var rows = await db.SaveChangesAsync();
                Saved = rows == 1;

                window?.Close();

                return Saved;
            }
        }
    }
}
