using AdditionalFreightTransporters.OptionsFramework.Extensions;
using AdditionalFreightTransporters.Utils;
using CitiesHarmony.API;
using ColossalFramework.UI;
using ICities;
using System;

namespace AdditionalFreightTransporters
{
    public class Mod : IUserMod
    {
        public static int MaxVehicleCount;
        
        public string Name => "Additional Freight Transporters";

        public string Description => "Adds new types of cargo transports - Barges, Helicopters and Trams. They are like cargo ships, cargo planes and cargo trains but use ferry paths, passenger helicopters paths and tram tracks";

        private static UISlider m_sliderBarge, m_sliderHelicopter;

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>();

            UIHelperBase group = helper.AddGroup("Delay Cargo Spawning");
            UIPanel panel = ((UIPanel)((UIHelper)group).self);
            UILabel label = panel.AddUIComponent<UILabel>();
            label.text = "How long to delay part-full cargo vehicles compared to the default. \ndelay. Takes effect after a game restart. Recommended: 4x";
            m_sliderBarge = (UISlider)group.AddSlider($"Cargo Barges:", 1f, 10f, 1f, Settings.DelayBarge.value, ChangeSliderBarge);
            m_sliderBarge.width = 400f;
            m_sliderBarge.tooltip = Settings.DelayBarge.value.ToString() + "x";
            m_sliderHelicopter = (UISlider)group.AddSlider($"Cargo Helicopters:", 1f, 10f, 1f, Settings.DelayHelicopter.value, ChangeSliderHelicopter);
            m_sliderHelicopter.width = 400f;
            m_sliderHelicopter.tooltip = Settings.DelayHelicopter.value.ToString() + "x";

            group.AddCheckbox("Wait until a barge is Full befor sending away", Settings.BargeWaitUntilFull.value, (b) =>
            {
                Settings.BargeWaitUntilFull.value = b;
            });

            group.AddSpace(10);
        }
        
        public void OnEnabled()
        {
            Settings.Init();
            HarmonyHelper.DoOnHarmonyReady(() => PatchUtil.PatchAll());
        }

        public void OnDisabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled) PatchUtil.UnpatchAll();
        }

        private static void ChangeSliderBarge(float v)
        {
            Settings.DelayBarge.value = Convert.ToInt32(v);
            m_sliderBarge.tooltip = v.ToString() + "x";
            m_sliderBarge.RefreshTooltip();
        }

        private static void ChangeSliderHelicopter(float v)
        {
            Settings.DelayHelicopter.value = Convert.ToInt32(v);
            m_sliderHelicopter.tooltip = v.ToString() + "x";
            m_sliderHelicopter.RefreshTooltip();
        }
    }
}
