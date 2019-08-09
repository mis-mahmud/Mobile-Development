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
    public class FabricRollController : ApiController
    {
        DBFabricRoll _db;
        // GET api/<controller>
        public FabricRollController()
        {
            _db = new DBFabricRoll();
        }
        public List<string> GetGrnID()
        {
            return _db.GetGRNID();
        }
        public List<string> GetColor(string GrnID)
        {
            return _db.GetColor(GrnID);
        }
        public List<RollSettingsDBModel> GetRoll(string GrnID, string colorid)
        {
            return _db.GetRoll(GrnID, colorid);
        }
        public RollSettingsDBModel GetRollID(string GrnID, string ColorID)
        {
            return _db.GetRollID(GrnID, ColorID);
        }
        [ActionName("GetUOM")]
        public List<UOM> GetUOM()
        {
            return _db.GetUOM();
        }
        [HttpPost]
        public int SetRolls(List<RollSettingsDBModel> list)
        {
            int result = _db.SetRoll(list);
            return result;
        }
        [HttpPost]
        public int SetRoll(RollSettingsDBModel model)
        {
            int result = _db.SetRoll(model);
            return result;
        }
        [HttpPost]
        [ActionName("SetScanResult")]
        public int SetScanResult([FromBody] List<ScanResult> result)
        {
            result = result;
            return 0;
        }
       
    }
}