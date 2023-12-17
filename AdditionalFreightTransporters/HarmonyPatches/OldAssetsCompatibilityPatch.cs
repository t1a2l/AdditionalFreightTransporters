using HarmonyLib;
using UnityEngine;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(PackageHelper), "ResolveLegacyTypeHandler")]
    static class OldAssetsCompatibilityPatch
    {
        // 'CargoFerriesMod.CargoFerryAI, CargoFerriesMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
        [HarmonyPostfix]
        public static void Postfix(ref string __result)
        {
            string[] temp = __result.Split(',');
            if (temp[1] == " CargoFerriesMod")
            {
                if (temp[0] == "CargoFerries.AI.CargoFerryAI")
                {
                    __result = "AdditionalFreightTransporters.AI.CargoFerryAI, AdditionalFreightTransportersMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }

                else if (temp[0] == "CargoFerries.AI.CargoFerryHarborAI")
                {
                    __result = "AdditionalFreightTransporters.AI.CargoFerryHarborAI, AdditionalFreightTransportersMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }

                else if(temp[0] == "CargoFerries.AI.CargoFerryWarehouseHarborAI")
                {
                    __result = "AdditionalFreightTransporters.AI.CargoFerryWarehouseHarborAI, AdditionalFreightTransportersMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }

                else if(temp[0] == "CargoFerries.AI.CargoHelicopterAI")
                {
                    __result = "AdditionalFreightTransporters.AI.CargoHelicopterAI, AdditionalFreightTransportersMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }

                else if(temp[0] == "CargoFerries.AI.CargoHelicopterDepotAI")
                {
                    __result = "AdditionalFreightTransporters.AI.CargoHelicopterDepotAI, AdditionalFreightTransportersMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }

            }
            Debug.Log(__result);
        }
    }
}