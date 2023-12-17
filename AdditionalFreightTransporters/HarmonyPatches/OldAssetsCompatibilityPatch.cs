using HarmonyLib;
using UnityEngine;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(PackageHelper), "ResolveLegacyTypeHandler")]
    static class OldAssetsCompatibilityPatch
    {
        // 'CargoFerries.CargoFerryAI, CargoFerries, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
        [HarmonyPostfix]
        public static void Postfix(ref string __result)
        {
            string[] temp = __result.Split(',');
            if (temp[1] == " CargoFerries")
            {
                if (temp[0] == "CargoFerries.AI.CargoFerryAI")
                {
                    __result = "AdditionalFreightTransporters.AI.CargoFerryAI, AdditionalFreightTransporters, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }

                else if (temp[0] == "CargoFerries.AI.CargoFerryHarborAI")
                {
                    __result = "AdditionalFreightTransporters.AI.CargoFerryHarborAI, AdditionalFreightTransporters, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }

                else if(temp[0] == "CargoFerries.AI.CargoFerryWarehouseHarborAI")
                {
                    __result = "AdditionalFreightTransporters.AI.CargoFerryWarehouseHarborAI, AdditionalFreightTransporters, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }

                else if(temp[0] == "CargoFerries.AI.CargoHelicopterAI")
                {
                    __result = "AdditionalFreightTransporters.AI.CargoHelicopterAI, AdditionalFreightTransporters, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }

                else if(temp[0] == "CargoFerries.AI.CargoHelicopterDepotAI")
                {
                    __result = "AdditionalFreightTransporters.AI.CargoHelicopterDepotAI, AdditionalFreightTransporters, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }

            }
            Debug.Log(__result);
        }
    }
}