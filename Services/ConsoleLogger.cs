using System;
using IoCLiteContainer.Attributes;
using Services.Helpers;
using Services.Interfaces;

namespace Services
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string value)
        {
            Console.WriteLine(value);
        }

        [InjectionRequired]
        public MessagePrefix PrefixMessage { get; set; }
        
        public void Log(string format, object arg0)
        {
            Console.WriteLine(format, arg0);
        }
    }
}
