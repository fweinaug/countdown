using System;
using System.Windows.Input;

namespace CountdownApp
{
  public class Command : ICommand
  {
    public event EventHandler CanExecuteChanged;

    private readonly Action action;
    private readonly Func<bool> canExecute;

    public Command(Action action, Func<bool> canExecute = null)
    {
      this.action = action;
      this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
      return canExecute == null || canExecute();
    }

    public void Execute(object parameter)
    {
      action();
    }

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}