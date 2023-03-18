using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.MyLIMS.Integracao.DTO.WorksSamples
{
    public class ServiceCenter
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
        public object PriceList { get; set; }
    }

    public class SampleConclusion
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
    }

    public class PriceList
    {
        public bool Active { get; set; }
        public object Expire { get; set; }
        public bool ConsiderServiceCenterPriceListFromSampleOrWork { get; set; }
        public int Id { get; set; }
        public string Identification { get; set; }
    }

    public class SampleReason
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
    }

    public class SampleStatus
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool BeforeReceive { get; set; }
        public bool AfterPublish { get; set; }
    }

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

    public class CurrentStatus
    {
        public int Id { get; set; }
        public SampleStatus SampleStatus { get; set; }
        public EditionUser EditionUser { get; set; }
        public DateTime EditionDateTime { get; set; }
    }

    public class SampleType
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public bool Active { get; set; }
        public object SampleClass { get; set; }
        public object SamplePublishType { get; set; }
        public object SampleReason { get; set; }
        public object SampleTypeParent { get; set; }
        public object ReferenceKey { get; set; }
    }

    public class CollectionPoint
    {
        public object ZipCode { get; set; }
        public object District { get; set; }
        public object Account { get; set; }
        public object Country { get; set; }
        public object State { get; set; }
        public object City { get; set; }
        public object SampleReason { get; set; }
        public object CollectionPointClass { get; set; }
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
        public string Complement { get; set; }
        public object ReferenceKey { get; set; }
        public AccountType AccountType { get; set; }
        public object PriceList { get; set; }
        public bool Active { get; set; }
        public int Id { get; set; }
        public string Identification { get; set; }
    }

    public class Sample
    {
        public int Id { get; set; }
        public string Identification { get; set; }
        public object ControlIdentification { get; set; }
        public string ControlNumber { get; set; }
        public object ReferenceKey { get; set; }
        public object Prefix { get; set; }
        public int GroupId { get; set; }
        public int Number { get; set; }
        public int Year { get; set; }
        public int SubNumber { get; set; }
        public int Revision { get; set; }
        public bool Active { get; set; }
        public bool SyncPortal { get; set; }
        public bool Received { get; set; }
        public bool Finalized { get; set; }
        public bool Published { get; set; }
        public bool Reviewed { get; set; }
        public object Conclusion { get; set; }
        public object TakenDateTime { get; set; }
        public object ReceivedTime { get; set; }
        public object FinalizedTime { get; set; }
        public object PublishedTime { get; set; }
        public object ReviewedTime { get; set; }
        public object ExpectedCollectionTime { get; set; }
        public object ReferenceSample { get; set; }
        public object ConclusionTime { get; set; }
        public bool ConclusionTimeFixed { get; set; }
        public object QCTestsControlSample { get; set; }
        public ServiceCenter ServiceCenter { get; set; }
        public SampleConclusion SampleConclusion { get; set; }
        public PriceList PriceList { get; set; }
        public SampleReason SampleReason { get; set; }
        public CurrentStatus CurrentStatus { get; set; }
        public SampleType SampleType { get; set; }
        public CollectionPoint CollectionPoint { get; set; }
        public Account Account { get; set; }
        public object RelatedAccount { get; set; }
        public object SampleWorks { get; set; }
    }

    public class WorkSampleType
    {
        public int Id { get; set; }
        public string Identification { get; set; }
    }

    public class Result
    {
        public DTO.SumPrice.RootObject valor { get; set; }

        public int Id { get; set; }
        public int Amount { get; set; }
        public object Work { get; set; }
        public Sample Sample { get; set; }
        public WorkSampleType WorkSampleType { get; set; }
    }

    public class RootObject
    {
        public object Skip { get; set; }
        public int Count { get; set; }
        public object TotalCount { get; set; }
        public List<Result> Result { get; set; }
    }

}