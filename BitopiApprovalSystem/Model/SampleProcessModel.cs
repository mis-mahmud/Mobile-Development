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
}
