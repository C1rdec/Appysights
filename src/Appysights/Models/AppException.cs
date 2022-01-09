namespace Appysights.Models
{
    public class AppException
    {
        #region Properties

        public string Message { get; set; }

        public string Type { get; set; }

        public string Assembly { get; set; }

        public string Method { get; set; }

        public string OuterMessage { get; set; }

        #endregion
    }
}