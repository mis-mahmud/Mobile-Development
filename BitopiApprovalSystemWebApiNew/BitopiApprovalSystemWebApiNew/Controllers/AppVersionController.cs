using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BitopiApprovalSystemWebApiNew.Controllers
{
    public class AppVersionController : ApiController
    {
        [HttpGet]
        public int get()
        {
            int version= Convert.ToInt16(ConfigurationManager.AppSettings["AppVersion"]);
            return version;
        }
    }
}
