using System;
using AdditionalFreightTransporters.AI;
using AdditionalFreightTransporters.Utils;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(VehicleInfo))]
    internal static class VehicleInfoPatch
    {
        [HarmonyPatch(typeof(VehicleInfo), "InitializePrefab")]
        [HarmonyPrefix]
        public static bool PreInitializePrefab(VehicleInfo __instance)
        {
            try
            {
                var Ai = __instance.GetComponent<PrefabAI>();

                if (__instance?.m_class?.name == ItemClasses.cargoFerryVehicle.name && Ai is CargoShipAI)
                {
                    var oldAi = __instance.GetComponent<CargoShipAI>();
                    Object.DestroyImmediate(oldAi);
                    var ai = __instance.gameObject.AddComponent<CargoFerryAI>();
                    PrefabUtil.TryCopyAttributes(oldAi, ai, false);
                }

                if (__instance?.m_class?.name == ItemClasses.cargoHelicopterVehicle.name && Ai is PassengerHelicopterAI)
                {
                    var oldAi = __instance.GetComponent<PassengerHelicopterAI>();
                    Object.DestroyImmediate(oldAi);
                    var ai = __instance.gameObject.AddComponent<CargoHelicopterAI>();
                    PrefabUtil.TryCopyAttributes(oldAi, ai, false);
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