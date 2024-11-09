using KAKEIBO.Service;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KAKEIBO.Item;

namespace KAKEIBO.ViewModels
{
    public class TrendControlViewModel : BindableBase
    {
        private readonly IDataAccessor _dataAccessor;
        private ISeries[] _series;
        public ISeries[] Series
        {
            get => _series;
            set => SetProperty(ref _series, value);
        }

        private Axis[] _xaxes;
        public Axis[] XAxes
        {
            get => _xaxes;
            set => SetProperty(ref _xaxes, value);
        }
        private Axis[] _yaxes;
        public Axis[] YAxes
        {
            get => _yaxes;
            set => SetProperty(ref _yaxes, value);
        }

        private string[] _monthLabels;
        public string[] MonthLabels
        {
            get => _monthLabels;
            set => SetProperty(ref _monthLabels, value);
        }

        public TrendControlViewModel(IDataAccessor dataAccessor)
        {
            _dataAccessor = dataAccessor;
            LoadDataCommand = new DelegateCommand(async () => await LoadDataAsync());
        }

        public DelegateCommand LoadDataCommand { get; }

        private async Task LoadDataAsync()
        {
            var now = DateTime.Now;
            var data = new List<(DateTime Date, decimal Amount, string Category)>();

            for (int i = 11; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                var records = await _dataAccessor.GetPaymentRecordsAsync(date.Year, date.Month);
                foreach (var record in records)
                {
                    data.Add((date, record.Amount, record.Category));
                }
            }

            var categories = data.Select(d => d.Category).Distinct().ToList();

            var seriesList = new List<ISeries>();

            foreach (var category in categories)
            {
                var seriesData = Enumerable.Range(0, 12)
                    .Select(month => data.Where(d => d.Date.Month == now.AddMonths(-11 + month).Month && d.Category == category)
                        .Sum(d => d.Amount))
                    .ToArray();

                seriesList.Add(new StackedColumnSeries<decimal>
                {
                    Name = category,
                    Values = seriesData,
                    Fill = new SolidColorPaint(CategoryColors.GetColorForCategory(category))
                });
            }

            Series = seriesList.ToArray();

            MonthLabels = Enumerable.Range(0, 12)
                .Select(i => now.AddMonths(-11 + i).ToString("yyyy/MM"))
                .ToArray();

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = MonthLabels,
                    LabelsRotation = 45
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => value.ToString("C0")
                }
            };
        }
    }
}
