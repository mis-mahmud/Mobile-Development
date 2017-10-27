using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using BitopiApprovalSystemWebApiModels;
using System.Globalization;
using System.Configuration;
using System.Xml.Serialization;

namespace BitopiDBContext
{
    public class DBProduction : DBContext
    {
        public List<ProductionAccountingDBModel> Get(string UserCode, string ProcessID, string LocationID, string PRStatus, int EntryType, string RefID)
        {

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@UserCode",UserCode),
                new SqlParameter("@ProcessID",ProcessID),
                new SqlParameter("@LocationID",LocationID),
                new SqlParameter("@PRStatus",PRStatus),
                new SqlParameter("@EntryType",EntryType),
                new SqlParameter("@RefID",RefID)

            };
            string sql = toSqlString(param, "bimob.dbo.sp_productionAccounting");
            List<ProductionAccountingDBModel> _DBModelList = new List<ProductionAccountingDBModel>();
            try
            {
                DataTable dt = ExecuteDataTable("bimob.dbo.sp_productionAccounting", param);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ProductionAccountingDBModel _DBModel = new ProductionAccountingDBModel();
                        _DBModel.RefNo = dr["RefNO"].ToString();
                        _DBModel.LocationRef = dr["LocationRef"].ToString();
                        _DBModel.LocationName = dr["LocationName"].ToString();
                        _DBModel.Style = dr["Style"].ToString();
                        _DBModel.PR = dr["PR"].ToString();
                        _DBModel.EO = dr["EO"].ToString();
                        _DBModel.Buyer = dr["Buyer"].ToString();
                        _DBModel.DeliveryDate = dr["DeliveryDate"].ToString();
                        _DBModel.Color = dr["Color"].ToString();
                        _DBModel.BalanceQty = Convert.ToInt32(dr["BalanceQty"]);
                        _DBModel.OrderQty = Convert.ToInt32(dr["OrderQty"]);
                        _DBModel.ProducedQty = Convert.ToInt32(dr["ProducedQty"]);
                        _DBModel.WIP = Convert.ToInt32(dr["WIP"]);
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
        public int Set(ProductionAccountingDBModel model)
        {
            string xmlString = ConvertToXML(model.OperationList);
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Ref",model.RefNo),
                new SqlParameter("@ProdDateTime",model.ProdDateTime),
                new SqlParameter("@LocationRef",model.LocationRef),
                new SqlParameter("@Qty",model.ProducedQty),
                new SqlParameter("@xmlString",xmlString),
                new SqlParameter("@AddedBy",model.AddedBy),
            };

            try
            {
                string sql = toSqlString(param, "bimob.dbo.USP_Productionentry ");
                int count = ExecuteNonQuery(CommandType.StoredProcedure, "bimob.dbo.USP_Productionentry", param);

                return count;
            }
            catch (Exception ex)
            {
                //ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
        }
        public int SetRejection(ProductionRejectionDBModel model)
        {
            
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Ref",model.RefNo),
                new SqlParameter("@ProdDateTime",DateTime.Now),
                new SqlParameter("@LocationRef",model.LocationRef),
                new SqlParameter("@Grade",model.Grade),
                new SqlParameter("@SKUCode",model.SKUCode.ToString()),
                new SqlParameter("@SKUQuantiy",model.ProducedQty),
                
                new SqlParameter("@AddedBy",model.AddedBy)
            };

            try
            {
                int count = ExecuteNonQuery(CommandType.StoredProcedure, "bimob.dbo.USP_Rejectionentry", param);

                return count;
            }
            catch (Exception ex)
            {
                //ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
        }
        public int SetQuality(ProductionQualityDBModel model)
        {
            string xmlString = ConvertToXML(model.DefectList.Where(t=>t.No>0).ToList());
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Ref",model.RefNo),
                new SqlParameter("@ProdDateTime",DateTime.Now),
                new SqlParameter("@LocationRef",model.LocationRef),
                new SqlParameter("@LotQ",model.LotQ),
                new SqlParameter("@Sample",model.Sample),
                new SqlParameter("@Check",model.Check),
                new SqlParameter("@Status",model.QualityStatus),
                new SqlParameter("@DefectiveUnit",model.DefectiveUnit),
                new SqlParameter("@xmlString",xmlString),
                new SqlParameter("@AddedBy",model.AddedBy)
            };
            string sql = toSqlString(param, "bimob.dbo.USP_Qualityentry ");
            try
            {
                int count = ExecuteNonQuery(CommandType.StoredProcedure, "bimob.dbo.USP_Qualityentry", param);

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
        public string GetAQL(string RefID, int LotQ, int? DefectUnit = null)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@RefID",RefID),
                new SqlParameter("@LotQ",LotQ),
                new SqlParameter("@DefectUnit",DefectUnit)
            };
            List<DDL> _DBModelList = new List<DDL>();
            string result="";
            try
            {
                DataTable dt = ExecuteDataTable("bimob.dbo.sp_aql", param);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if(DefectUnit==null)
                        {
                            result = dr["Sample"].ToString();
                        }
                        else
                        {
                            result = dr["Status"].ToString();
                        }
                    }
                }
                return result;
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
        public List<DefectMaster> GetDefectList()
        {
            using (SqlConnection f_conn = new SqlConnection(ConfigurationManager.ConnectionStrings["sqlConnStr"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from bimob..Defect_Master", f_conn);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<DefectMaster> List = (from DataRow dr in dt.Rows
                                           select new DefectMaster
                                           {
                                               DefectCode = dr.Field<string>("DefectCode"),
                                               DefectName = dr.Field<string>("DefectName"),
                                               Category = dr.Field<string>("Category"),
                                               OperationCode = dr.Field<string>("OperationCode")
                                           }).ToList();
                return List;
            }
        }
        public List<Operation> GetOperationList(string RefNo)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@RefNO",RefNo)
            };
            List<Operation> _DBModelList = new List<Operation>();
            try
            {
                DataTable dt = ExecuteDataTable("bimob.dbo.sp_get_operation_code", param);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Operation _DBModel = new Operation();
                        _DBModel.OperationCode = dr["OperationCode"].ToString();
                        _DBModel.OperationName = dr["OperationName"].ToString();

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
        public string ConvertToXML(List<DefectMaster> list)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<DefectMaster>));

            var stringwriter = new System.IO.StringWriter();
            serializer.Serialize(stringwriter, list);
            return stringwriter.ToString();
        }
        public string ConvertToXML(List<Operation> list)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Operation>));

            var stringwriter = new System.IO.StringWriter();
            serializer.Serialize(stringwriter, list);
            return stringwriter.ToString();
        }
        public string  toSqlString(SqlParameter[] param,string sql)
        {
            
            foreach (var p in param)
            {
                sql += p.ParameterName + "='" + p.SqlValue + "',";
            }
            //sql = sql.Substring(sql.LastIndexOf(','), 1);
            return sql;
        }
    }
}
