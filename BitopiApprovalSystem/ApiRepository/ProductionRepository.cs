using BitopiApprovalSystem;
using BitopiApprovalSystem.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiRepository
{
    public class ProductionRepository
    {
        public  List<ProdcutionAccountingDBModel> GetProductionList(string userid, string ProcessID,string LocationID,string PRStatus, int EntryType,
            string RefID="")
        {
            userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "ProdcutionAccounting?UserID=" + userid + "&ProcessID=" + ProcessID + "&LocationID=" + LocationID + 
                "&PRStatus=" + PRStatus+ "&EntryType="+ EntryType+(RefID!=""? "&RefID="+RefID:"");

            HttpClient client = new HttpClient();
            HttpResponseMessage result =  client.GetAsync(url).Result;
            var aproves = JsonConvert.DeserializeObject<List<ProdcutionAccountingDBModel>>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new List<ProdcutionAccountingDBModel>();
            return aproves;
        }
        public async Task<List<DDL>> GetProductionDDL(string userid)
        {
            userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "ProdcutionAccounting?UserID=" + userid;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            var aproves = JsonConvert.DeserializeObject<List<DDL>>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new List<DDL>();
            return aproves;
        }
        public int SetProduction(string RefNO, int Qty, string LocationRef, string AddedBy)
        {
            
                string url = RepositorySettings.BaseURl + "ProdcutionAccounting?RefNO="
                    + RefNO + "&Qty=" + Qty + "&ProdDateTime=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm: ss.fff") + " &AddedBy=" + AddedBy
                    + "&LocationRef=" + LocationRef;

                HttpClient client = new HttpClient();
                HttpResponseMessage result =  client.GetAsync(url).Result;
                var aproves = JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);

                return aproves;
            
        }
        public int SetQuality(ProductionQualityDBModel model)
        {
            try
            {
                //model.RefNo = "Test Ref";
                string url = RepositorySettings.BaseURl + "Quality";

                HttpClient client = new HttpClient();
                HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
    "application/json");
                HttpResponseMessage result = client.PostAsync(url, contentPost).Result;
                var aproves = JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
           

            return aproves;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }

        }
        public int SetRejection(ProductionRejectionDBModel model)
        {
            try
            {
                //model.RefNo = "Test Ref";
            string url = RepositorySettings.BaseURl + "Rejection";

            HttpClient client = new HttpClient();
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
"application/json");
            HttpResponseMessage result = client.PostAsync(url, contentPost).Result;
            var aproves = JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
            return aproves;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public List<DefectMaster> GetGetDefectList()
        {
            
            string url = RepositorySettings.BaseURl + "ProdcutionAccounting/GetGetDefectList";

            HttpClient client = new HttpClient();
            HttpResponseMessage result =  client.GetAsync(url).Result;
            var aproves = JsonConvert.DeserializeObject<List<DefectMaster>>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new List<DefectMaster>();
            return aproves;
        }
        public string GetAQL(string RefID, int LotQ, int? DefectUnit = null)
        {

            string url = RepositorySettings.BaseURl + "ProdcutionAccounting?RefID=" + RefID + "&LotQ=" + LotQ + "&DefectUnit=" + DefectUnit;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = client.GetAsync(url).Result;
            var aproves = JsonConvert.DeserializeObject<string>(result.Content.ReadAsStringAsync().Result);
            return aproves;
        }
        public List<Operation> GetOperationList(string RefID)
        {

            string url = RepositorySettings.BaseURl + "Quality?RefID=" + RefID ;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = client.GetAsync(url).Result;
            var aproves = JsonConvert.DeserializeObject<List<Operation>>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new List<Operation>();
            return aproves;
        }
    }
}
