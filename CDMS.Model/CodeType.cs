using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class CodeType : TextValue<CodeType>
    {       
        static List<CodeType> _Source = null;

        public CodeType(string value, string text) : base(value, text)
        {            
        }

        public static List<CodeType> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.CodeType>();
                _Source.Add(CodeType.NewsType);
                _Source.Add(CodeType.ProductKind);
                _Source.Add(CodeType.Unit);
                _Source.Add(CodeType.Department);
                _Source.Add(CodeType.Title);
                _Source.Add(CodeType.QuotationLevel);
                _Source.Add(CodeType.Currency);
                _Source.Add(CodeType.Condition);
            }

            return _Source;
        }
               
        public static CodeType NewsType => new CodeType("NEWS_TYPE", "消息類別");
        public static CodeType ProductKind => new CodeType("PRODUCT_KIND", "產品類別");
        public static CodeType Unit => new CodeType("UNIT", "單位");
        public static CodeType Department => new CodeType("DEPARTMENT", "部門");
        public static CodeType Title => new CodeType("TITLE", "職稱");
        public static CodeType QuotationLevel => new CodeType("QUOTATION_LEVEL", "報價授權等級");
        public static CodeType Currency => new CodeType("CURRENCY_KIND", "幣別");
        public static CodeType Condition => new CodeType("CONDITION_KIND", "條件");
        public static CodeType SalesRate => new CodeType("SALES_RATE", "管銷費用比率");
    }
}
