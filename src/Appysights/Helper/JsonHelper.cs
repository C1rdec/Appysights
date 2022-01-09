using System.Text.Json;

namespace Appysights.Helper
{
    public static class JsonHelper
    {
        #region Fields

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        #endregion

        #region Methods

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, Options);
        }

        public static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, Options);
        }

        #endregion
    }
}