using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.MyLIMS.Integracao.DTO
{
    public class Info
    {
        public int InfoTypeId { get; set; }
        public object InfoType { get; set; }
        public bool Active { get; set; }
        public object MeasurementUnit { get; set; }
        public object ForceScale { get; set; }
        public object ForceSignifDigits { get; set; }
        public bool ReadOnlyValue { get; set; }
        public object EquipmentTypeId { get; set; }
        public object AccountTypeId { get; set; }
        public bool AllowAnyValue { get; set; }
        public bool AllowText { get; set; }
        public object ConsumableTypeId { get; set; }
        public object Options { get; set; }
        public object DLLFileId { get; set; }
        public int Id { get; set; }
        public string Identification { get; set; }
    }

    public class Result
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public Info Info { get; set; }
        public object MeasurementUnit { get; set; }
        public object DependentInfo { get; set; }
        public string DisplayValue { get; set; }
        public object ForceScale { get; set; }
        public object ForceSignifDigits { get; set; }
        public object InputScale { get; set; }
        public object InputSignifDigits { get; set; }
        public object InputFormat { get; set; }
        public string ValueText { get; set; }
        public object ValueInteger { get; set; }
        public object ValueFloat { get; set; }
        public object ValueDateTime { get; set; }
        public object ValueBoolean { get; set; }
        public object ValueAccount { get; set; }
    }

    public class workitem
    {
        public object Skip { get; set; }
        public int Count { get; set; }
        public object TotalCount { get; set; }
        public List<Result> Result { get; set; }
    }
}