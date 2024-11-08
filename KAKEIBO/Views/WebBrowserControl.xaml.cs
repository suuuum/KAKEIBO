using CefSharp;
using KAKEIBO.ViewModels;
using System.Windows.Controls;

namespace KAKEIBO.Views
{
    /// <summary>
    /// Interaction logic for WebBrowserControl
    /// </summary>
    public partial class WebBrowserControl : UserControl
    {
        private WebBrowserControlViewModel ViewModel;
        public WebBrowserControl()
        {
            InitializeComponent();
            ViewModel = this.DataContext as WebBrowserControlViewModel;
            browser.TitleChanged += Browser_TitleChanged;
            ViewModel.WebBrowser = browser;
        }

        private void Browser_TitleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            ViewModel.PageTitle = (string)e.NewValue; // PageTitleプロパティにタイトルを設定
            ViewModel.SetTitle(ViewModel.PageTitle);
        }

    }
}
