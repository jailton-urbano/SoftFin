using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.MyLIMS.Integracao.DTO.SumPrice
{
    
    public class PriceItemType
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
    }

    public class ByPriceItemType
    {
        public PriceItemType PriceItemType { get; set; }
        public double TotalPrice { get; set; }
    }

    public class RootObject
    {
        public List<ByPriceItemType> ByPriceItemTypes { get; set; }
        public double TotalPriceToNotBill { get; set; }
        public double TotalPriceToBill { get; set; }
        public double TotalPriceBilled { get; set; }
        public double TotalPrice { get; set; }
    }
}