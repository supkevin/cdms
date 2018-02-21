using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CDMS.Web
{
    public class StoreDeptModel
    {
        public string StoreID { get; set; }

        public string PosStore { get; set; }

        public string StoreName { get; set; }

        public StoreDeptModel GetStoreDept(string pDeptID)
        {
            StoreDeptModel data = new StoreDeptModel();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["EventReportConnectionString"].ToString()))
            {
                string Sql = @"SELECT * from  StoreDept where 1 = 1 and DeptID = @DeptID";
                SqlCommand cmd = new SqlCommand(Sql, conn);
                cmd.Parameters.AddWithValue("@DeptID", pDeptID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                try
                {
                    da.Fill(dt);
                    List<StoreDeptModel> lst = DataTableAndListClass.DataTableToList<StoreDeptModel>(dt);

                    if (lst.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        foreach (var item in lst)
                            data = item;
                    }

                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    return null;
                    // ErrorClass.Write("sql=" + Sql + "-" + ex.Message.ToString());
                }
                finally
                {
                    conn.Close();
                    cmd.Dispose();
                    da.Dispose();
                }
            }
            return data;
        }

    }
}