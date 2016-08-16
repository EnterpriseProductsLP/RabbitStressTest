using Contracts;

namespace Publisher
{
    public class TestEvent : ITestEvent
    {
        public TestEvent(string clientName, string eventName, string payload)
        {
            ClientName = clientName;
            EventName = eventName;
            Payload = payload;
        }

        public string ClientName { get; }

        public string EventName { get; }

        public string Payload { get; }
    }
}