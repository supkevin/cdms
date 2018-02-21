using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class StockTrackViewModel
    {
        public string ProductID { get; set; }               //產品編號		
        public string ProductName { get; set; }             //產品名稱            
        public int Purchase { get; set; }                   //進貨數量
        public int Sales { get; set; }                      //銷貨數量
        public int Adjust { get; set; }                     //調整數量
        public int Transfer { get; set; }                   //調貨數量
        public int Stock { get; set; }            		    //庫存數量
        public DateTime? ChangeStartDate { get; set; }      //異動日期起
        public DateTime? ChangeFinishDate { get; set; }     //異動日期迄
    }

    public class StockTrackDetailViewModel
    {
        public string ChangeType { get; set; }          // 異動類別
        public string ChangeReason { get; set; }        // 異動原因
        public string ProductID { get; set; }           // 產品代碼
        public string KindID { get; set; }              // 產品類別
        public DateTime ChangeDate { get; set; }        // 異動日期
        public string SourceID { get; set; }            // 憑證單號
        public string CompanySupplier { get; set; }     // 客戶廠商代碼
        public string WarehouseID { get; set; }         // 倉庫代碼
        public int Qty { get; set; }                    // 數量
        public int StockQty { get; set; }               // 數量含正負        
        public decimal Price { get; set; }              // 售價
        public string ProductName { get; set; }         // 產品名稱
        public string CompanyName { get; set; }         // 公司名稱
        public string WarehouseName { get; set; }       // 產品名稱    

    }
}
