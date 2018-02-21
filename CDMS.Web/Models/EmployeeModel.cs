using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CDMS.Web
{
    public class EmployeeModel
    {
        public string eID { get; set; }//工號
        public string eName { get; set; }//姓名
        public string DeptID { get; set; }//部門代號
        public string DeptName { get; set; }//部門名稱
        public string ParentDeptName { get; set; }//父部門名稱
        public string PosStore { get; set; }//父部門ID        
        public string Title { get; set; }//職稱代號
        public string TitleName { get; set; }//職稱
        public string EducationLevel { get; set; }//學歷
        public string Education { get; set; }//學校名稱
        public string EducationDepartment { get; set; }//科系
        public string Experience1 { get; set; }//工作經驗1
        public string Experience2 { get; set; }//工作經驗2
        public string Experience3 { get; set; }//工作經驗3
        public string Experience4 { get; set; }//工作經驗4
        public DateTime OnBoardDate { get; set; }//到職日可能因復職
        public DateTime OriginalOnBoardDate { get; set; }//原始到職日
        public DateTime LeaveDate { get; set; }//離職日
        public string workstatusid { get; set; }//0在職 1離職 2留停
        public string OnBoard { get; set; }//職日格式化後
        public string OriginalOnBoard { get; set; }//原始到到職日格式化後

        public string eName2 { get; set; }

        public int EmpType { get; set; }

        public string Gender { get; set; }

        public DateTime Birthday { get; set; }

        public string OBirthday { get; set; }//格式化後到職日

        /// <summary>
        /// 查員工資料
        /// </summary>
        /// <param name="eID">工號</param>  
        /// <param name="pPOSStore">店ID 0001</param>  
        /// <param name="pWorkStatusID">0在職 1離職 2留停</param>  
        /// <returns>集合</returns>
        public List<EmployeeModel> GetEmpDataByStaffID(string eID, string pPOSStore, string pWorkStatusID)
        {
            List<EmployeeModel> data = new List<EmployeeModel>();

           // using (SqlConnection conn = new SqlConnection(ConnClass.getEmployeesConn()))
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["EmployeesConnectionString"].ToString()))
            {
                string Sql = @"SELECT [eID],[eName],[DeptID],DeptName,ParentDeptName
			,PosStore = s.StoreNo
			,[Title],[TitleName]
			,[EducationLevel],[Education],[EducationDepartment]      
			,[Experience1],[Experience2]
			,[Experience3],[Experience4]     
			,[OnBoardDate],[OriginalOnBoardDate],[LeaveDate] 
			,[workstatusid]  
            ,[EmpType]
            ,[Gender]
            ,[Birthday] 
		FROM
		(SELECT [eID],[eName]      
			,emp.[DeptID],dept.DeptName
			,StoreID = 
			CASE 
			WHEN Left(dept.DeptID, 1) < 4 THEN Left(dept.DeptID, 1)+'000'
			ELSE dept.ParentDept
			END	
			,ParentDeptName = Parentdept.DeptName			
			,emp.[Title],[TitleName]
			,[EducationLevel],[Education],[EducationDepartment]      
			,[Experience1],[Experience2]
			,[Experience3],[Experience4]     
			,[OnBoardDate],[OriginalOnBoardDate],[LeaveDate] 
			,[workstatusid]     
            ,[EmpType]
            ,[Gender] 
            ,[Birthday] 
		FROM [HumanWeb].[dbo].[Employee] emp  
		Left Join [HumanWeb].[dbo].[Title] t on emp.title = t.TitleID
		Left Join [HumanWeb].[dbo].[vw_aDepartment] dept on emp.DeptID = dept.DeptId
		Left Join [HumanWeb].[dbo].[vw_aDepartment] Parentdept on dept.ParentDept = Parentdept.DeptId
		--Left Join [HumanWeb].[dbo].[StoreDisplay] s on  dept.ParentDept = s.StoreID
	  ) as Staff
	  Left Join [HumanWeb].[dbo].[StoreDisplay] s on  Staff.StoreID = s.StoreID	  
        	  where 1 = 1  ";

                if (!eID.Trim().Equals(""))
                    Sql += " and eID = @eID ";

                if (!pPOSStore.Trim().Equals("0"))
                    Sql += " and s.StoreNo=@POSStore ";

                if (!pWorkStatusID.Trim().Equals("0"))
                    Sql += "  and WorkStatusID = @WorkStatusID ";


                SqlCommand cmd = new SqlCommand(Sql, conn);
                cmd.Parameters.AddWithValue("@eID", eID);
                //cmd.Parameters.Add("@eID ", SqlDbType.VarChar).Value = "%" + eID + "%";
                cmd.Parameters.AddWithValue("@POSStore", pPOSStore);
                cmd.Parameters.AddWithValue("@WorkStatusID", pWorkStatusID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                try
                {
                    da.Fill(dt);
                    data = DataTableAndListClass.DataTableToList<EmployeeModel>(dt);
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                finally
                {
                    conn.Close();
                    cmd.Dispose();
                    da.Dispose();
                }            
            }
            foreach (var item in data)
            {
              if( item.OriginalOnBoardDate !=null)
                item.OnBoard=item.OriginalOnBoardDate.ToString("yyyy-MM-dd");

              if (item.Birthday != null)
                  item.OBirthday = item.Birthday.ToString("yyyy-MM-dd");
            }


            return data;
        }

//        /// <summary>
//        /// 抓目前該部門人員
//        /// </summary>
//        /// <param name="pPOSStore">店ID 0001</param>  
//        /// <param name="pWorkStatusID">0在職 1離職 2留停</param>  
//        /// <param name="pDeptID">部門代號412</param>
//        /// <returns>集合</returns>
//        public List<EmployeeModel> GetEmployee(string pPOSStore, string pWorkStatusID, string pDeptID)
//        {
//            List<EmployeeModel> data = new List<EmployeeModel>();

//            //using (SqlConnection conn = new SqlConnection(ConnClass.getEmployeesConn()))
//            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["EmployeesConnectionString"].ToString()))
//            {
//                string Sql = @"SELECT [eID],[eName],Staff.[DeptID],DeptName,ParentDeptName              
//              ,PosStore
//              ,[Title],[TitleName]
//              ,[EducationLevel],[Education],[EducationDepartment]      
//              ,[Experience1],[Experience2] 
//              ,[Experience3],[Experience4]     
//              ,[OnBoardDate],[OriginalOnBoardDate],[LeaveDate] 
//              ,[workstatusid]  
//              ,[EmpType]
//              ,[Gender]
//              ,[Birthday]  
//         FROM
//         (SELECT [eID],[eName]      
//              ,emp.[DeptID],dept.DeptName
//              ,StoreID = 
//              CASE 
//              WHEN Left(dept.DeptID, 1) < 4 THEN Left(dept.DeptID, 1)+'000'
//              ELSE dept.ParentDept
//              END  
//              ,ParentDeptName = Parentdept.DeptName             
//              ,emp.[Title],[TitleName]
//              ,[EducationLevel],[Education],[EducationDepartment]      
//              ,[Experience1],[Experience2]
//              ,[Experience3],[Experience4]     
//              ,[OnBoardDate],[OriginalOnBoardDate],[LeaveDate] 
//              ,[workstatusid]    
//              ,[EmpType] 
//              ,[Gender]
//              ,[Birthday]  
//         FROM [Employee] emp  
//         Left Join [Title] t on emp.title = t.TitleID
//         Left Join [vw_aDepartment] dept on emp.DeptID = dept.DeptId
//         Left Join [vw_aDepartment] Parentdept on dept.ParentDept = Parentdept.DeptId         
//       ) as Staff
//       Left Join [vw_StoreDept] s on  Staff.DeptID = s.DeptID
//            where 1 = 1
// ";

//                if (!pPOSStore.Trim().Equals(""))
//                    Sql += " and s.PosStore=@POSStore ";

//                if (!pWorkStatusID.Trim().Equals(""))
//                    Sql += "  and WorkStatusID = @WorkStatusID ";

//                if (!pDeptID.Trim().Equals("") && !pDeptID.Trim().Equals("0"))
//                    Sql += " and  Staff.[DeptID] = @DeptID ";

//                SqlCommand cmd = new SqlCommand(Sql, conn);
//                cmd.Parameters.AddWithValue("@POSStore", pPOSStore);
//                cmd.Parameters.AddWithValue("@WorkStatusID", pWorkStatusID);
//                cmd.Parameters.AddWithValue("@DeptID", pDeptID);

//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                try
//                {
//                    da.Fill(dt);
//                    data = DataTableAndListClass.DataTableToList<EmployeeModel>(dt);

//                    foreach (var item in data)
//                    {
//                        //if (eID == "8849")
//                        //{
//                        //    string abc = "";

//                        //}
//                        if (string.IsNullOrEmpty(item.ParentDeptName))
//                            item.ParentDeptName = item.DeptName;

//                        item.eID = item.eID.Trim();
//                    }
//                }
//                catch (Exception ex)
//                {
//                    //ErrorClass.Write("sql=" + Sql + "-" + ex.Message.ToString());
//                }
//                finally
//                {
//                    conn.Close();
//                    cmd.Dispose();
//                    da.Dispose();
//                }
//            }
//            return data;
//        }

        //public List<EmployeeModel> GetDept(string pPOSStore)
        //{
        //    List<EmployeeModel> data = new List<EmployeeModel>();

        //    var deptlist = this.GetEmployee(pPOSStore, "", "");

        //    var query = (from u in deptlist
        //                 select new
        //                 {
        //                     DeptID = u.DeptID,
        //                     DeptName = u.DeptName
        //                 }).Distinct().OrderBy(x => x.DeptID);

        //    foreach (var item in query)
        //    {
        //        EmployeeModel CreateRow = new EmployeeModel()
        //        {
        //            DeptID = item.DeptID,
        //            DeptName = item.DeptName
        //        };
        //        data.Add(CreateRow);
        //    }

        //    return data;
        //}
    }
}