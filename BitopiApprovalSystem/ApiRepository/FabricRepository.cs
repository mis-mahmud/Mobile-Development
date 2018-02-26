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
    public class FabricRepository
    {
        public async Task<List<string>> GetGrnID()
        {
            List<string> grnList;
            try
            {
                //userid = Cipher.Encrypt(userid);
                string url = RepositorySettings.BaseURl + "FabricRoll/GetGrnID";
                HttpClient client = new HttpClient();
                HttpResponseMessage result = await client.GetAsync(url);
                string strResult = result.Content.ReadAsStringAsync().Result;
                grnList = JsonConvert.DeserializeObject<List<string>>(strResult);
                if (grnList == null)
                    grnList = new List<string>();
            }
            catch (Exception ex)
            {
                grnList = new List<string>();
            }
            return grnList;
        }
        public async Task<List<string>> GetColor(string GRNID)
        {
            List<string> colorList;
            try
            {
                //userid = Cipher.Encrypt(userid);
                string url = RepositorySettings.BaseURl + "FabricRoll/GetColor?GrnID=" + GRNID;
                HttpClient client = new HttpClient();
                HttpResponseMessage result = await client.GetAsync(url);
                string strResult = result.Content.ReadAsStringAsync().Result;
                colorList = JsonConvert.DeserializeObject<List<string>>(strResult);
                if (colorList == null)
                    colorList = new List<string>();
            }
            catch (Exception ex)
            {
                colorList = new List<string>();
            }
            return colorList;
        }
        public async Task<List<RollSettingsDBModel>> GetRollList(string GrnId, string Colorid)
        {
            List<RollSettingsDBModel> grnList;
            try
            {
                //userid = Cipher.Encrypt(userid);
                string url = RepositorySettings.BaseURl + "FabricRoll/GetRoll?GrnID="+GrnId+ "&Colorid="+Colorid;
                HttpClient client = new HttpClient();
                HttpResponseMessage result = await client.GetAsync(url);
                string strResult = result.Content.ReadAsStringAsync().Result;
                grnList = JsonConvert.DeserializeObject<List<RollSettingsDBModel>>(strResult);
                if (grnList == null)
                    grnList = new List<RollSettingsDBModel>();
            }
            catch (Exception ex)
            {
                grnList = new List<RollSettingsDBModel>();
            }
            return grnList;
        }
        public async Task<int> SetRoll(List<RollSettingsDBModel> RollSettingList)
        {
             
            string url = RepositorySettings.BaseURl + "FabricRoll/SetRolls";
            HttpClient client = new HttpClient();
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(RollSettingList), Encoding.UTF8,
"application/json");
            HttpResponseMessage result = await client.PostAsync(url, contentPost);
            var aproves = JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
            return aproves;
        }
        public async Task<int> SetRoll(RollSettingsDBModel roll)
        {

            string url = RepositorySettings.BaseURl + "FabricRoll/SetRoll";
            HttpClient client = new HttpClient();
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(roll), Encoding.UTF8,
"application/json");
            HttpResponseMessage result = await client.PostAsync(url, contentPost);
            var aproves = JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
            return aproves;
        }
        public async Task<List<UOM>> GetUOM()
        {
            List<UOM> uom;
            try
            {
                //userid = Cipher.Encrypt(userid);
                string url = RepositorySettings.BaseURl + "FabricRoll/GetUOM";
                HttpClient client = new HttpClient();
                HttpResponseMessage result = await client.GetAsync(url);
                string strResult = result.Content.ReadAsStringAsync().Result;
                uom = JsonConvert.DeserializeObject<List<UOM>>(strResult);
                if (uom == null)
                    uom =new List<UOM>();
            }
            catch (Exception ex)
            {
                uom = new List<UOM>();
            }
            return uom;
        }
        public async Task<RollSettingsDBModel> GetRollID(string GrnId, string Colorid)
        {
            RollSettingsDBModel roll;
            try
            {
                //userid = Cipher.Encrypt(userid);
                string url = RepositorySettings.BaseURl + "FabricRoll/GetRollID?GrnID=" + GrnId + "&Colorid=" + Colorid;
                HttpClient client = new HttpClient();
                HttpResponseMessage result = await client.GetAsync(url);
                string strResult = result.Content.ReadAsStringAsync().Result;
                roll = JsonConvert.DeserializeObject<RollSettingsDBModel>(strResult);
                if (roll == null)
                    roll = new RollSettingsDBModel();
            }
            catch (Exception ex)
            {
                roll = new RollSettingsDBModel();
            }
            return roll;
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
    

}
