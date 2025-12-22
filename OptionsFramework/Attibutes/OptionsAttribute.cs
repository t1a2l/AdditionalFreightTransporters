using System;

namespace AdditionalFreightTransporters.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class OptionsAttribute(string fileName, string legacyFileName = "") : Attribute
    {

        //file name in local app data
        public string FileName { get; } = fileName;

        //file name in Cities: Skylines folder
        public string LegacyFileName { get; } = legacyFileName;
    }
}