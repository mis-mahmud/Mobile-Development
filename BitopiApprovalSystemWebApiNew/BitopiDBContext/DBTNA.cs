using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using BitopiApprovalSystemWebApiModels;
using System.Globalization;

namespace BitopiDBContext
{
    public class DBTNA : DBContext
    {
        public List<MyTaskDBModel> GetUnSeenTask(string UserCode, string TaskType)
        {
            SqlParameter[] param = null;

            if (TaskType == "UNSEEN")
                param = new SqlParameter[] {
                new SqlParameter("@QryOption",1),
                new SqlParameter("@DefaultAsignee",UserCode) };
            else if (TaskType == "SEEN")
                param = new SqlParameter[] {
                new SqlParameter("@QryOption",2),
                new SqlParameter("@DefaultAsignee",UserCode) };
            else if (TaskType == "COMPLETE")
                param = new SqlParameter[] {
                new SqlParameter("@QryOption",3),
                new SqlParameter("@DefaultAsignee",UserCode) };

            List<MyTaskDBModel> _DBModelList = new List<MyTaskDBModel>();
            try
            {
                DataTable dt = ExecuteDataTable("PlanningTNA.dbo.Sp_Get_MyTask_Details", param);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        MyTaskDBModel _DBModel = new MyTaskDBModel();
                        _DBModel.OrderRefNo = dr["OrderRefNo"].ToString();
                        _DBModel.Buyer = dr["Buyer"].ToString();
                        _DBModel.FileRefNo = dr["FileRefNo"].ToString();
                        _DBModel.BuyerStyleRef = dr["BuyerStyle"].ToString();
                        _DBModel.PoNo = dr["PoNo"].ToString();
                        _DBModel.ShipmentDate = dr["ShipmentDate"].ToString();
                        _DBModel.LotNo = dr["LotNo"].ToString();
                        _DBModel.Task = dr["TaskDescription"].ToString();
                        _DBModel.PlannedDate = dr["PlannedStartDate"].ToString();
                        if (TaskType == "UNSEEN")
                        {
                            _DBModel.CommittedDate = dr["PlannedStartDate"].ToString();
                        }
                        else
                        {
                            _DBModel.CommittedDate = dr["CommittedStartDate"].ToString();
                        }
                        _DBModel.Asignee = dr["Asignee"].ToString();
                        _DBModel.Seen = dr["IsSeen"].ToString();
                        _DBModel.Remarks = dr["Remarks"].ToString();
                        _DBModel.ChekList = dr["ChekList"].ToString();
                        _DBModel.OrderRefTaskTemplateChildId = Convert.ToInt32(dr["OrderRefTaskTemplateChildId"].ToString());
                        _DBModelList.Add(_DBModel);
                    }
                }
                return _DBModelList;
            }
            catch (Exception ex)
            {
                //ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
            finally
            {
                _DBModelList = null;
            }
        }
        
        public MyTaskCountDBModel GetTaskCount(string UserCode)
        {
            SqlParameter[] param = null;

           
                param = new SqlParameter[] {
                new SqlParameter("@QryOption",6),
                new SqlParameter("@DefaultAsignee",UserCode) };

            MyTaskCountDBModel _DBModel=null;
            try
            {
                DataTable dt = ExecuteDataTable("PlanningTNA.dbo.Sp_Get_MyTask_Details", param);
                if (dt.Rows.Count > 0)
                {
                    
                    foreach (DataRow dr in dt.Rows)
                    {
                        _DBModel = new MyTaskCountDBModel()
                        {
                            TotalCompleteTask=dr["TotalCompleteTask"].ToString(),
                            TotalSeenTask = dr["TotalSeenTask"].ToString(),
                            TotalUnSeenTask = dr["TotalUnSeenTask"].ToString()
                        };

                    }
                }
                return _DBModel;
            }
            catch (Exception ex)
            {
                //ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
            finally
            {
                _DBModel = null;
            }
        }
        public int SetTaskUnSeentoSeen(MyTaskDBModel _dbModel, string UserCode)
        {

            DateTime dtS = DateTime.ParseExact(_dbModel.CommittedDate.ToString(), "mm/dd/yyyy", CultureInfo.InvariantCulture);

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@OrderRefTaskTemplateChildId",_dbModel.OrderRefTaskTemplateChildId),
                new SqlParameter("@CommittedDate",dtS),
                new SqlParameter("@Remarks",_dbModel.Remarks),
                new SqlParameter("@SeenBy", UserCode),
                new SqlParameter("@QryOption",4),

            };
            int result = ExecuteNoResult("PlanningTNA.dbo.Sp_Get_MyTask_Details", param);
            return result;
        }
        public int SetTaskSeenToComplete(MyTaskDBModel _dbModel, string UserCode)
        {
            //DateTime dtS = DateTime.ParseExact(_dbModel.CommittedDate.ToString(), "mm/dd/yyyy", CultureInfo.InvariantCulture);

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@OrderRefTaskTemplateChildId",_dbModel.OrderRefTaskTemplateChildId),


                new SqlParameter("@SeenBy", UserCode),
                new SqlParameter("@QryOption",5),

            };
            int result = ExecuteNoResult("PlanningTNA.dbo.Sp_Get_MyTask_Details", param);
            return result;
        }




    }
}
