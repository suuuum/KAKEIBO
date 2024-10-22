using KAKEIBO.Item;
using KAKEIBO.Service;
using KAKEIBO.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace KAKEIBO.ViewModels
{
    public class CsvImportControlViewModel : BindableBase
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly CsvImporter _csvImporter;
        private readonly DataBaseAccessor _accessor;
        private readonly Service.DialogService _dialogService;

        private ObservableCollection<PaymentRecordViewModel> _paymentRecords;
        public ObservableCollection<PaymentRecordViewModel> PaymentRecords
        {
            get => _paymentRecords;
            set => SetProperty(ref _paymentRecords, value);
        }

        private string _selectedFilePath;
        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set => SetProperty(ref _selectedFilePath, value);
        }

        private bool _isAllSelected;
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                SetProperty(ref _isAllSelected, value);
                UpdateAllSelections();
            }
        }

        private void UpdateAllSelections()
        {
            if (PaymentRecords != null)
            {
                foreach (var record in PaymentRecords)
                {
                    record.IsSelected = IsAllSelected;
                }
            }
        }

        public DelegateCommand SelectFileCommand { get; }
        public DelegateCommand SavePaymentCommand { get; set; }

        public CsvImportControlViewModel(IFileDialogService fileDialogService, DataBaseAccessor dataBaseAccessor, CsvImporter csvImporter, IDialogService dialogService)
        {
            _accessor = dataBaseAccessor;
            _csvImporter = csvImporter;
            _fileDialogService = fileDialogService;
            _dialogService = (Service.DialogService)dialogService;
            SavePaymentCommand = new DelegateCommand(async () => await SavePaymentAsync());
            SelectFileCommand = new DelegateCommand(async () => await SelectFileAsync());
        }

        private async Task SavePaymentAsync()
        {
            try
            {
                var paymentRecords = PaymentRecords.Where(pr => pr.IsSelected).Select(pr => pr.PaymentRecord).ToList();
                await _accessor.AddPaymentRecordsAsync(paymentRecords);
                await _dialogService.ShowDialogAsync("保存完了", "選択されたレコードの保存が完了しました。");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowDialogAsync("エラー", $"保存中にエラーが発生しました: {ex.Message}");
            }
        }
        private async Task SelectFileAsync()
        {
            var filePath = await _fileDialogService.OpenFileDialogAsync();
            if (!string.IsNullOrEmpty(filePath))
            {
                SelectedFilePath = filePath;
                var records = _csvImporter.ImportCsv(filePath);
                PaymentRecords = new ObservableCollection<PaymentRecordViewModel>(
                records.Select(r => new PaymentRecordViewModel(r))
        );
            }
        }

    }

    public class PaymentRecordViewModel : BindableBase
    {
        private PaymentRecord _paymentRecord;
        public PaymentRecord PaymentRecord
        {
            get => _paymentRecord;
            set => SetProperty(ref _paymentRecord, value);
        }

        public string FormattedAmount
        {
            get => PaymentRecord.Amount.ToString("C0", new CultureInfo("ja-JP"));
            set
            {
                if (decimal.TryParse(value, NumberStyles.Currency, new CultureInfo("ja-JP"), out decimal amount))
                {
                    PaymentRecord.Amount = amount;
                    RaisePropertyChanged(nameof(FormattedAmount));
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public List<string> Categories { get; } = new List<string>
    {
        "食費", "積み立て", "サブスク", "日用品", "雑費", "交際費", "交通費", "通信費", "光熱費", "娯楽", "その他"
    };

        public string SelectedCategory
        {
            get => PaymentRecord.Category;
            set
            {
                PaymentRecord.Category = value;
                RaisePropertyChanged();
            }
        }

        public PaymentRecordViewModel(PaymentRecord paymentRecord)
        {
            PaymentRecord = paymentRecord;
        }
    }
}
