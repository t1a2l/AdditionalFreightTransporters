using System;
using System.ComponentModel;

namespace AdditionalFreightTransporters.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.All)]
    public class DontTranslateDescriptionAttribute : DescriptionAttribute
    {
        public DontTranslateDescriptionAttribute(string description) :
            base(description)
        {
            
        }
    }
}