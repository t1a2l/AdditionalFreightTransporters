using System;

namespace AdditionalFreightTransporters.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckboxAttribute(string description, string group = null, string actionClass = null, string actionMethod = null) : AbstractOptionsAttribute(description, group, actionClass, actionMethod)
    {
    }
}