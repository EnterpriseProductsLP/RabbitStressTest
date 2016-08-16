namespace Contracts
{
    public interface ITestEvent
    {
        string ClientName { get; }

        string EventName { get; }

        string Payload { get; }
    }
}