using BitopiApprovalSystemWebApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Data.SqlClient;
using System.Data;

namespace BitopiDBContext
{
    public class DBApproval : DBContext
    {


        public DBApproval()
        {

        }
        public List<ApprovalModel> GetApprovalList(string userid)
        {

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@userid",userid),


            };
            DataTable dt = ExecuteDataTable("bimob.dbo.sp_ApprovalList_mobile", param);

            ApprovalModel user = null;
            List<ApprovalModel> approvalList = new List<ApprovalModel>();
            foreach (DataRow row in dt.Rows)
            {
                user = new ApprovalModel
                {
                    Approval = row.Field<ApprovalType>("ApprovalType"),
                    ApprovalName = row.Field<string>("ApprovalName"),
                    RoleType = row.Field<ApprovalRoleType>("ApprovalRole"),
                    Count = row.Field<int>("TotalPendingApproval"),
                    DataVisualType = row.Field<string>("DataVisualType"),
                };
                approvalList.Add(user);
            }
            return approvalList;
        }

        public int ReceiveNotification(string userid, string deviceId, string approval, string requisitionid)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@RequisitionID",requisitionid),
                new SqlParameter("@Approval",approval),
                new SqlParameter("@UserId",userid),
                new SqlParameter("@DeviceID",deviceId)
            };
            int result = ExecuteNoResult("SystemManager..sp_receiveNotification", param);
            return result;
        }
        public List<Message> GetNotification(string userId, string DeviceID)
        {

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@UserID",userId),
            new SqlParameter("@DeviceID",DeviceID)};
            DataTable dt = ExecuteDataTable("SystemManager..sp_POMobileNotification", param);

            List<Message> messages = new List<Message>();
            foreach (DataRow dr in dt.Rows)
            {
                Message message = new Message();
                message.POID = dr["POID"].ToString();
                message.DeviceID = dr["DeviceID"].ToString();
                message.DeviceToken = dr["DeviceToken"].ToString();
                message.Approval = dr["Approval"].ToString();
                message.ApprovalName = dr["ApprovalName"].ToString();
                message.ApprovalType = dr["ApprovalType"].ToString();
                messages.Add(message);
                //if (!String.IsNullOrEmpty(message.DeviceID))
                //{
                //    if (NotificationSingleton.Instance.RecieveMessage.ContainsKey(message.DeviceID))
                //    {
                //        if (!NotificationSingleton.Instance.RecieveMessage[message.DeviceID].Contains(message.POID))
                //        {
                //            messages.Add(message);
                //            //SendNotification2(message.DeviceToken, JsonConvert.SerializeObject(message));
                //            NotificationSingleton.Instance.RecieveMessage[message.DeviceID].Add(message.POID);
                //        }
                //    }
                //    else
                //    {
                //        messages.Add(message);
                //        //SendNotification2(message.DeviceToken, JsonConvert.SerializeObject(message));
                //        NotificationSingleton.Instance.RecieveMessage.Add(message.DeviceID, new List<string>() { message.POID });
                //    }
                //}

            }



            return messages;
        }
        public List<ApprovalDetailsModel> POApprovalDetails(string UserId, ApprovalRoleType roleType, ApprovalType approvalType)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@userid",UserId),
            new SqlParameter("@ApprovalRolType",(int)roleType),
             new SqlParameter("@ApprovalType",(int)approvalType)};

            DataTable dt = ExecuteDataTable("BiMob.dbo.sp_POapprovalDetails_mobile", param);

            List<ApprovalDetailsModel> approvalList = new List<ApprovalDetailsModel>();

            ApprovalDetailsModel user = null;
            foreach (DataRow row in dt.Rows)
            {
                string jsonValue = row.Field<string>("JsonValue");
                List<ApprovalDataModel> _approvalDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ApprovalDataModel>>(jsonValue);
                user = new ApprovalDetailsModel
                {
                    ApprovalDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ApprovalDataModel>>(row.Field<string>("JsonValue")),
                    POID = row.Field<string>("POID"),
                    isApproved = false

                };
                approvalList.Add(user);
            }
            return approvalList;

        }

        public int SavePOCheck(string @POID, string @CheckedBy, string @POStatus)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@POID",@POID),
                new SqlParameter("@CheckedBy",@CheckedBy),
                new SqlParameter("@POStatus",@POStatus)
            };
            int result = ExecuteNoResult("BitopiSplint.dbo.sp_POChecked", param);
            return result;
        }
        public int SavePOApprove(string @POID, string @CheckedBy, string @POStatus)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@POID",@POID),
                new SqlParameter("@ApprovedBy",@CheckedBy),
                new SqlParameter("@POStatus",@POStatus)
            };
            int result = ExecuteNoResult("BitopiSplint.dbo.sp_POApproved", param);
            return result;
        }
        public int Set_Cash_Requestion_Approve_Reject(string @POID, string @CheckedBy, string @POStatus, string ApprovedBy)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@RequisitionID",@POID),
                new SqlParameter("@ApproveID",@CheckedBy),
                new SqlParameter("@RequsitionSatatus",@POStatus),
                new SqlParameter("@ApprovedBy",ApprovedBy),
                new SqlParameter("@Remarks",""),

            };
            int result = ExecuteNoResult("BiMob.dbo.Sp_Set_Cash_Requestion_Approve_Reject", param);
            return result;
        }

        public int ProcessApproval(string ApprovalID, string ApproveByID, string ApprovedBy, string ApprovaStatus, ApprovalType approvalType,
           ApprovalRoleType approvalRoleType, string Remarks)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@ApprovalID",ApprovalID),
                new SqlParameter("@ApproveByID",ApproveByID),
                new SqlParameter("@ApprovedBy",ApprovedBy),
                new SqlParameter("@ApprovaStatus",ApprovaStatus),
                new SqlParameter("@ApprovalType",(int)approvalType),
                new SqlParameter("@ApprovalRoleType",(int)approvalRoleType),
                new SqlParameter("@Remarks",Remarks)
            };
            int result = ExecuteNoResult("BiMob.dbo.sp_ProcessApproval_mobile", param);
            return result;
        }
    }
}
