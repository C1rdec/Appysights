using Appysights.Models;

namespace Appysights.Services
{
    public class ConfigurationService : AppdataServiceBase<Configuration>
    {
        protected override string FileName => "Services.json";
    }
}
