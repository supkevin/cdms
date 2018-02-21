using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace CDMS.Web
{
    public class NpoiClass
    {
        /// <summary>
        ///  將DataTable轉為 Stream輸出
        /// </summary>
        /// <param name="DataTable">DataTable</param> 
        public static Stream RenderDataTableToExcel(DataTable DataTable)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();
            HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);

            // handling header.
            foreach (DataColumn column in DataTable.Columns)
            {
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
            }
            // handling value.
            int rowIndex = 1;

            foreach (DataRow row in DataTable.Rows)
            {
                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);

                foreach (DataColumn column in DataTable.Columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }
                rowIndex++;
            }

            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            stream.Flush();
            stream.Position = 0;

            sheet = null;
            headerRow = null;
            workbook = null;

            return stream;
        }

        // List<Login> data = DataTableAndListExtensions.DataTableToList<Login>(dt);       
        public static Stream RenderListToExcel<T>(IEnumerable<T> ListValue) where T : class, new()
        {
            //建Excel內容
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();
            HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
            PropertyInfo[] PI_List = null;

            int i = 0;
            foreach (var item in ListValue)
            {
                //判斷 DataTable 是否已經定義欄位名稱與型態
                //只會做第一次
                if (i == 0)
                {
                    //取得 Type 所有的共用屬性
                    PI_List = item.GetType().GetProperties();

                    //將 List 中的 名稱 與 型別，定義 DataTable 中的欄位 名稱 與 型別
                    int ii = 0;//第幾行
                    foreach (var item1 in PI_List)
                    {
                        headerRow.CreateCell(ii).SetCellValue(item1.Name);
                        ii++;
                    }
                }
                i++;
            }

            int rowIndex = 1;
            foreach (var item in ListValue)
            {
                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
                int ii = 0;//第幾行
                foreach (var column in PI_List)
                {
                    dataRow.CreateCell(ii).SetCellValue(column.GetValue(item, null) == null ? "" : column.GetValue(item, null).ToString());
                    ii++;
                }
                rowIndex++;
            }



            MemoryStream stream = new MemoryStream();
            workbook.Write(stream);
            stream.Flush();
            stream.Position = 0;

            sheet = null;
            headerRow = null;
            workbook = null;

            return stream;
        }
    }
}