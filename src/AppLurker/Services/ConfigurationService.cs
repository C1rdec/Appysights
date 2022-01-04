using AppLurker.Models;

namespace AppLurker.Services
{
    public class ConfigurationService : AppdataServiceBase<Configuration>
    {
        protected override string FileName => "Services.json";
    }
}
