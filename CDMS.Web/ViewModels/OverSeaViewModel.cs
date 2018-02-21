using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDMS.Web.ViewModels
{
    public class OverSeaBatchCreateViewModel
    {
        public OverSeaBatchCreateMasterViewModel Master { get; set; }
        public List<OverSeaBatchCreateDetailViewModel> Detail { get; set; }
    }

    public class OverSeaBatchCreateMasterViewModel
    {
        public int ID_OverType { get; set; }
        public int ID_Country { get; set; }

        public string CX_From_Date { get; set; }

        public string CX_To_Date { get; set; }

        public string CX_OverSea_Remark { get; set; }

        public string CX_Place_Remark { get; set; }

    }

    public class OverSeaBatchCreateDetailViewModel
    {
        public string CX_PID { get; set; }


        public bool FG_IsWorkCard { get; set; }

    }

    public class NowOverSeaViewModel
    {

        public string CX_Country { get; set; }
        public string CX_Group_Name { get; set; }

        public int NQ_Sub_People { get; set; }

        public int NQ_Total_People { get; set; }

        public string CX_Remark { get; set; }
    }

    public class DisplayExportViewModel
    {
        public int 序號 { get; set; }

        public string 工號 { get; set; }

        public string 姓名 { get; set; }

        public string 出差國家 { get; set; }

        public string 出發日期 { get; set; }

        public string 返台日期 { get; set; }

        public string  種類 { get; set; }

        public string 單位 { get; set; }

        public string 職稱 { get; set; }

        public string 到職日 { get; set; }

        public string  工作簽 { get; set; }

        public string 狀態 { get; set; }

    }
}