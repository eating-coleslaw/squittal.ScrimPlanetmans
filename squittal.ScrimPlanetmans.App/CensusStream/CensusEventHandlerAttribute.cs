using System;

namespace squittal.LivePlanetmans.CensusStream
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CensusEventHandlerAttribute : Attribute
    {
        public string EventName { get; private set; }
        public Type PayloadType { get; private set; }

        public CensusEventHandlerAttribute(string eventName, Type payloadType)
        {
            EventName = eventName;
            PayloadType = payloadType;
        }
    }
}
