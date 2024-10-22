using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAKEIBO.Item
{
    public static class CategoryColors
    {
        public static readonly Dictionary<string, SKColor> Colors = new Dictionary<string, SKColor>
        {
            { "食費", SKColor.Parse("#FF4136") },
            { "積み立て", SKColor.Parse("#FF851B") },
            { "サブスク", SKColor.Parse("#FFDC00") },
            { "日用品", SKColor.Parse("#2ECC40") },
            { "雑費", SKColor.Parse("#0074D9") },
            { "交際費", SKColor.Parse("#B10DC9") },
            { "交通費", SKColor.Parse("#F012BE") },
            { "通信費", SKColor.Parse("#01FF70") },
            { "光熱費", SKColor.Parse("#7FDBFF") },
            { "娯楽", SKColor.Parse("#39CCCC") },
            { "その他", SKColor.Parse("#85144b") }
        };

        public static SKColor GetColorForCategory(string category)
        {
            return Colors.TryGetValue(category, out var color) ? color : SKColors.Gray;
        }
    }
}
