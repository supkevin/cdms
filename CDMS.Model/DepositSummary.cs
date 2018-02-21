using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{
    /// <summary>
    /// 票據摘要
    /// </summary>
    public class DepositSummary : TextValue<DepositSummary>
    {       
        static List<DepositSummary> _Source = null;

        public DepositSummary(string value, string text) : base(value, text)
        {            
        }

        public static List<DepositSummary> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.DepositSummary>();
                _Source.Add(DepositSummary.CashDeposit);
                _Source.Add(DepositSummary.CashWithdraw);
                _Source.Add(DepositSummary.BillCollection);
                _Source.Add(DepositSummary.TicketOpened);
            }

            return _Source;
        }
             
        public static DepositSummary FromValue(string value)
        {
            if (value == "1")
                return CashDeposit;
            if (value == "2")
                return CashWithdraw;
            if (value == "3")
                return BillCollection;
            if (value == "4")
                return TicketOpened;
            return null;
        }

        // 現金存入、現金提出、票據託收、票據開立

        public static DepositSummary CashDeposit => new DepositSummary("1", "現金存入");
        public static DepositSummary CashWithdraw => new DepositSummary("2", "現金提出");
        public static DepositSummary BillCollection => new DepositSummary("3", "票據託收");
        public static DepositSummary TicketOpened => new DepositSummary("4", "票據開立");
    }
}
