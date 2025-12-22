using System;

namespace AdditionalFreightTransporters.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LabelAttribute(string description, string group) : AbstractOptionsAttribute(description, group, null, null)
    {
    }
}