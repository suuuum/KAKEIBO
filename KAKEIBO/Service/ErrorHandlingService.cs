using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAKEIBO.Service
{
    public interface IErrorHandlingService
    {
        void HandleError(Exception ex);
    }

    public class ErrorHandlingService : IErrorHandlingService
    {
        private readonly DialogService _dialogService;

        public ErrorHandlingService(IDialogService dialogService)
        {
            _dialogService = (DialogService)dialogService;
        }

        public void HandleError(Exception ex)
        {
            _dialogService.ShowErrorDialog("エラーが発生しました", ex.Message);
        }
    }
}
