namespace Appysights.Models
{
    public class AppException
    {
        public string Message { get; set; }

        public string Type { get; set; }

        public string Assembly { get; set; }

        public string Method { get; set; }

        public string OuterMessage { get; set; }
    }
}
