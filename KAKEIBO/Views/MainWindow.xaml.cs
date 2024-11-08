using System.Linq;
using System.Windows;
using AvalonDock;
using AvalonDock.Layout;
using KAKEIBO.Service;
using KAKEIBO.ViewModels;

namespace KAKEIBO.Views
{
    public partial class MainWindow : Window
    {
        public LayoutPanel LayoutPanel { get; private set; }
        public DockingManager DockingManager { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            WindowAgent.MainWindow = this;
            LayoutPanel = MainPanel;
            DockingManager = dockManager;

            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
            {
                var documentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            }
        }
    }
}