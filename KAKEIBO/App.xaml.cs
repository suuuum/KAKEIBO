using KAKEIBO.Service;
using KAKEIBO.Services;
using KAKEIBO.Views;
using KAKEIBO.ViewModels;
using Prism.Ioc;
using System.Text;
using System.Windows;
using Prism.Services.Dialogs;
using System.Threading.Tasks;
using System;

namespace KAKEIBO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IErrorHandlingService _errorHandlingService;
        protected override void OnStartup(StartupEventArgs e)
        {
            // エンコーディングプロバイダーを登録
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            base.OnStartup(e);

            _errorHandlingService = Container.Resolve<IErrorHandlingService>();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IFileDialogService, FileDialogService>();
            containerRegistry.RegisterSingleton<IDataAccessor, DataBaseAccessor>();
            containerRegistry.RegisterSingleton<CsvImporter>();
            containerRegistry.RegisterSingleton<CsvImportControlViewModel>();
            containerRegistry.RegisterSingleton<IDialogService, Service.DialogService>();
            containerRegistry.RegisterSingleton<IErrorHandlingService, ErrorHandlingService>();

        }
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // DataBaseAccessorのCreateメソッドを呼び出す
            var dataAccessor = Container.Resolve<IDataAccessor>();
            if (dataAccessor is DataBaseAccessor dbAccessor)
            {
                dbAccessor.CreateDatabase();
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.Handled = true;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.SetObserved();
        }

        private void HandleException(Exception ex)
        {
            _errorHandlingService.HandleError(ex);
        }
    }
}
