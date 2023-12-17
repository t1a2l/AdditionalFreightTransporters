using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AdditionalFreightTransporters.Utils;
using ColossalFramework;
using HarmonyLib;
using UnityEngine;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(CargoTruckAI))]
    internal static class CargoTruckAIPatch
    {
        [HarmonyPatch(typeof(CargoTruckAI), "NeedChangeVehicleType")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> NeedChangeVehicleTypeTranspile(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            return VehicleTypeReplacingTranspiler.Transpile(original, instructions);
        }

        [HarmonyPatch(typeof(CargoTruckAI), "StartPathFind")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> StartPathFindTranspile(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            return VehicleTypeReplacingTranspiler.Transpile(original, instructions);
        }

        [HarmonyPatch(typeof(CargoTruckAI), "ChangeVehicleType")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ChangeVehicleTypeTranspile(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            if(Util.IsModActive("Vehicle Selector"))
            {
                return default;
            }
            Debug.Log("AdditionalFreightTransporters: Transpiling method: " + original.DeclaringType + "." + original);
            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            foreach (var codeInstruction in codes)
            {
                if (SkipInstruction(codeInstruction))
                {
                    newCodes.Add(codeInstruction);
                    continue;
                }

                var patchIndex = newCodes.Count - 9;
                newCodes.RemoveRange(patchIndex, 2); //remove randomizer
                newCodes.Insert(patchIndex, new CodeInstruction(OpCodes.Ldloc_S, 6));
                newCodes.Insert(patchIndex + 1, new CodeInstruction(OpCodes.Ldloc_S, 7));
                newCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CargoTruckAIPatch), nameof(GetCargoVehicleInfo))));
                Debug.Log("AdditionalFreightTransporters: Transpiled CargoTruckAI.ChangeVehicleType()");
            }

            return newCodes.AsEnumerable();
        }

        private static bool SkipInstruction(CodeInstruction codeInstruction)
        {
            return codeInstruction.opcode != OpCodes.Callvirt || codeInstruction.operand == null || !codeInstruction.operand.ToString().Contains(nameof(VehicleManager.GetRandomVehicleInfo));
        }

        private static VehicleInfo GetCargoVehicleInfo(VehicleManager instance, ushort cargoStation1, ushort cargoStation2, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            var infoFrom = BuildingManager.instance.m_buildings.m_buffer[cargoStation1].Info;
            if (infoFrom?.m_class?.name == ItemClasses.cargoFerryFacility.name || infoFrom?.m_class?.name == ItemClasses.cargoHelicopterFacility.name) // To support Additional Freight Transporters
            {
                level = ItemClass.Level.Level5;
            }

            var vehicleInfo = instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, service, subService, level);
            if (vehicleInfo == null)
            {
                if (infoFrom?.m_class?.name == ItemClasses.cargoFerryFacility.name)
                {
                    Debug.LogWarning("AdditionalFreightTransporters: no barges found!");
                }
                if (infoFrom?.m_class?.name == ItemClasses.cargoHelicopterFacility.name)
                {
                    Debug.LogWarning("AdditionalFreightTransporters: no cargo Helicopters found!");
                }
                if (infoFrom?.m_class?.name == ItemClasses.cargoTramFacility.name)
                {
                    Debug.LogWarning("AdditionalFreightTransporters: no cargo Trams found!");
                }
            }
            return vehicleInfo;
        }

    }
}