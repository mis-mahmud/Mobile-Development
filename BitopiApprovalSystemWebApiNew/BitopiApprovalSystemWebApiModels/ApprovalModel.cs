using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiApprovalSystemWebApiModels
{
    public class ApprovalModel
    {
        public ApprovalType Approval { get; set; }
        public string ApprovalName { get; set; }
        public int Count { get; set; }
        public ApprovalRoleType RoleType { get; set; }
        public string DataVisualType { get; set; }
    }
    public class ApprovalDetailsModel
    {
        public string POID { get; set; }

        public List<ApprovalDataModel> ApprovalDataList { get; set; }
        public bool isApproved { get; set; }
    }
    public class ApprovalDataModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class Message
    {
        public string POID { get; set; }
        [JsonIgnoreAttribute]
        public string DeviceID { get; set; }
        [JsonIgnoreAttribute]
        public string DeviceToken { get; set; }
        public string ApprovalName { get; set; }
        public string ApprovalType { get; set; }
        public string Approval { get; set; }

    }
}
