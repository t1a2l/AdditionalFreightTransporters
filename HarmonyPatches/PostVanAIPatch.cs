using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(PostVanAI))]
    internal static class PostVanAIPatch
    {

        [HarmonyPatch(typeof(PostVanAI), "StartPathFind",
                [typeof(ushort), typeof(Vehicle), typeof(Vector3), typeof(Vector3), typeof(bool), typeof(bool), typeof(bool)],
                [ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal])]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpile(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            return VehicleTypeReplacingTranspiler.Transpile(original, instructions);
        }

    }
}