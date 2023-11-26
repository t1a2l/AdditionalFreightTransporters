using System;
using CargoFerries.AI;
using CargoFerries.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CargoFerries.HarmonyPatches.VehicleInfoPatch
{
    internal static class InitializePrefabPatch
    {
        private static bool deployed;

        public static void Apply()
        {
            if (deployed)
            {
                return;
            }

            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(VehicleInfo), nameof(VehicleInfo.InitializePrefab)),
                new PatchUtil.MethodDefinition(typeof(InitializePrefabPatch), nameof(PreInitializePrefab)));

            deployed = true;
        }

        public static void Undo()
        {
            if (!deployed)
            {
                return;
            }

            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(VehicleInfo), nameof(VehicleInfo.InitializePrefab)));

            deployed = false;
        }

        private static bool PreInitializePrefab(VehicleInfo __instance)
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