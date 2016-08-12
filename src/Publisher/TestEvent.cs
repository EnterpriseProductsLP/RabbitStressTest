using Contracts;

namespace Publisher
{
    public class TestEvent : ITestEvent
    {
        public TestEvent(string eventName, string payload)
        {
            EventName = eventName;
            Payload = payload;
        }

        public string EventName { get; }

        public string Payload { get; }
    }
}