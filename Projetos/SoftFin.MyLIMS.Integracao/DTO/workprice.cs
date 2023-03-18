using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.MyLIMS.Integracao.DTO.workprices
{
    public class PriceItemType
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
    }

    public class PriceItem
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public PriceItemType PriceItemType { get; set; }
        public double Price { get; set; }
        public object Bill { get; set; }
        public bool Active { get; set; }
    }

    public class Result
    {
        public int Id { get; set; }
        public object Work { get; set; }
        public object OriginalPrice { get; set; }
        public double Percent { get; set; }
        public bool Inherit { get; set; }
        public PriceItem PriceItem { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public object Description { get; set; }
        public bool Active { get; set; }
        public object StatusNotes { get; set; }
        public object Bill { get; set; }
        public object BillUser { get; set; }
        public object BillDateTime { get; set; }
        public object BillNotes { get; set; }
        public bool Billed { get; set; }
        public object BilledUser { get; set; }
        public object BilledDateTime { get; set; }
        public object BilledNotes { get; set; }
    }

    public class workprice
    {
        public object Skip { get; set; }
        public int Count { get; set; }
        public object TotalCount { get; set; }
        public List<Result> Result { get; set; }
    }
}