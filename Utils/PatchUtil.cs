using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace AdditionalFreightTransporters.Utils
{
    internal static class PatchUtil
    {
        private const string HarmonyId = "AdditionalFreightTransporters";

        private static bool patched = false;

        public static void PatchAll()
        {
            if (patched)
            {
                return;
            }

            Debug.Log("AdditionalFreightTransporters: Patching...");

            patched = true;

            // Apply your patches here!
            // Harmony.DEBUG = true;
            var harmony = new Harmony("AdditionalFreightTransporters");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAll()
        {
            if (!patched)
            {
                return;
            }

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);

            patched = false;

            Debug.Log("AdditionalFreightTransporters: Reverted...");
        }
    }
}