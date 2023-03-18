using System.Web;
using System.Web.Mvc;

namespace SoftFin.POC.ConforLab
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
