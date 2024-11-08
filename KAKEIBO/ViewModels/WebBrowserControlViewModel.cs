using CefSharp;
using CefSharp.Wpf;
using KAKEIBO.Item;
using KAKEIBO.Service;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace KAKEIBO.ViewModels
{
    public class WebBrowserControlViewModel : BindableBase
    {
        private readonly DataBaseAccessor _accessor;
        private readonly WindowAgent _window_agent;
        private string _currentUrl = "https://www.google.com";

        private string id = "Browser1";
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }
        public string CurrentUrl
        {
            get { return _currentUrl; }
            set { SetProperty(ref _currentUrl, value);
                DisplayUrl = CurrentUrl;
            }
        }

        private string _displayUrl;
        public string DisplayUrl
        {
            get { return _displayUrl; }
            set { SetProperty(ref _displayUrl, value); }
        }

        private string _pageTitle = "Home";
        public string PageTitle
        {
            get { return _pageTitle; }
            set { SetProperty(ref _pageTitle, value); }
        }

        private IWebBrowser _webBrowser;
        public IWebBrowser WebBrowser
        {
            get { return _webBrowser; }
            set { SetProperty(ref _webBrowser, value); }
        }

        private ObservableCollection<BookmarkViewModel> _bookmarks;

        public ObservableCollection<BookmarkViewModel> Bookmarks
        {
            get { return _bookmarks; }
            set { SetProperty(ref _bookmarks, value); }
        }

        public DelegateCommand BackCommand { get; private set; }
        public DelegateCommand ForwardCommand { get; private set; }
        public DelegateCommand ReloadCommand { get; private set; }
        public DelegateCommand NavigateCommand { get; private set; }
        public DelegateCommand<string> NavigateBookmarkCommand { get; private set; }

        public DelegateCommand AddBookmarkCommand { get; private set; }
        public DelegateCommand<int?> DeleteBookmarkCommand { get; private set; }

        public WebBrowserControlViewModel(DataBaseAccessor accessor,WindowAgent windowAgent)
        {
            _accessor = accessor;
            _window_agent = windowAgent;
            DisplayUrl = CurrentUrl;
            BackCommand = new DelegateCommand(Back);
            ForwardCommand = new DelegateCommand(Forward);
            ReloadCommand = new DelegateCommand(Reload);
            NavigateCommand = new DelegateCommand(Navigate);
            AddBookmarkCommand = new DelegateCommand(AddBookmark);
            DeleteBookmarkCommand = new DelegateCommand<int?>(DeleteBookmark);
            NavigateBookmarkCommand = new DelegateCommand<string>(Navigate);
            LoadBookmarks();
        }

        public void SetTitle(string title)
        {
            _window_agent.SetWebBrowserTitle(Id, title);
        }

        private async void Back()
        {
            if (WebBrowser.CanGoBack)
            {
                WebBrowser.Back();
                await WebBrowser.WaitForNavigationAsync();
                DisplayUrl = CurrentUrl;
            }
        }

        private async void Forward()
        {
            if (WebBrowser.CanGoForward)
            {
                WebBrowser.Forward();
                await WebBrowser.WaitForNavigationAsync();
                DisplayUrl = CurrentUrl;
            }
        }

        private void Reload()
        {
            WebBrowser.Reload();
        }

        private void Navigate()
        {

            CurrentUrl = DisplayUrl;
        }

        private void Navigate(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                CurrentUrl = url;
                DisplayUrl = url;
            }
        }

        private void LoadBookmarks()
        {
            var bookmarks = _accessor.GetBookmarks();
            Bookmarks = new ObservableCollection<BookmarkViewModel>(bookmarks.Select(r => new BookmarkViewModel(r)));
        }

        private void AddBookmark()
        {
            var bookmark = new Bookmark { DisplayName = PageTitle, Url = CurrentUrl };
            _accessor.AddBookmark(bookmark);
            LoadBookmarks(); // Reload bookmarks after adding
        }

        private void DeleteBookmark(int? id)
        {
            if (id != null)
            {
                _accessor.DeleteBookmark((int)id);
                LoadBookmarks(); // ブックマークを再読み込みして更新
            }
        }
    }

    public class BookmarkViewModel : BindableBase
    {
        private Bookmark _bookmark;
        public Bookmark Bookmark
        {
            get => _bookmark;
            set => SetProperty(ref _bookmark, value);
        }

        public BookmarkViewModel(Bookmark bookmark)
        {
            Bookmark = bookmark;
        }
    }
}
