using BitopiApprovalSystem;
using BitopiApprovalSystem.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiRepository
{
    public class ProductionRepository
    {
        WebClient client;
        public ProductionRepository()
        {
            client = new WebClient();
        }
        public ProductionRepository(Action<long?, long, double?> ProgressChanged,Action ProgressComplete)
        {
            client = new WebClient();
            client.DownloadProgressChanged += (sender, e) => {
                int length = Convert.ToInt32(e.TotalBytesToReceive.ToString());
                int prog = Convert.ToInt32(e.BytesReceived.ToString());
                int perc = Convert.ToInt32(e.ProgressPercentage.ToString());
                ProgressChanged(length, prog, perc);
            };
            client.DownloadDataCompleted += (sender, ev) =>
            {
                ProgressComplete();
            };
        }
        public async Task<List<ProductionAccountingDBModel>> GetProductionList(string userid, string ProcessID,string LocationID,string PRStatus, int EntryType,
            string RefID="")
        {
            userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "ProdcutionAccounting?UserID=" + userid + "&ProcessID=" + ProcessID + "&LocationID=" + LocationID + 
                "&PRStatus=" + PRStatus+ "&EntryType="+ EntryType+(RefID!=""? "&RefID="+RefID:"");


            var data = await client.DownloadDataTaskAsync(new Uri(url));
            string result = System.Text.Encoding.UTF8.GetString(data);
            var aproves = JsonConvert.DeserializeObject<List<ProductionAccountingDBModel>>(result);
            if (aproves == null)
                aproves = new List<ProductionAccountingDBModel>();
            return aproves;
        }
        public async Task<List<DDL>> GetProductionDDL(string userid)
        {
            userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "ProdcutionAccounting?UserID=" + userid;

            var data = await client.DownloadDataTaskAsync(new Uri(url));
            string result = System.Text.Encoding.UTF8.GetString(data);
            var aproves = JsonConvert.DeserializeObject<List<DDL>>(result);
            if (aproves == null)
                aproves = new List<DDL>();
            return aproves;
        }
        //public int SetProduction(string RefNO, int Qty, string LocationRef, string AddedBy,List<Operation> OperationList)
        //{
            
        //        string url = RepositorySettings.BaseURl + "ProdcutionAccounting?RefNO="
        //            + RefNO + "&Qty=" + Qty + "&ProdDateTime=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm: ss.fff") + " &AddedBy=" + AddedBy
        //            + "&LocationRef=" + LocationRef;

        //        HttpClient client = new HttpClient();
        //        HttpResponseMessage result =  client.GetAsync(url).Result;
        //        var aproves = JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);

        //        return aproves;
            
        //}
        public async Task<int> SetProduction(string RefNO, int Qty, string LocationRef, string AddedBy, List<Operation> OperationList)
        {
            ProductionAccountingDBModel model = new ProductionAccountingDBModel
            {

                RefNo = RefNO,
                ProducedQty = Qty,
                LocationRef = LocationRef,
                AddedBy = AddedBy,
                OperationList=OperationList!=null?OperationList.Where(t=>t.Qty!=0).ToList():null,
                ProdDateTime=DateTime.Now
                
            };
            string url = RepositorySettings.BaseURl + "ProdcutionAccounting";
            HttpClient client = new HttpClient();
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
"application/json");
            HttpResponseMessage result =await client.PostAsync(url, contentPost);
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
        public async Task<List<DefectMaster>> GetGetDefectList()
        {
            
            string url = RepositorySettings.BaseURl + "ProdcutionAccounting/GetGetDefectList";
            var data = await client.DownloadDataTaskAsync(new Uri(url));
            string result = System.Text.Encoding.UTF8.GetString(data);
            var aproves = JsonConvert.DeserializeObject<List<DefectMaster>>(result);
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
        public async Task<List<Operation>> GetOperationList(string RefID)
        {

            string url = RepositorySettings.BaseURl + "Quality?RefID=" + RefID ;

            var data = await client.DownloadDataTaskAsync(new Uri(url));
            string result = System.Text.Encoding.UTF8.GetString(data);
            var aproves = JsonConvert.DeserializeObject<List<Operation>>(result);
            if (aproves == null)
                aproves = new List<Operation>();
            return aproves;
        }
    }
}
