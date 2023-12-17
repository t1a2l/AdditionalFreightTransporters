using System;
using AdditionalFreightTransporters.OptionsFramework.Attibutes;

namespace AdditionalFreightTransporters.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HideInGameOrEditorConditionAttribute : HideConditionAttribute
    {
        public override bool IsHidden()
        {
            return SimulationManager.exists && SimulationManager.instance.m_metaData != null &&
                   SimulationManager.instance.m_metaData.m_updateMode != SimulationManager.UpdateMode.Undefined;
        }
    }
}