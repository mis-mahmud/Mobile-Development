using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiApprovalSystem.Model
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
        public string Buyer { get; set; }
        public string DeliveryDate { get; set; }
        public string Color { get; set; }
        public int OrderQty { get; set; }
        public int ProducedQty { get; set; }

        public int BalanceQty { get; set; }
        public int WIP { get; set; }
        public string AddedBy { get; set; }
    }
    public class ProductionQualityDBModel : ProdcutionAccountingDBModel
    {
        public int LotQ { get; set; }
        public int Sample { get; set; }
        public string Check { get; set; }
        public int DefectiveUnit { get; set; }
        public string QualityStatus { get; set; }
        public List<DefectMaster> DefectList { get; set; }
    }
    public class ProductionRejectionDBModel : ProdcutionAccountingDBModel
    {
        public string Grade { get; set; }
        public int SKUCode { get; set; }
    }
    public class DDL
    {
        public string LocationRef { get; set; }
        public string LocationName { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
    }
    public class DefectMaster
    {
        public string DefectCode { get; set; }
        public string DefectName { get; set; }
        public string Category { get; set; }
        public string OperationCode { get; set; }
        public int No { get; set; }
    }
    public class Operation
    {
        public string OperationCode { get; set; }
        public string OperationName { get; set; }

    }
}
