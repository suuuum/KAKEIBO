using KAKEIBO.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAKEIBO.Service
{
    public interface IDataAccessor
    {
        public void CreateDatabase();
        public void AddPaymentRecords(List<PaymentRecord> paymentRecords);
        public void DeletePaymentRecords(List<PaymentRecord> paymentRecords);
        public void UpdatePaymentRecords(List<PaymentRecord> paymentRecords);
        public Task<List<PaymentRecord>> GetPaymentRecordsAsync(int year, int month);
        public void AddAssetsRecords();
        public void DeleteAssetsRecords();
        public void UpdateAssetsRecords();
    }
}
