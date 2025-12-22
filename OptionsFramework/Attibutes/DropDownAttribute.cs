using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AdditionalFreightTransporters.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DropDownAttribute(string description, string itemsClass, string group = null, string actionClass = null,
        string actionMethod = null) : AbstractOptionsAttribute(description, group, actionClass, actionMethod)
    {
        public IList<KeyValuePair<string, int>> GetItems(Func<string, string> translator = null)
        {
            var type = Util.FindType(ItemsClass);
            var enumValues = Enum.GetValues(type);
            return [.. from object enumValue in enumValues
                let code = (int) enumValue
                let memInfo = type.GetMember(Enum.GetName(type, enumValue))
                let attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                let description = ((DescriptionAttribute) attributes[0]).Description
                let translatedDesctiption = translator == null ? description : translator.Invoke(description)
                select new KeyValuePair<string, int>(translatedDesctiption, code)];
        }

        private string ItemsClass { get; } = itemsClass;
    }
}