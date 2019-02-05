using MahApps.Metro.Controls;
using TimeTracker.Util;
using TimeTracker.ViewModels;

namespace TimeTracker.Views
{
    /// <summary>
    /// Lógica de interacción para TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : MetroWindow, IClosable
    {
        public TaskWindow(Models.Task task)
        {
            InitializeComponent();
            this.DataContext = new TaskVM(task, this);
        }
    }
}
