﻿using Prism.Mvvm;

namespace KAKEIBO.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public CsvImportControlViewModel CsvImportViewModel { get; }

        public MainWindowViewModel(CsvImportControlViewModel csvImportViewModel)
        {
            CsvImportViewModel = csvImportViewModel;
        }
    }
}
