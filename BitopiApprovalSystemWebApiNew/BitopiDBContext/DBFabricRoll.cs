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
    public class DBFabricRoll : DBContext
    {
        public List<string> GetGRNID()
        {

            List<string> _DBModelList = new List<string>();
            try
            {
                var param = new SqlParameter[] {
                new SqlParameter("@QryOption",1) };
                DataTable dt = ExecuteDataTable("Inventory.dbo.SP_ROLL_INFORMATION_MOB", param, CommandType.StoredProcedure);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        _DBModelList.Add(dr["GRNID"].ToString());
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
        public List<string> GetColor(string grnID)
        {

            List<string> _DBModelList = new List<string>();
            try
            {
                var param = new SqlParameter[] {
                new SqlParameter("@QryOption",2),
                new SqlParameter("@GRNId",grnID)};
                DataTable dt = ExecuteDataTable("Inventory.dbo.SP_ROLL_INFORMATION_MOB", param, CommandType.StoredProcedure);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        _DBModelList.Add(dr["ColorName"].ToString());
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
        public List<RollSettingsDBModel> GetRoll(string grnID, string colorNmae)
        {

            List<RollSettingsDBModel> _DBModelList = new List<RollSettingsDBModel>();
            try
            {
                var param = new SqlParameter[] {
                new SqlParameter("@QryOption",3),
                new SqlParameter("@GRNId",grnID),
                new SqlParameter("@ColorId",colorNmae)};
                DataTable dt = ExecuteDataTable("Inventory.dbo.SP_ROLL_INFORMATION_MOB", param, CommandType.StoredProcedure);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        RollSettingsDBModel model = new RollSettingsDBModel();
                        model.OwnWidth = dr["OwnWidth"] != DBNull.Value ? dr["OwnWidth"].ToString() : "";
                        model.SupplierRollNo = dr["SupplierRollNo"] != DBNull.Value ? dr["SupplierRollNo"].ToString() : "";
                        model.WidthBeforeWash = dr["SWidthBW"] != DBNull.Value ? dr["SWidthBW"].ToString() : "";
                        model.SWidthAW = dr["SWidthAW"] != DBNull.Value ? dr["SWidthAW"].ToString() : "";
                        model.RollSerial = dr["RollSerial"] != DBNull.Value ? dr["RollSerial"].ToString() : "";
                        model.SLengthBW = dr["SLengthBW"] != DBNull.Value ? dr["SLengthBW"].ToString() : "";
                        model.SLengthAW = dr["SLengthAW"] != DBNull.Value ? dr["SLengthAW"].ToString() : "";

                        model.SLengthAWPercent = dr["SLengthAWPercent"] != DBNull.Value ? dr["SLengthAWPercent"].ToString() : "";
                        model.SWidthAWParcent = dr["SWidthAWParcent"] != DBNull.Value ? dr["SWidthAWParcent"].ToString() : "";

                        model.RollID = dr["RollID"].ToString();
                        model.SerialNo = Convert.ToInt16(dr["SerialNo"].ToString());
                        model.OwnLength = dr["OwnLength"] != DBNull.Value ? Convert.ToDecimal(dr["OwnLength"].ToString()) : 0;
                        model.QCPass = dr["QCPass"] != DBNull.Value ? dr.Field<bool?>("QCPass") : null;
                        _DBModelList.Add(model);
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
        public RollSettingsDBModel GetRollID(string grnID, string colorNmae)
        {
            RollSettingsDBModel model = new RollSettingsDBModel();
            try
            {
                var param = new SqlParameter[] {
                new SqlParameter("@QryOption",4),
                new SqlParameter("@GRNId",grnID),
                new SqlParameter("@ColorId",colorNmae)};
                DataTable dt = ExecuteDataTable("Inventory.dbo.SP_ROLL_INFORMATION_MOB", param, CommandType.StoredProcedure);

                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow dr in dt.Rows)
                    {

                        model.RollSerial = dr["RollSerial"] != DBNull.Value ? dr["RollSerial"].ToString() : "";
                        model.RollID = dr["RollID"] != DBNull.Value ? dr["RollID"].ToString() : "";
                        model.OwnWidth = dr["OwnWidth"] != DBNull.Value ? dr["OwnWidth"].ToString() : "";
                        model.SupplierWidth = dr["SupplierWidth"] != DBNull.Value ? Convert.ToDecimal(dr["SupplierWidth"].ToString()) : 0;
                        model.SWidthBW = dr["SWidthBW"] != DBNull.Value ? dr["SWidthBW"].ToString() : "";
                        model.SLengthBW = dr["SLengthBW"] != DBNull.Value ? dr["SLengthBW"].ToString(): "";

                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                //ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
            finally
            {
                model = null;
            }
        }
        public List<UOM> GetUOM()
        {
            List<UOM> List = new List<UOM>();
            try
            {
                var param = new SqlParameter[] {
                new SqlParameter("@QryOption",7)};
                DataTable dt = ExecuteDataTable("Inventory.dbo.SP_ROLL_INFORMATION_MOB", param, CommandType.StoredProcedure);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        UOM model = new UOM();
                        model.Unitid = dr["Unitid"] != DBNull.Value ? dr["Unitid"].ToString() : "";
                        model.Unitname = dr["Unitname"] != DBNull.Value ? dr["Unitname"].ToString() : "";
                        List.Add(model);
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                //ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
            finally
            {
                List = null;
            }
        }
        public int SetRoll(RollSettingsDBModel model)
        {

          string xmlStr=UtilityOptions.GetXMLFromObject(model);

           SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@XmlStr",xmlStr),                
                new SqlParameter("@QryOption",5)
            };
            int result = ExecuteNoResult("Inventory.dbo.SP_ROLL_INFORMATION_MOB", param);
            return result;
        }
        public int SetRoll(List<RollSettingsDBModel> model)
        {

            string xmlStr = UtilityOptions.GetXMLFromObject(model);

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@XmlStr",xmlStr),
                new SqlParameter("@QryOption",6)
            };
            int result = ExecuteNoResult("Inventory.dbo.SP_ROLL_INFORMATION_MOB", param);
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
