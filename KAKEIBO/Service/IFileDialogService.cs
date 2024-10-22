using System.Threading.Tasks;

namespace KAKEIBO.Services
{
    public interface IFileDialogService
    {
        Task<string> OpenFileDialogAsync(string filter = "CSVファイル (*.csv)|*.csv");
    }
}