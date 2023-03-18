using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.MyLIMS.Integracao.DTO.Address
{
    public class AddressType
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
    }

    public class Country
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public string ShortName { get; set; }
    }

    public class State
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public string ShortName { get; set; }
        public Country Country { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public State State { get; set; }
    }

    public class Result
    {
        public int Id { get; set; }
        public string District { get; set; }
        public string ZipCode { get; set; }
        public string Address1 { get; set; }
        public object Address2 { get; set; }
        public object Latitude { get; set; }
        public object Longitude { get; set; }
        public string Notes { get; set; }
        public int AddressTypeId { get; set; }
        public AddressType AddressType { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
    }

    public class RootObject
    {
        public object Skip { get; set; }
        public int Count { get; set; }
        public object TotalCount { get; set; }
        public List<Result> Result { get; set; }
    }
}