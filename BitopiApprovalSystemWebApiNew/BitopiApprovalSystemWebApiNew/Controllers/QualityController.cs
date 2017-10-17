using BitopiApprovalSystemWebApiModels;
using BitopiDBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BitopiApprovalSystemWebApiNew.Controllers
{
    public class QualityController : ApiController
    {
        DBProduction Context = new DBProduction();
        [HttpPost]
        public int SetQuality(ProductionQualityDBModel model)
        {
            int result = Context.SetQuality(model);
            return result;
        }
        public List<Operation> GetOperationList(string RefID = "")
        {
            return Context.GetOperationList(RefID);
        }
    }

}
