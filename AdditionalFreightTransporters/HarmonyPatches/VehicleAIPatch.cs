using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using System;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(VehicleAI), "GetColor", [typeof(ushort), typeof(Vehicle), typeof(InfoManager.InfoMode), typeof(InfoManager.SubInfoMode)],
                [ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal])]
    internal static class VehicleAIPatch
    {
        [HarmonyReversePatch]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Color GetColor(VehicleAI instance, ushort vehicleID, ref Vehicle data, InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subInfoMode)
        {
            throw new NotImplementedException("It's a stub");
        }
    }
}