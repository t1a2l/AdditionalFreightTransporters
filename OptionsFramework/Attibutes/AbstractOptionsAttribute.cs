using System;
using System.Reflection;

namespace AdditionalFreightTransporters.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AbstractOptionsAttribute(string description, string group, string actionClass, string actionMethod) : Attribute
    {
        public string Description { get; } = description;
        public string Group { get; } = group;

        public Action<T> Action<T>()
        {
            if (ActionClass == null || ActionMethod == null)
            {
                return s => { };
            }
            var method = Util.FindType(ActionClass).GetMethod(ActionMethod, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                return s => { };
            }
            return s =>
            {
                method.Invoke(null, [s]);
            };
        }

        public Action Action()
        {
            if (ActionClass == null || ActionMethod == null)
            {
                return () => { };
            }
            var method = Util.FindType(ActionClass).GetMethod(ActionMethod, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                return () => { };
            }
            return () =>
            {
                method.Invoke(null, []);
            };
        }

        private string ActionClass { get; } = actionClass;

        private string ActionMethod { get; } = actionMethod;


    }
}