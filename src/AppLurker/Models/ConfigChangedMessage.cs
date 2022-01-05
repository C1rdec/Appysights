namespace AppLurker.Models
{
    public class ConfigChangedMessage
    {
        private static readonly ConfigChangedMessage _instance = new ConfigChangedMessage();

        public static ConfigChangedMessage Default => _instance;
    }
}
