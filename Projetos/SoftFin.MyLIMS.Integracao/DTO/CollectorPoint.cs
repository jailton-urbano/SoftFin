using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.MyLIMS.Integracao.DTO.CollectorPoint
{
    public class EditionUser
    {
        public bool Active { get; set; }
        public object CultureId { get; set; }
        public object Login { get; set; }
        public object Email { get; set; }
        public object ServiceCenter { get; set; }
        public object ServiceArea { get; set; }
        public object Account { get; set; }
        public object SignatureCertFileId { get; set; }
        public object LicenseGroup { get; set; }
        public object ReadOnlyAccess { get; set; }
        public object Reserved { get; set; }
        public object ServiceAreaId { get; set; }
        public int Id { get; set; }
        public string Identification { get; set; }
    }

    public class Account
    {
        public int Id { get; set; }
        public string Identification { get; set; }
    }

    public class Country
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public object ShortName { get; set; }
    }

    public class State
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public object ShortName { get; set; }
        public object Country { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public object State { get; set; }
    }

    public class SampleReason
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
    }

    public class RootObject
    {
        public object Latitude { get; set; }
        public object Longitude { get; set; }
        public object Altitude { get; set; }
        public EditionUser EditionUser { get; set; }
        public DateTime EditionDateTime { get; set; }
        public string ZipCode { get; set; }
        public string District { get; set; }
        public Account Account { get; set; }
        public Country Country { get; set; }
        public State State { get; set; }
        public City City { get; set; }
        public SampleReason SampleReason { get; set; }
        public object CollectionPointClass { get; set; }
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
    }
}