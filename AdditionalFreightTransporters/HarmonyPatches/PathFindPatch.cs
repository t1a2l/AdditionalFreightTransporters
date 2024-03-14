using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch]
    internal class PathFindPatch
    {
        private static readonly FieldInfo laneTypesField = typeof(PathFind).GetField("m_laneTypes", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo vehicleTypesField = typeof(PathFind).GetField("m_vehicleTypes", BindingFlags.NonPublic | BindingFlags.Instance);

        public static MethodBase TargetMethod()
        {
            return AccessTools.FirstMethod(typeof(PathFind), method => method.Name == "ProcessItem" && method.GetParameters().Length == 5);
        }

        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpile(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            var replaceNextWithNop = false;
            foreach (var codeInstruction in codes)
            {
                if (replaceNextWithNop)
                {
                    newCodes.Add(new CodeInstruction(OpCodes.Nop));
                    replaceNextWithNop = false;
                    continue;
                    
                }
                if (codeInstruction.opcode != OpCodes.Ldc_I4 || codeInstruction.operand is not 3072)
                {
                    newCodes.Add(codeInstruction);
                    continue;
                }
                newCodes.RemoveAt(newCodes.Count - 1);
                newCodes.Add(new CodeInstruction(OpCodes.Ldarg_3));
                newCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PathFindPatch), nameof(TreatSpecially))));
                //because there will be 'and' that has to be removed
                replaceNextWithNop = true;
                Debug.Log($"AdditionalFreightTransporters: PathFindPatch - prevented trucks from using train tracks");
            }
            return newCodes.AsEnumerable();
        }

        //prevents trucks from using train tracks. That issue happens because this mod adds Additional Freight Transporters to vehicleTypes
        //only trucks/post vans request CargoVehicle lane type so we can easily identify them
        private static bool TreatSpecially(PathFind pathFind, ref NetNode node)
        {
            var vehicleTypes = (VehicleInfo.VehicleType) vehicleTypesField.GetValue(pathFind);
            var laneTypes = (NetInfo.LaneType) laneTypesField.GetValue(pathFind);
            return ((laneTypes & NetInfo.LaneType.CargoVehicle) == NetInfo.LaneType.None || BelongsToAdditionalFreightTransportersNetwork(ref node)) && DoVanillaCheck(vehicleTypes);

            static bool DoVanillaCheck(VehicleInfo.VehicleType vehicleType)
            {
                return (vehicleType & (VehicleInfo.VehicleType.Ferry | VehicleInfo.VehicleType.Monorail | VehicleInfo.VehicleType.Helicopter | VehicleInfo.VehicleType.Tram)) != VehicleInfo.VehicleType.None;
            }
        }

        private static bool BelongsToAdditionalFreightTransportersNetwork(ref NetNode node)
        {
            for (var index = 0; index < 8; ++index)
            {
                var segment = node.GetSegment(index);
                if (segment == 0)
                {
                    continue;
                }
                var segmentInfo = NetManager.instance.m_segments.m_buffer[segment].Info;
                if (segmentInfo == null)
                {
                    continue;
                }

                if ((segmentInfo.m_vehicleTypes & VehicleInfo.VehicleType.Ferry) == VehicleInfo.VehicleType.None && 
                    (segmentInfo.m_vehicleTypes & VehicleInfo.VehicleType.Helicopter) == VehicleInfo.VehicleType.None && 
                    (segmentInfo.m_vehicleTypes & VehicleInfo.VehicleType.Tram) == VehicleInfo.VehicleType.None)
                {
                    return false;
                }
            }
            return true;
        }

    }
}