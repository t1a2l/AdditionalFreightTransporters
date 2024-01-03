using AdditionalFreightTransporters.AI;
using ColossalFramework;
using HarmonyLib;
using UnityEngine;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(DefaultTool))]
    public static class DefaultToolPatch
    {
        [HarmonyPatch(typeof(DefaultTool), "OpenWorldInfoPanel")]
        [HarmonyPrefix]
        public static bool OpenWorldInfoPanel(DefaultTool __instance, InstanceID id, Vector3 position)
        {
            if (id.Building != 0)
            {
                BuildingInfo info = Singleton<BuildingManager>.instance.m_buildings.m_buffer[id.Building].Info;
                CargoFerryWarehouseHarborAI cargoFerryWarehouseHarborAI = info.m_buildingAI as CargoFerryWarehouseHarborAI;
                CargoHelicopterWarehouseDepotAI cargoHelicopterWarehouseHarborAI = info.m_buildingAI as CargoHelicopterWarehouseDepotAI;
                CargoTramWarehouseDepotAI cargoTramWarehouseHarborAI = info.m_buildingAI as CargoTramWarehouseDepotAI;
                if (Singleton<InstanceManager>.instance.SelectInstance(id))
                {
                    if (cargoFerryWarehouseHarborAI != null)
                    {
                        WorldInfoPanel.Show<WarehouseWorldInfoPanel>(position, id);
                        return false;
                    }
                    else if (cargoHelicopterWarehouseHarborAI != null)
                    {
                        WorldInfoPanel.Show<WarehouseWorldInfoPanel>(position, id);
                        return false;
                    }
                    else if (cargoTramWarehouseHarborAI != null)
                    {
                        WorldInfoPanel.Show<WarehouseWorldInfoPanel>(position, id);
                        return false;
                    }
                    else
                    {
                        WorldInfoPanel.Hide<WarehouseWorldInfoPanel>();
                    }
                }
            }
            else
            {
                WorldInfoPanel.Hide<WarehouseWorldInfoPanel>();
            }
            return true;
        }
    }
}
