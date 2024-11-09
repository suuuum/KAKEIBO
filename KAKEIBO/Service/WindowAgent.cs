using AvalonDock.Layout;
using KAKEIBO.ViewModels;
using KAKEIBO.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace KAKEIBO.Service
{
    public class WindowAgent
    {
        /// <summary>
        /// メインウィンドウ
        /// </summary>
        public static MainWindow MainWindow { get; set; }

        /// <summary>
        /// 設定されているブラウザIDの最大値(ウィンドウが追加されると増える)
        /// </summary>
        private long BrowserIdMax { get; set; } = 1;


        /// <summary>
        /// 新規WebBrowser画面をメインウインドウに追加します。
        /// </summary>
        public void AddWebBrowserDoc()
        {
            var newBrowserDocLayout = new LayoutDocument { Title = "Browser", ContentId = "Browser" + (BrowserIdMax + 1), Content = new WebBrowserControl() };
            BrowserIdMax++;
            ((WebBrowserControlViewModel)((WebBrowserControl)newBrowserDocLayout.Content).DataContext).Id = newBrowserDocLayout.ContentId;
            AddDocument<WebBrowserControl>(newBrowserDocLayout);
        }
        /// <summary>
        /// 別タブ表示でのWebBrowser画面追加を行います。
        /// </summary>
        public void OpenAnotherTabWebBrowserDoc(string url)
        {
            var window = new WebBrowserControl();
            var newBrowserDocLayout = new LayoutDocument { Title = "Browser", ContentId = "Browser" + (BrowserIdMax + 1), Content = window };
            BrowserIdMax++;
            ((WebBrowserControlViewModel)((WebBrowserControl)newBrowserDocLayout.Content).DataContext).Id = newBrowserDocLayout.ContentId;
            AddDocument<WebBrowserControl>(newBrowserDocLayout);
            window.ViewModel.CurrentUrl = url;
        }

        public void SetWebBrowserTitle(string contentId, string title)
         => SetTitile<WebBrowserControl>(contentId, title);

        public void OpenCsvImport()
            => AddOrActiveDocument<CsvImportControl>(new LayoutDocument { Title = "CSV Import", ContentId = "CSV Import" + (BrowserIdMax + 1), Content = new CsvImportControl() });
        public void OpenPaymentViewer()
           => AddOrActiveDocument<PaymentViewerControl>(new LayoutDocument { Title = "Payment Viewer", ContentId = "Payment Viewer" + (BrowserIdMax + 1), Content = new PaymentViewerControl() });
        public void OpenTrend()
           => AddOrActiveDocument<TrendControl>(new LayoutDocument { Title = "Trend Viewer", ContentId = "CSV Import" + (BrowserIdMax + 1), Content = new TrendControl() });


        /// <summary>
        /// 画面をメインウインドウに追加し,Activeにします
        /// すでに同じ画面が存在する場合はそのグループに追加されます。
        /// </summary>
        /// <typeparam name="T">追加したい画面のクラス</typeparam>
        /// <param name="docLayout">追加したいレイアウトドキュメント</param>
        private void AddDocument<T>(LayoutDocument docLayout) where T : UserControl
        {
            //メインウィンドウのDocumentPaneのリストを取得
            var existingDocumentPanes = MainWindow.LayoutPanel.Descendents().OfType<LayoutDocumentPane>();
            //指定された画面のクラスを持つDocumentPaneを取得
            var haveSameDocumentPane = existingDocumentPanes.FirstOrDefault(x => x.Descendents().OfType<LayoutDocument>().Any(x => x.Content.GetType() == typeof(T)));
            if (haveSameDocumentPane != null)
            {
                haveSameDocumentPane.Children.Add(docLayout);
                docLayout.IsActive = true;
                return;
            }

            //メインウィンドウに対象がない場合はフローティングウィンドウを探しに行く
            var floatingWindows = GetFloatingWindows();
            foreach (var window in floatingWindows)
            {
                haveSameDocumentPane = window.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault(x => x.Descendents().OfType<LayoutDocument>().Any(x => x.Content.GetType() == typeof(T)));
                if (haveSameDocumentPane != null)
                {
                    haveSameDocumentPane.Children.Add(docLayout);
                    docLayout.IsActive = true;
                    return;
                }
            }

            //同じ画面が存在しない場合はメインウィンドウに追加する
            if (existingDocumentPanes.FirstOrDefault() != null)
            {
                existingDocumentPanes.FirstOrDefault().Children.Add(docLayout);
            }
            else
            {
                var newDocpane = new LayoutDocumentPane();
                newDocpane.Children.Add(docLayout);
                MainWindow.LayoutPanel.Children.Add(newDocpane);
            }
            docLayout.IsActive = true;
        }

        /// <summary>
        /// 画面をメインウインドウに追加します
        /// すでに存在している場合はActiveにします
        /// </summary>
        /// <typeparam name="T">追加したい画面のクラス</typeparam>
        /// <param name="docLayout">追加したいレイアウトドキュメント</param>
        private void AddOrActiveDocument<T>(LayoutDocument docLayout) where T : UserControl
        {
            var target = MainWindow.LayoutPanel.Descendents().OfType<LayoutDocument>().FirstOrDefault(x => x.Content.GetType() == typeof(T));
            if (target != null)
            {
                target.IsActive = true;
                return;
            }
            var floatingWindows = GetFloatingWindows();
            foreach (var window in floatingWindows)
            {
                target = window.Descendents().OfType<LayoutDocument>().FirstOrDefault(x => x.Content.GetType() == typeof(T));
                if (target != null)
                {
                    target.IsActive = true;
                    return;
                }
            }
            var existingDocumentPane = MainWindow.LayoutPanel.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (existingDocumentPane != null)
            {
                existingDocumentPane.Children.Add(docLayout);
            }
            else
            {
                var newDocpane = new LayoutDocumentPane();
                newDocpane.Children.Add(docLayout);
                MainWindow.LayoutPanel.Children.Add(newDocpane);
            }
            docLayout.IsActive = true;
        }

        /// <summary>
        /// タイトルを設定します。
        /// </summary>
        /// <typeparam name="T">対象画面のクラス</typeparam>
        /// <param name="contentId">ID</param>
        /// <param name="title">設定したいタイトル</param>
        private void SetTitile<T>(string contentId, string title) where T : UserControl
        {
            var layout = MainWindow.LayoutPanel.Descendents().OfType<LayoutDocument>().FirstOrDefault(x => x.ContentId == contentId && x.Content.GetType() == typeof(T)); ;
            if (layout != null)
            {
                layout.Title = title;
                return;
            }
            //存在しない場合、フローティングウィンドウも探す
            var floatingWindows = GetFloatingWindows();
            foreach (var window in floatingWindows)
            {
                layout = window.Descendents().OfType<LayoutDocument>().FirstOrDefault(x => x.ContentId == contentId && x.Content.GetType() == typeof(T)); ;
                if (layout != null)
                    layout.Title = title;
            }
        }

        private List<LayoutFloatingWindow> GetFloatingWindows()
        {
            var floatingWindows = MainWindow.DockingManager.Layout.Descendents().OfType<LayoutFloatingWindow>().ToList();
            return floatingWindows;
        }
    }
}
