using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softfin.Vindi.DTO
{
    public class bill
    {

        public string id { get; set; }
        public string code { get; set; }
        public string amount { get; set; }
        public string installments { get; set; }
        public string status { get; set; }
        public string seen_at { get; set; }
        public string billing_at { get; set; }
        public string due_at { get; set; }
        public string url { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        
    }
}
