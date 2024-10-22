using KAKEIBO.Item;
using KAKEIBO.Service;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Defaults;

namespace KAKEIBO.ViewModels
{
    public class PaymentViewerControlViewModel : BindableBase
    {
        private readonly IDataAccessor _dataAccessor;

        private ObservableCollection<PaymentViewerRecordViewModel> _paymentRecords;
        public ObservableCollection<PaymentViewerRecordViewModel> PaymentRecords
        {
            get => _paymentRecords;
            set => SetProperty(ref _paymentRecords, value);
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        private IEnumerable<ISeries> _series;
        public IEnumerable<ISeries> Series
        {
            get => _series;
            set => SetProperty(ref _series, value);
        }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }
        public DelegateCommand LoadPaymentCommand { get; }
        public DelegateCommand PreviousMonthCommand { get; }
        public DelegateCommand NextMonthCommand { get; }

        public PaymentViewerControlViewModel(IDataAccessor dataAccessor)
        {
            LiveChartsSkiaSharp.DefaultSKTypeface = SKFontManager.Default.MatchCharacter('あ');
            _dataAccessor = dataAccessor;
            LoadPaymentCommand = new DelegateCommand(async () => await LoadPaymentAsync());
            PreviousMonthCommand = new DelegateCommand(PreviousMonth);
            NextMonthCommand = new DelegateCommand(NextMonth);
            SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            UpdateChartData();
        }

        private void PreviousMonth()
        {
            SelectedDate = SelectedDate?.AddMonths(-1);
            LoadPaymentCommand.Execute();
        }

        private void NextMonth()
        {
            SelectedDate = SelectedDate?.AddMonths(1);
            LoadPaymentCommand.Execute();
        }

        private async Task LoadPaymentAsync()
        {
            if (SelectedDate.HasValue)
            {
                var year = SelectedDate.Value.Year;
                var month = SelectedDate.Value.Month;
                var paymentRecords = await _dataAccessor.GetPaymentRecordsAsync(year, month);
                PaymentRecords = new ObservableCollection<PaymentViewerRecordViewModel>(
                    paymentRecords.Select(r => new PaymentViewerRecordViewModel(r))
                );

                UpdateChartData();
            }
        }

        private void UpdateChartData()
        {
            if (PaymentRecords == null)
                return;
            var groupedData = PaymentRecords
                .GroupBy(r => r.PaymentRecord.Category)
                .Select(g => new { Category = g.Key, Amount = g.Sum(r => r.PaymentRecord.Amount) })
                .ToList();

            TotalAmount = groupedData.Sum(d => d.Amount);

            var targetData = groupedData.Select(x => new PieSeries<ObservableValue>
            {
                Values = new[] { new ObservableValue((double?)x.Amount) },
                Name = x.Category + $"：{(int)(x.Amount / TotalAmount * 100)} %",
                Fill = new SolidColorPaint(CategoryColors.GetColorForCategory(x.Category)),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
            }).ToArray();

            Series = targetData;
        }
    }

    public class PaymentViewerRecordViewModel : BindableBase
    {
        public PaymentRecord PaymentRecord { get; set; }
        public string FormattedAmount => PaymentRecord.Amount.ToString("C0", new CultureInfo("ja-JP"));

        public PaymentViewerRecordViewModel(PaymentRecord paymentRecord)
        {
            PaymentRecord = paymentRecord;
        }
    }
}