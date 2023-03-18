using Softfin.Vindi.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Vindi.callApi
{
    public class Fatura: BaseVindi
    {
        ///https://app.vindi.com.br:443/api/v1/bills?page=1&query=customer_id%3D889740&sort_by=id&sort_order=asc
        public Object getList(string holdingCode)
        {
            var ret = GetSync<Bills>("https://app.vindi.com.br:443/api/v1/bills?page=1&query=customer_id%3D" + holdingCode + "&sort_by=id&sort_order=asc");
            return ret;
        }
    }
}
