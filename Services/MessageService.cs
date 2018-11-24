using Services.Interfaces;

namespace Services
{
    public class MessageService : IMessageService
    {
        private readonly IMailSender _messageSender;
        private readonly ILogger _logger;
        private int _counter;

        public MessageService(IMailSender sender, ILogger logger)
        {
            _counter = 0;
            _messageSender = sender;
            _logger = logger;
        }

        public void SendMessage(string message)
        {
            _logger.Log("Service is sending message...");
            _messageSender.SendMessage(message);

            _counter++;
        }

        public int GetSentCount()
        {
            return _counter;
        }
    }
}
