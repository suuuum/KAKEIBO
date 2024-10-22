using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace KAKEIBO.Service
{
    public class CsvFormatter
    {
        public string FormatToCsv(string inputContent)
        {
            var lines = inputContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var csvLines = new List<string> { "date,description,amount" }; // ヘッダー行を追加

            foreach (var line in lines.Skip(1)) // ヘッダー行をスキップ
            {
                var columns = line.Split(',');
                if (columns.Length >= 3 && DateTime.TryParseExact(columns[0], "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    var date = columns[0];
                    var description = columns[1];
                    var amount = columns[2];
                    csvLines.Add($"{date},{description},{amount}");
                }
            }

            return string.Join(Environment.NewLine, csvLines);
        }

        public void SaveToCsvFile(string inputContent, string outputPath)
        {
            var csvContent = FormatToCsv(inputContent);
            File.WriteAllText(outputPath, csvContent, Encoding.UTF8);
            Console.WriteLine($"CSVファイルが正常に保存されました: {outputPath}");
        }
    }
}