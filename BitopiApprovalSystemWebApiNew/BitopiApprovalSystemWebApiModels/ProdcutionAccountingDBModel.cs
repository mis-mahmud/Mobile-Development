using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiApprovalSystemWebApiModels
{
    public class ProductionAccountingDBModel
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
        public DateTime ProdDateTime { get; set; }
        public List<Operation> OperationList { get; set; }
        public HourlyProduction HourlyProduction { get; set; }
        public string Period { get; set; }
    }
    public class ProductionQualityDBModel : ProductionAccountingDBModel
    {
        public int LotQ { get; set; }
        public int Sample { get; set; }
        public string Check { get; set; }
        public int DefectiveUnit { get; set; }
        public string QualityStatus { get; set; }
        public List<DefectMaster> DefectList { get; set; }
    }
    public class ProductionRejectionDBModel : ProductionAccountingDBModel
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
        public int Qty { get; set; }

    }
    public class HourlyProduction
    {
        public int one { get; set; }
        public int two { get; set; }
        public int three { get; set; }
        public int four { get; set; }
        public int five { get; set; }
        public int six { get; set; }
        public int seven { get; set; }
        public int eight { get; set; }
        public int nine { get; set; }
        public int ten { get; set; }
        public int eleven { get; set; }
        public int tweleve { get; set; }
        public int thirteen { get; set; }
        public int fourteen { get; set; }
        public int fifteen { get; set; }
        public int sixteen { get; set; }
    }
}
