using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{
    /// <summary>
    /// 支票狀態
    /// </summary>
    public class CheckStatus : TextValue<CheckStatus>
    {       
        static List<CheckStatus> _Source = null;

        public CheckStatus(string value, string text) : base(value, text)
        {            
        }

        public static List<CheckStatus> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.CheckStatus>();
                _Source.Add(CheckStatus.Uncounted);
                _Source.Add(CheckStatus.Counted);
                _Source.Add(CheckStatus.Refund);
            }

            return _Source;
        }
             
        public static CheckStatus FromValue(string value)
        {
            if (value == "1")
                return Uncounted;
            if (value == "2")
                return Counted;
            if (value == "3")
                return Refund;
            return null;
        }

        // 未兌現、已兌現、退票

        public static CheckStatus Uncounted => new CheckStatus("1", "未兌現");
        public static CheckStatus Counted => new CheckStatus("2", "已兌現");
        public static CheckStatus Refund => new CheckStatus("3", "退票");
    }
}
