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
    public class RejectionController : ApiController
    {
        DBProduction Context = new DBProduction();
        public int SetRejection(ProductionRejectionDBModel model)
        {
            int result = Context.SetRejection(model);
            return result;
        }
    }
}
