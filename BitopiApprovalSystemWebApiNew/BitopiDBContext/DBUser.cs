
using BitopiApprovalSystemWebApiModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiDBContext
{
    public class DBUser : DBContext
    {

        public DBUser()
        {


        }

        public UserModel GetUser(string username, string password,
            string DeviceID,
            string DeviceToken,
            string DeviceName,
            string Platform,
            int QryOption, string UserCode,string VersionCode)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@UserName",username),
                new SqlParameter("@Password",password),
                new SqlParameter("@DeviceID",DeviceID),
                new SqlParameter("@DeviceToken",DeviceToken),
                new SqlParameter("@DeviceName",DeviceName),
                new SqlParameter("@Platform",Platform),
                 new SqlParameter("@UserCode",UserCode),
                new SqlParameter("@QryOption",QryOption),
                new SqlParameter("@VersionCode",VersionCode)
            };
            if (QryOption == 1)
            {
                DataTable dt = ExecuteDataTable("BIMOB_MVC..Sp_Get_LogInInfo_mobile2", param);

                UserModel user = null;
                foreach (DataRow row in dt.Rows)
                {
                    user = new UserModel
                    {
                        UserCode = row.Field<string>("UserCode"),
                        UserName = row.Field<string>("UserName"),
                        Department = row.Field<string>("Department"),
                        Designation = row.Field<string>("Designation"),
                        Email = row.Field<string>("Email"),
                        EmpImage = row.Field<byte[]>("EmpImage"),
                        EmployeeName = row.Field<string>("EmployeeName"),
                        VersionCode = row.Field<int>("VersionCode")
                    };
                }
                return user;
            }
            if (QryOption == 2)
            {
                int resutl = ExecuteNonQuery(CommandType.StoredProcedure, "BIMOB_MVC..Sp_Get_LogInInfo_mobile2", param);
            }
            return new UserModel { };
        }

    }
}
