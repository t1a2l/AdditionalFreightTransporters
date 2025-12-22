using System;

namespace AdditionalFreightTransporters.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SliderAttribute(string description, float min, float max, float step, string group = null, string actionClass = null, string actionMethod = null) : AbstractOptionsAttribute(description, group, actionClass, actionMethod)
    {
        public float Min { get; private set; } = min;

        public float Max { get; private set; } = max;

        public float Step { get; private set; } = step;
    }
}
