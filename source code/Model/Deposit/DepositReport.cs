
namespace EzPos.Model.Deposit
{
    /// <summary>
    /// Summary description for SalesOrder.
    /// </summary>
    public class DepositReport
    {
        public const string ConstDepositId = "DepositId";
        public const string ConstDepositNumber = "DepositNumber";
        public const string ConstDepositDate = "DepositDate";

        public int DepositId { get; set; }

        public string DepositNumber { get; set; }

        public object DepositDate { get; set; }

        public string CustomerName { get; set; }

        public string CashierName { get; set; }

        public float ExchangeRate { get; set; }

        public float AmountSoldInt { get; set; }

        public float AmountPaidInt { get; set; }

        public float AmountPaidRiel { get; set; }

        public float AmountReturnInt { get; set; }

        public float AmountReturnRiel { get; set; }

        public float TotalDiscount { get; set; }

        public string CardNumber { get; set; }

        public string ReferenceNum { get; set; }

        public object UpdateDate { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public float UnitPriceIn { get; set; }

        public float UnitPriceOut { get; set; }

        public float Discount { get; set; }

        public int QtySold { get; set; }

        public float SubTotal { get; set; }        
        
        public int DepositItemId { get; set; }

        public int ReportHeader { get; set; }
    }
}