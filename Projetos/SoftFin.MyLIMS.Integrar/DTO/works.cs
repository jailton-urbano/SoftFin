using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.MyLIMS.Integrar.DTO
{
    public class Works
    {


        public class WorkType
        {
            public int Id { get; set; }
            public int MasterId { get; set; }
            public string Identification { get; set; }
            public bool Active { get; set; }
            public bool Confidential { get; set; }
            public bool LastVersion { get; set; }
            public int Version { get; set; }
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

        public class AccountType2
        {
            public int Id { get; set; }
            public string Identification { get; set; }
            public bool Active { get; set; }
        }

        public class RelatedAccount
        {
            public string Complement { get; set; }
            public object ReferenceKey { get; set; }
            public AccountType2 AccountType { get; set; }
            public object PriceList { get; set; }
            public bool Active { get; set; }
            public int Id { get; set; }
            public string Identification { get; set; }
        }

        public class OwnerUser
        {
            public int Id { get; set; }
            public string Identification { get; set; }
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
        }

        public class WorkFlowStepFrom
        {
            public int Id { get; set; }
            public string Identification { get; set; }
            public bool Active { get; set; }
        }

        public class WorkFlowStepTo
        {
            public int Id { get; set; }
            public string Identification { get; set; }
            public bool Active { get; set; }
        }

        public class ResponsibleUser
        {
            public int Id { get; set; }
            public string Identification { get; set; }
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
        }

        public class ResponsibleServiceArea
        {
            public int Id { get; set; }
            public string Identification { get; set; }
            public object ServiceCenter { get; set; }
            public object ExtraTime { get; set; }
            public bool ExternalServiceArea { get; set; }
            public bool Active { get; set; }
        }

        public class SampleStatus
        {
            public int Id { get; set; }
            public string Identification { get; set; }
            public bool BeforeReceive { get; set; }
            public bool AfterPublish { get; set; }
        }

        public class CurrentWorkFlow
        {
            public int Id { get; set; }
            public WorkFlowStepFrom WorkFlowStepFrom { get; set; }
            public WorkFlowStepTo WorkFlowStepTo { get; set; }
            public ResponsibleUser ResponsibleUser { get; set; }
            public ResponsibleServiceArea ResponsibleServiceArea { get; set; }
            public DateTime Conclusion { get; set; }
            public DateTime Execution { get; set; }
            public object EstimatedWork { get; set; }
            public bool AllowSamples { get; set; }
            public SampleStatus SampleStatus { get; set; }
            public bool Returned { get; set; }
        }

        public class Priority
        {
            public int Id { get; set; }
            public string Identification { get; set; }
        }

        public class ServiceCenter
        {
            public int Id { get; set; }
            public string Identification { get; set; }
            public bool Active { get; set; }
            public object PriceList { get; set; }
        }

        public class Result
        {
            public int Id { get; set; }
            public string ControlNumber { get; set; }
            public int Number { get; set; }
            public int Year { get; set; }
            public string Identification { get; set; }
            public bool Active { get; set; }
            public object ReferenceKey { get; set; }
            public bool Confidential { get; set; }
            public bool Finished { get; set; }
            public WorkType WorkType { get; set; }
            public Account Account { get; set; }
            public RelatedAccount RelatedAccount { get; set; }
            public OwnerUser OwnerUser { get; set; }
            public CurrentWorkFlow CurrentWorkFlow { get; set; }
            public Priority Priority { get; set; }
            public ServiceCenter ServiceCenter { get; set; }
            public object WorkConclusion { get; set; }
        }

        public class RootObject
        {
            public object Skip { get; set; }
            public int Count { get; set; }
            public object TotalCount { get; set; }
            public List<Result> Result { get; set; }
        }

    }
}
