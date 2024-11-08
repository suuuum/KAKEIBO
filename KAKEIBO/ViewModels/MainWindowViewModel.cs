using Prism.Mvvm;
using Prism.Commands;
using AvalonDock.Layout;
using KAKEIBO.Views;
using System.Linq;
using KAKEIBO.Service;

namespace KAKEIBO.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly WindowAgent _window_agent;
        private string _title = "KAKEIBO Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand OpenCsvImportCommand { get; private set; }
        public DelegateCommand OpenPaymentViewerCommand { get; private set; }
        public DelegateCommand OpenTrendCommand { get; private set; }

        public DelegateCommand OpenWebBrowserCommand { get; private set; }

        public MainWindowViewModel(WindowAgent windowAgent)
        {
            _window_agent = windowAgent;
            OpenCsvImportCommand = new DelegateCommand(OpenCsvImport);
            OpenPaymentViewerCommand = new DelegateCommand(OpenPaymentViewer);
            OpenTrendCommand = new DelegateCommand(OpenTrend);
            OpenWebBrowserCommand = new DelegateCommand(OpenWebBrowser);
        }


        private void OpenCsvImport()
        =>_window_agent.OpenCsvImport();

        private void OpenPaymentViewer()
        => _window_agent.OpenPaymentViewer();

        private void OpenTrend()
        =>_window_agent.OpenTrend();
        private void OpenWebBrowser()
        {
            _window_agent.AddWebBrouserDoc();
        }
    }
}