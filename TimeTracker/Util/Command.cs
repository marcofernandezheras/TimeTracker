using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TimeTracker.Util
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<object> ExecuteAction;
        private readonly Func<object, bool> CanExecuteFunc;

        public Command(Action<object> ex, Func<object, bool> can)
        {
            this.CanExecuteFunc = can;
            this.ExecuteAction = ex;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc != null ? 
                CanExecuteFunc.Invoke(parameter) : true;
        }

        public void Execute(object parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
