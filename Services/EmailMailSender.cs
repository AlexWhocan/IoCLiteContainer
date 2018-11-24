using Services.Interfaces;

namespace Services
{
    /// <summary>
    /// Email Communicator Class, Sending message via Email
    /// </summary>
    public class EmailMailSender : IMailSender
    {
        private readonly ILogger _logger;

        public EmailMailSender(ILogger logger)
        {
            _logger = logger;
        }

        public void SendMessage(string body)
        {
            _logger.Log("Email sent successfully!\n{0}\n\n", body);
        }
    }
}
