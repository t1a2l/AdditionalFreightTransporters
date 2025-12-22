using System;
using AdditionalFreightTransporters.AI;
using AdditionalFreightTransporters.OptionsFramework;
using AdditionalFreightTransporters.Utils;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(BuildingInfo))]
    internal static class BuildingInfoPatch
    {
        [HarmonyPatch(typeof(BuildingInfo), "InitializePrefab")]
        [HarmonyPrefix]
        public static bool PreInitializePrefab(BuildingInfo __instance)
        {
            try
            {
                var Ai = __instance.GetComponent<PrefabAI>();

                if (__instance?.m_class?.name == ItemClasses.cargoFerryFacility.name && Ai is CargoHarborAI)
                {
                    var oldAi = __instance.GetComponent<CargoHarborAI>();
                    Object.DestroyImmediate(oldAi);
                    var ai = SteamHelper.IsDLCOwned(SteamHelper.DLC.IndustryDLC) &
                              OptionsWrapper<Options>.Options.EnableWarehouseAI
                        ? __instance.gameObject.AddComponent<CargoFerryWarehouseHarborAI>()
                        : __instance.gameObject.AddComponent<CargoFerryHarborAI>();
                    PrefabUtil.TryCopyAttributes(oldAi, ai, false);
                }

                if (Ai is CargoStationAI)
                {
                    if (__instance?.m_class?.name == ItemClasses.cargoHelicopterFacility.name)
                    {
                        var oldAi = __instance.GetComponent<CargoStationAI>();
                        Object.DestroyImmediate(oldAi);
                        var ai = SteamHelper.IsDLCOwned(SteamHelper.DLC.IndustryDLC) &
                                  OptionsWrapper<Options>.Options.EnableWarehouseAI
                            ? __instance.gameObject.AddComponent<CargoHelicopterWarehouseDepotAI>()
                            : __instance.gameObject.AddComponent<CargoHelicopterDepotAI>();
                        PrefabUtil.TryCopyAttributes(oldAi, ai, false);
                    }

                    if (__instance?.m_class?.name == ItemClasses.cargoTramFacility.name)
                    {
                        var oldAi = __instance.GetComponent<CargoStationAI>();
                        Object.DestroyImmediate(oldAi);
                        var ai = SteamHelper.IsDLCOwned(SteamHelper.DLC.IndustryDLC) &
                                  OptionsWrapper<Options>.Options.EnableWarehouseAI
                            ? __instance.gameObject.AddComponent<CargoTramWarehouseDepotAI>()
                            : __instance.gameObject.AddComponent<CargoTramDepotAI>();
                        PrefabUtil.TryCopyAttributes(oldAi, ai, false);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return true;
        }
    }
}