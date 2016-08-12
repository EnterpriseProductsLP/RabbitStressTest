namespace Contracts
{
    public interface ITestEvent
    {
        string EventName { get; }

        string Payload { get; }
    }
}