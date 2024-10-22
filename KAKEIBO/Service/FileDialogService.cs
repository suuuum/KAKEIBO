using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows;

namespace KAKEIBO.Services
{
    public class FileDialogService : IFileDialogService
    {
        public Task<string> OpenFileDialogAsync(string filter = "CSVファイル (*.csv)|*.csv")
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = filter,
                    Title = "CSVファイルを選択してください"
                };

                return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
            }).Task;
        }
    }
}