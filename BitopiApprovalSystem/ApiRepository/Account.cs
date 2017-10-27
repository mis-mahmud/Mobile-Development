using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiRepository
{
    public class AccountRepository
    {
        WebClient client;

        public AccountRepository()
        {
            client = new WebClient();
        }
        public AccountRepository(Action<long?, long, double?> ProgressChanged, Action ProgressComplete)
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
        public async Task<UserModel> getUser(string userName, string password, string DeviceID,
            string DeviceToken,
            string DeviceName,
            string Platform,
            int QryOption, int VersionCode, string UserCode = "")
        {
            string url = RepositorySettings.BaseURl + "Account?userName=" + userName + "&Password=" + password
                + "&DeviceID=" + DeviceID + " &DeviceToken=" + DeviceToken
                + "&DeviceName=" + DeviceName + "&Platform=" + Platform + "&QryOption=" + QryOption + "&UserCode="
                + UserCode + "&VersionCode=" + VersionCode;


            //HttpResponseMessage response = await client.GetAsync(url);
            //var result =  response.Content.ReadAsStringAsync().Result;

            
            var data = await client.DownloadDataTaskAsync(new Uri(url));
            string result = System.Text.Encoding.UTF8.GetString(data);
            var user = JsonConvert.DeserializeObject<UserModel>(result);
            
            return user;
        }
        public int GetVersion()
        {

            string url = RepositorySettings.BaseURl + "AppVersion";

            HttpClient client = new HttpClient();
            HttpResponseMessage result = client.GetAsync(url).Result;
            return JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
        }
        
    }

}
