using AdditionalFreightTransporters.OptionsFramework.Extensions;
using AdditionalFreightTransporters.Utils;
using CitiesHarmony.API;
using ICities;

namespace AdditionalFreightTransporters
{
    public class AdditionalFreightTransportersMod : IUserMod
    {
        public static int MaxVehicleCount;
        
        public string Name => "Additional Freight Transporters";
        public string Description => "Adds new types of cargo transports - Barges, Helicopters and Trams. They are like cargo ships, cargo planes and cargo trains but use ferry paths, passenger helicopters paths and tram tracks";
        
        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>();
        }
        
        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(() => PatchUtil.PatchAll());
        }

        public void OnDisabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled) PatchUtil.UnpatchAll();
        }
    }
}
