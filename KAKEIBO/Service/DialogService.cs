using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KAKEIBO.Service
{
    public class DialogService : IDialogService
    {
        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            throw new NotImplementedException();
        }

        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            throw new NotImplementedException();
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            throw new NotImplementedException();
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            throw new NotImplementedException();
        }

        public Task ShowDialogAsync(string title, string message)
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(message, title);
            }).Task;
        }
        public void ShowErrorDialog(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
