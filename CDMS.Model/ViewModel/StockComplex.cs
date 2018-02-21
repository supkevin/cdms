using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class StockQueryViewModel
    {
        public long SeqNo { get; set; }                // 流水號
        public string ProductID { get; set; }          // 產品編號		
        public string ProductName { get; set; }        // 產品名稱
        public string KindID { get; set; }             // 產品類別代碼
        public string UnitName { get; set; }           // 單位
        public string ProductKindName { get; set; }    // 產品類別
        public decimal ListPrice { get; set; }         // 牌價
        public decimal SetPrice { get; set; }          // 定價
        public decimal RealPrice { get; set; }         // 實價
        public decimal BizCost { get; set; }           // 業務成本

        public int SafeStock { get; set; }             // 安全庫存量
        public int QtyWarehouse1 { get; set; }         // 倉庫-1
        public int QtyWarehouse2 { get; set; }         // 倉庫-2

        // 庫存量合計
        public int QtyTotal {                   
            get {
                return this.QtyWarehouse1 + this.QtyWarehouse2;
            }
        }

        // 不足量
        public int Shortage
        {
            get
            {
                // 庫存量足夠顯示0
                int temp = this.SafeStock - this.QtyTotal;
                return temp >= 0 ? 0 :  (-1) * temp;
            }
        }        
    }
}
