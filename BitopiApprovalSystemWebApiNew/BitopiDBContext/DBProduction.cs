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
    public class DBProduction : DBContext
    {
        public List<ProdcutionAccountingDBModel> Get(string UserCode, string ProcessID)
        {
       
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@UserCode",UserCode),
                new SqlParameter("@ProcessID",ProcessID)
            };
            List<ProdcutionAccountingDBModel> _DBModelList = new List<ProdcutionAccountingDBModel>();
            try
            {
                DataTable dt = ExecuteDataTable("bimob.dbo.sp_productionAccounting", param);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ProdcutionAccountingDBModel _DBModel = new ProdcutionAccountingDBModel();
                        _DBModel.RefNo = dr["RefNO"].ToString();
                        _DBModel.LocationRef = dr["LocationRef"].ToString();
                        _DBModel.LocationName = dr["LocationName"].ToString();
                        _DBModel.Style = dr["Style"].ToString();
                        _DBModel.PR = dr["PR"].ToString();
                        _DBModel.EO = dr["EO"].ToString();
                        _DBModel.Color = dr["Color"].ToString();
                        _DBModel.BalanceQty = Convert.ToInt32(dr["BalanceQty"]);
                        _DBModel.OrderQty = Convert.ToInt32(dr["OrderQty"]);
                        _DBModel.ProducedQty = Convert.ToInt32(dr["ProducedQty"]);
                        _DBModel.Size = dr["Size"].ToString();
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
        public int Set(string RefNO, DateTime ProdDateTime,int Qty,string AddedBy)
        {

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Ref",RefNO),
                new SqlParameter("@ProdDateTime",ProdDateTime),
                new SqlParameter("@Qty",Qty),
                new SqlParameter("@AddedBy",AddedBy),
            };
            
            try
            {
                int count = ExecuteNonQuery(CommandType.StoredProcedure,"bimob.dbo.USP_Productionentry", param);
                
                return count;
            }
            catch (Exception ex)
            {
                //ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
        }
        public List<DDL> GetDDL(string UserCode)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@UserCode",UserCode),
                new SqlParameter("@QryOption","2")
            };
            List<DDL> _DBModelList = new List<DDL>();
            try
            {
                DataTable dt = ExecuteDataTable("bimob.dbo.sp_productionAccounting", param);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DDL _DBModel = new DDL();
                        _DBModel.LocationRef = dr["LocationRef"].ToString();
                        _DBModel.LocationName = dr["LocationName"].ToString();
                        _DBModel.ProcessCode = dr["ProcessCode"].ToString();
                        _DBModel.ProcessName = dr["ProcessName"].ToString();
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
        
    }
}
