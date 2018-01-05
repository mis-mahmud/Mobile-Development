using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SampleProcessModel
    {
        public string SampleID { get; set; }
        public string SampleRef { get; set; }
        public string SampleRequestDate { get; set; }
        public string SampleType { get; set; }
        public string StyleName { get; set; }
        public string StatusColor { get; set; }
        public string ProcessEndDt { get; set; }
        public string ManualProcessEndDt { get; set; }
        public string BuyerName { get; set; }
        public string DeliveryTentativeDate { get; set; }
        public string MerchRequestDt { get; set; }
        public bool OrderPlanned { get; set; }
        public bool MaterialReceived { get; set; }
    }
    public class SampleUpcommingModel
    {
        public Int64 RowSL { get; set; }
        public string SampleID { get; set; }
        public string Buyer { get; set; }
        public string ProductName { get; set; }
        public int ReqQty { get; set; }
        public string BuyerDeliveryDt { get; set; }
        public string PlanningDate { get; set; }
        public string StatusColor { get; set; }
        public string DevelopmentStatus { get; set; }
    }
    public class SampleFollowupModel
    {
        public int tblSampleRequestEntityId { get; set; }
        public string SampleID { get; set; }
        public string Buyer { get; set; }
        public string ProductName { get; set; }
        public int ReqQty { get; set; }
        public string BuyerDeliveryDt { get; set; }
        public int RequiredDays { get; set; }
        public string PlanningDate { get; set; }
        public string StatusColor { get; set; }
    }
}
