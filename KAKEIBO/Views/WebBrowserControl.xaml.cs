using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using KAKEIBO.ViewModels;
using System.Windows.Controls;

namespace KAKEIBO.Views
{
    /// <summary>
    /// Interaction logic for WebBrowserControl
    /// </summary>
    public partial class WebBrowserControl : UserControl
    {
        public WebBrowserControlViewModel ViewModel { get; private set; }
        public WebBrowserControl()
        {
            InitializeComponent();
            ViewModel = this.DataContext as WebBrowserControlViewModel;
            browser.NavigationCompleted += Browser_NavigationCompleted;
            browser.CoreWebView2InitializationCompleted += Browser_CoreWebView2InitializationCompleted;
            InitializeAsync();
            ViewModel.WebBrowser = browser;
        }

        private async void InitializeAsync()
        {
            await browser.EnsureCoreWebView2Async(null);
        }

        private void Browser_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                browser.CoreWebView2.NewWindowRequested += Browser_NewWindowRequested;
            }
        }

        private void Browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            ViewModel.PageTitle = browser.CoreWebView2.DocumentTitle;
            ViewModel.SetTitle(ViewModel.PageTitle);
        }

        private void Browser_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            // 右クリックメニューから新しいウィンドウを開く場合の処理
            if (e.Uri != null && e.IsUserInitiated)
            {
                e.Handled = true; // デフォルトのナビゲーションをキャンセル
                ViewModel.OpenNewBrowserWindow(e.Uri); // 新しいウィンドウを開く
            }
        }
    }
}
