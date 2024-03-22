using System;
using AdditionalFreightTransporters.AI;
using HarmonyLib;
using UnityEngine;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(VehicleInfo))]
    internal static class InitializePrefabVehiclePatch
    {
        [HarmonyPatch(typeof(VehicleInfo), "InitializePrefab")]
        [HarmonyPrefix]
        public static bool InitializePrefab(VehicleInfo __instance)
        {
            try
            {
                var component = __instance.GetComponent<PrefabAI>();
                if (__instance.m_class.m_service == ItemClass.Service.PublicTransport && __instance.m_class.m_subService == ItemClass.SubService.PublicTransportPlane 
                    && __instance.m_class.m_level == ItemClass.Level.Level5 && __instance.m_vehicleType == VehicleInfo.VehicleType.Helicopter && component != null && component is CargoHelicopterAI cargoHelicopterAI)
                {
                    cargoHelicopterAI.m_cargoCapacity = 3;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return true;
        }
    }
}