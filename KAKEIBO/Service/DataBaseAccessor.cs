using KAKEIBO.Item;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace KAKEIBO.Service
{
    /// <summary>
    /// DataBaseにアクセスします。
    /// </summary>
    public class DataBaseAccessor:IDataAccessor
    {
        private const string DbName = "card_payment.db";
        private string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);

        public void AddAssetsRecords()
        {
            throw new NotImplementedException();
        }

        public void AddPaymentRecords(List<PaymentRecord> paymentRecords)
        {
            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SQLiteCommand(connection))
                        {
                            command.CommandText = @"
                                INSERT INTO Payments (Date, Amount, Description, Category)
                                VALUES (@Date, @Amount, @Description, @Category)";

                            foreach (var record in paymentRecords)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Date", record.Date.ToString("yyyy-MM-dd"));
                                command.Parameters.AddWithValue("@Amount", record.Amount);
                                command.Parameters.AddWithValue("@Description", record.Description);
                                command.Parameters.AddWithValue("@Category", record.Category);
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"支払い記録の追加中にエラーが発生しました: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public void CreateDatabase()
        {
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
                using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Payments (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Date TEXT NOT NULL,
                                Amount REAL NOT NULL,
                                Description TEXT,
                                Category TEXT
                            )";
                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("データベースが正常に作成されました。");
            }
            else
            {
                Console.WriteLine("データベースは既に存在します。");
            }
        }

        public void DeleteAssetsRecords()
        {
            throw new NotImplementedException();
        }

        public void DeletePaymentRecords(List<PaymentRecord> paymentRecords)
        {
            throw new NotImplementedException();
        }

        public void UpdateAssetsRecords()
        {
            throw new NotImplementedException();
        }

        public void UpdatePaymentRecords(List<PaymentRecord> paymentRecords)
        {
            throw new NotImplementedException();
        }
        public async Task<List<PaymentRecord>> GetPaymentRecordsAsync(int year, int month)
        {
            var paymentRecords = new List<PaymentRecord>();
            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        SELECT * FROM Payments
                        WHERE Date > @Year || '-' || substr('0' || @Month, -2) AND Date < @Year || '-' || substr('0' || @Month, -2) || '-' || 32
                        ORDER BY Date";
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@Month", month.ToString("D2"));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var date = DateTime.Parse(reader["Date"].ToString());
                            var amount = Convert.ToDecimal(reader["Amount"]);
                            var description = reader["Description"].ToString();
                            var category = reader["Category"].ToString();
                            paymentRecords.Add(new PaymentRecord
                            {
                                Date = date,
                                Amount = amount,
                                Description = description,
                                Category = category
                            });
                        }
                    }
                }
            }
            return paymentRecords;
        }
        public async Task AddPaymentRecordsAsync(List<PaymentRecord> records)
        {
            // 非同期でデータベースに保存する処理を実装
            await Task.Run(() =>
            {
                AddPaymentRecords(records);
            });
        }
    }
}
