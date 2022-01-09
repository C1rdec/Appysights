namespace Appysights.Models
{
    public class ConfigChangedMessage
    {
        #region Properties

        public static ConfigChangedMessage Default => _instance;

        private static readonly ConfigChangedMessage _instance = new ConfigChangedMessage();

        #endregion
    }
}