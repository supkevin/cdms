using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class ResultType : TextValue<ResultType>
    {       
        static List<ResultType> _Source = null;

        public ResultType(string value, string text) : base(value, text)
        {            
        }

        public static List<ResultType> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.ResultType>();
                _Source.Add(ResultType.Approve);
                _Source.Add(ResultType.Oppose);
                _Source.Add(ResultType.Reject);
            }

            return _Source;
        }
             
        public static ResultType FromValue(string value)
        {
            if (value == "1")
                return Approve;
            if (value == "2")
                return Oppose;
            if (value == "3")
                return Reject;
            return null;
        }

        public static ResultType Approve => new ResultType("1", "己核准");
        public static ResultType Oppose => new ResultType("2", "不同意");
        public static ResultType Reject => new ResultType("3", "退回");
    }
}
