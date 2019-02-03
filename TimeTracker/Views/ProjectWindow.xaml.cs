using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TimeTracker.Models;
using TimeTracker.Util;
using TimeTracker.ViewModels;

namespace TimeTracker.Views
{
    /// <summary>
    /// Lógica de interacción para ProjectWindow.xaml
    /// </summary>
    public partial class ProjectWindow : Window, IClosable
    {   

        public ProjectWindow(string proyectName)
        {
            InitializeComponent();
            this.DataContext = new ProjectVM(proyectName, this);
        }

        public ProjectWindow(Project project)
        {
            InitializeComponent();
            this.DataContext = new ProjectVM(project, this);
        }
    }
}
