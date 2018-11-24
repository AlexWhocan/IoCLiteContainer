namespace Services.Interfaces
{
    public interface IMessageService
    {
        void SendMessage(string message);
        int GetSentCount();
    }
}