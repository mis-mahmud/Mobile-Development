using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiApprovalSystemWebApiModels
{
    public class ProdcutionAccountingDBModel
    {
        public string RefNo { get; set; }
        public string LocationRef { get; set; }
        public string LocationName { get; set; }
        public string Style { get; set; }
        public string PR { get; set; }
        public string EO { get; set; }
        public string Process { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int OrderQty { get; set; }
        public int ProducedQty { get; set; }

        public int BalanceQty { get; set; }
        public int WIP { get; set; }
    }
    public class DDL
    {
        public string LocationRef { get; set; }
        public string LocationName { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
    }
}
