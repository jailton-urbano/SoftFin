using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.MyLIMS.Integracao.DTO.Account
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

    public class RelationshipWorkAccount
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
    }

    public class AccountType
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
    }

    public class Account
    {
        public object Complement { get; set; }
        public object ReferenceKey { get; set; }
        public AccountType AccountType { get; set; }
        public object PriceList { get; set; }
        public bool Active { get; set; }
        public int Id { get; set; }
        public string Identification { get; set; }
    }

    public class AccountType2
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
    }

    public class RelatedAccount
    {
        public object Complement { get; set; }
        public object ReferenceKey { get; set; }
        public AccountType2 AccountType { get; set; }
        public object PriceList { get; set; }
        public bool Active { get; set; }
        public int Id { get; set; }
        public string Identification { get; set; }
    }

    public class ActivationUser
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

    public class Result
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public EditionUser EditionUser { get; set; }
        public DateTime EditionDateTime { get; set; }
        public object Work { get; set; }
        public RelationshipWorkAccount RelationshipWorkAccount { get; set; }
        public Account Account { get; set; }
        public RelatedAccount RelatedAccount { get; set; }
        public ActivationUser ActivationUser { get; set; }
        public DateTime? ActivationDateTime { get; set; }
    }

    public class RootObject
    {
        public object Skip { get; set; }
        public int Count { get; set; }
        public object TotalCount { get; set; }
        public List<Result> Result { get; set; }
    }
}