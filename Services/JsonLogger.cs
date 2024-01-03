using System.Text.Json;

namespace HoldemOddsAPI.Services
{
    public class JsonLogger
    {
        private readonly string _logFilePath;

        public JsonLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void LogError(Exception exception)
        {
            var errorInfo = new
            {
                Message = exception.Message,
                StrackTrance = exception.StackTrace,
                Time = DateTime.UtcNow.AddHours(1)
            };

            string json = JsonSerializer.Serialize(errorInfo, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            //Append the JSON string to the log file
            File.AppendAllText(_logFilePath, json + Environment.NewLine);
        }
    }
}
