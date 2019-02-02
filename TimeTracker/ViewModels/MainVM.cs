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

namespace TimeTracker.ViewModels 
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Status { get; set; } = "Iniciando...";

        private readonly ObservableCollection<Project> _proyects = new ObservableCollection<Project>();

        public IEnumerable<Project> Proyects { get { return _proyects; } }
        public bool LoadingProcts { get; set; } = true;



        public async Task<bool> Init()
        {
            using (TimeTrackerModel db = new TimeTrackerModel())
            {
                Status = "Cargando proyectos...";
                var proyects = await db.Projects.ToListAsync();
                proyects.ForEach(p => _proyects.Add(p));



                Status = "Modelo iniciado correctamente!";
                return true;
            }
        }
    }
}
