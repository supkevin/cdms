using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDMS.Model;
using System.ComponentModel.DataAnnotations;

namespace CDMS.Model.ViewModel
{
    public class ProductSalesRankingViewModel
    {
        public string ProductID { get; set; }           //產品編號
        public string ProductName { get; set; }         //產品名稱
        public string KindID { get; set; }              //產品類別
        public int TotalQty { get; set; }               //銷售量
        public decimal TotalAmount { get; set; }        //銷售總金額
        public decimal TotalSalesCost { get; set; }     //銷售總成本
        public decimal TotalProfit { get; set; }        //銷售總毛利
        public double GrossProfitMargin { get; set; }   //毛利率%
    }
}
