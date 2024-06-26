using System;
using AdditionalFreightTransporters.OptionsFramework.Attibutes;

namespace AdditionalFreightTransporters.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HideWhenNotInAssetEditorConditionAttribute : HideConditionAttribute
    {
        public override bool IsHidden()
        {
            return !SimulationManager.exists
                   || SimulationManager.instance.m_metaData is not {m_updateMode: SimulationManager.UpdateMode.LoadAsset or SimulationManager.UpdateMode.NewAsset};
        }
    }
}