using System;
using System.ComponentModel;

namespace AdditionalFreightTransporters.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.All)]
    public class DontTranslateDescriptionAttribute(string description) : DescriptionAttribute(description)
    {
    }
}