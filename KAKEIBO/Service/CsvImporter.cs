using KAKEIBO.Item;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace KAKEIBO.Service
{
    public class CsvImporter
    {
        public List<PaymentRecord> ImportCsv(string filePath)
        {
            var records = new List<PaymentRecord>();

            try
            {
                // Shift-JISエンコーディングを指定して読み込み
                var encoding = Encoding.GetEncoding("shift_jis");
                var lines = File.ReadAllLines(filePath, encoding);
                var dataLines = lines.Skip(1); // ヘッダー行をスキップ

                foreach (var line in dataLines)
                {
                    var columns = line.Split(',');
                    if (columns.Length >= 3)
                    {
                        if (DateTime.TryParseExact(columns[0], "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) &&
                            decimal.TryParse(columns[2], out var amount))
                        {
                            records.Add(new PaymentRecord
                            {
                                Date = date,
                                Description = columns[1],
                                Amount = amount
                            });
                        }
                        else
                        {
                            Console.WriteLine($"無効な行をスキップしました: {line}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CSVファイルの読み込み中にエラーが発生しました: {ex.Message}");
            }

            return records;
        }

        public void DisplayImportedData(List<PaymentRecord> records)
        {
            Console.WriteLine("インポートされたデータ:");
            Console.WriteLine("日付\t\t名称\t\t金額");
            Console.WriteLine("----------------------------------------");

            foreach (var record in records)
            {
                Console.WriteLine($"{record.Date:yyyy/MM/dd}\t{record.Description}\t{record.Amount:C}");
            }

            Console.WriteLine($"合計レコード数: {records.Count}");
        }
    }
}