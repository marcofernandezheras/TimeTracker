using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Util;

namespace TimeTracker.Managers
{
    public abstract class AbstractManager<T> : INotifyPropertyChanged, IStatus where T : class
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Status { get; set; }

        protected readonly ObservableCollection<T> _items = new ObservableCollection<T>();
        public IEnumerable<T> Items { get { return _items; } }

        public Command UpdateCommand { get; set; }
        public Command DeleteCommand { get; set; }

        protected abstract void UpdateItem();
        protected abstract void CreateItem(string value);
        protected abstract Task<bool> DeleteItem();

        public virtual Task<bool> Init()
        {
            #region Init Comands
            //Update Command
            UpdateCommand = new Command(
                (_) => UpdateItem(),
                (_) => SelectedItem != null
            );

            //Delete Command
            DeleteCommand = new Command(
                async (_) => await DeleteItem(),
                (_) => SelectedItem != null
            );
            #endregion

            return Task.FromResult(true);
        }

        protected T _selectedItem;
        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                UpdateCommand.OnCanExecuteChanged();
                DeleteCommand.OnCanExecuteChanged();
            }
        }

        protected string _newItem = "";
        public string NewItem {
            get => _newItem;
            set {
                if(SelectedItem == null && !string.IsNullOrWhiteSpace(value))
                {
                    CreateItem(value);
                }
                _newItem = SelectedItem?.ToString();
            }
        }
    }
}
