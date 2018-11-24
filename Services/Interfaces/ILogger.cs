using Services.Helpers;

namespace Services.Interfaces
{
    public interface ILogger
    {
        void Log(string format, object arg0);
        void Log(string value);
        MessagePrefix PrefixMessage { get; set; }
    }
}
