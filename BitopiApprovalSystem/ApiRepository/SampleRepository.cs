using BitopiApprovalSystem;
using BitopiApprovalSystem.Model;
using Model;
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
    public class SampleRepository
    {
        public async Task<List<SampleProcessModel>> GetSampleRequisition()
        {
            List<SampleProcessModel> aproves;
            try
            {
                //userid = Cipher.Encrypt(userid);
                string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/GetSampleRequisition";

                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                {
                    HttpClient client = new HttpClient();
                    cookieContainer.Add(new Uri(RepositorySettings.BimobAppsBaseURl), new Cookie("ASP.NET_SessionId", "123"));
                    HttpResponseMessage result = await client.GetAsync(url);
                    string strResult = result.Content.ReadAsStringAsync().Result;
                    aproves = JsonConvert.DeserializeObject<List<SampleProcessModel>>(strResult);
                    if (aproves == null)
                        aproves = new List<SampleProcessModel>();
                }

                ;
            }
            catch (Exception ex)
            {
                aproves = new List<SampleProcessModel>();
            }
            return aproves;
        }
        public async Task<string> UpdateOrderRcvd(string SampleID, string ActualSewingDt, string UserSewingDt)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
    new KeyValuePair<string, string>("SampleID", SampleID),
    new KeyValuePair<string, string>("ActualSewingDt", ActualSewingDt),
    new KeyValuePair<string, string>("UserSewingDt", UserSewingDt)
});
            string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/UpdateOrderRcvd";
            HttpClient client = new HttpClient();
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(""), Encoding.UTF8,
"application/json");

            var response = await client.PostAsync(url, formContent);

            var aproves = JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return aproves.Result;
        }
        public async Task<string> UpdateMaterialRcvd(string SampleID)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
    new KeyValuePair<string, string>("SampleID", SampleID)
});
            string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/UpdateMaterialRcvd";
            HttpClient client = new HttpClient();
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(""), Encoding.UTF8,
"application/json");

            var response = await client.PostAsync(url, formContent);

            var aproves = JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return aproves.Result;
        }

        public async Task<List<DDLList>> LoadddlProductionProcess()
        {

            string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/GetProductionProcessList";
            HttpClient client = new HttpClient();
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(""), Encoding.UTF8,
"application/json");

            var response = await client.GetAsync(url);

            var aproves = JsonConvert.DeserializeObject<List<DDLList>>(response.Content.ReadAsStringAsync().Result);
            return aproves;
        }
        public async Task<List<SampleFollowupModel>> GetProcessFollowUpList(string ProdProcess, string ProdProcessStatus)
        {
            string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/GetProcessFollowUpList?ProdProcess=" + ProdProcess + "&ProdProcessStatus=" + ProdProcessStatus;
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);

            var aproves = JsonConvert.DeserializeObject<List<SampleFollowupModel>>(response.Content.ReadAsStringAsync().Result);
            return aproves;
        }
        public async Task<List<SampleUpcommingModel>> GetProcessFollowUp_Upcoming(int ProductionProcessId)
        {
            string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/GetProcessFollowUp_Upcoming?ProductionProcessId=" + ProductionProcessId;
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);

            var aproves = JsonConvert.DeserializeObject<List<SampleUpcommingModel>>(response.Content.ReadAsStringAsync().Result);
            return aproves;
        }
        public async Task<string> MakeDelivered(string EntityId, string productionProcess)
        {
            string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/MakeDelivered?EntityId=" + EntityId+ "&productionProcess="+ productionProcess;
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);

            var aproves = JsonConvert.DeserializeObject<Response>(response.Content.ReadAsStringAsync().Result);
            return aproves.Result;
        }
        public async Task<string> MakeReceived(string EntityId, string productionProcess)
        {
            string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/MakeReceived?EntityId=" + EntityId + "&productionProcess=" + productionProcess;
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);

            var aproves = JsonConvert.DeserializeObject< Response>(response.Content.ReadAsStringAsync().Result);
            return aproves.Result;
        }
    }
    public class Response
    {
        public string ResponseMsg { get; set; }
        public string Result { get; set; }
    }
    public class DDLList
    {
        public string DrpValue { get; set; }
        public string DrpText { get; set; }
    }

}
