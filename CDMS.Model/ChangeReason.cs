using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDMS.Model
{

    public class ChangeReason : TextValue<ChangeReason>
    {       
        static List<ChangeReason> _Source = null;

        public ChangeReason(string value, string text) : base(value, text)
        {            
        }

        public static List<ChangeReason> GetAll()
        {
            if (null == _Source)
            {
                _Source = new List<Model.ChangeReason>();
                _Source.Add(ChangeReason.TransWarehouse);
                _Source.Add(ChangeReason.InventoryIncrese);
                _Source.Add(ChangeReason.InventoryReduce);
                _Source.Add(ChangeReason.PickingOut);
                _Source.Add(ChangeReason.StorageIn);
            }

            return _Source;
        }
             
        public static ChangeReason FromValue(string value)
        {
            if (value == "1")
                return TransWarehouse;
            if (value == "2")
                return InventoryIncrese;
            if (value == "3")
                return InventoryReduce;
            if (value == "4")
                return InventoryReduce;
            if (value == "5")
                return InventoryReduce;
            return null;
        }
        
        public static ChangeReason TransWarehouse => new ChangeReason("1", "轉倉");
        public static ChangeReason InventoryIncrese => new ChangeReason("2", "盤點增加");
        public static ChangeReason InventoryReduce => new ChangeReason("3", "盤點減少");
        public static ChangeReason PickingOut => new ChangeReason("4", "產品變更-領料出庫");
        public static ChangeReason StorageIn => new ChangeReason("5", "產品變更-成品入庫");

    }
}
