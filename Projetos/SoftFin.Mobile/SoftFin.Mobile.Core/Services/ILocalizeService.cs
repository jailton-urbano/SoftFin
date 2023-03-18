using System.Globalization;

namespace SoftFin.Mobile.Core.Services
{
    public interface ILocalizeService
    {
        CultureInfo GetCurrentCultureInfo();
    }
}
