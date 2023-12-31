using AdditionalFreightTransporters.AI;
using ColossalFramework;
using ColossalFramework.UI;
using HarmonyLib;
using System.Runtime.CompilerServices;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(WarehouseWorldInfoPanel))]
    public static class OnSetTargetPatch
    {
        [HarmonyPatch(typeof(WarehouseWorldInfoPanel), "OnSetTarget")]
        [HarmonyPrefix]
        public static bool OnSetTarget(WarehouseWorldInfoPanel __instance, ref UIPanel ___m_resourcePanel, ref float ___m_originalHeight, ref TransferManager.TransferReason[] ___m_transferReasons, ref InstanceID ___m_InstanceID, ref UIDropDown ___m_dropdownResource, ref UIDropDown ___m_dropdownMode)
        {
            var buildingAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI;
            if (buildingAI is CargoFerryWarehouseHarborAI || buildingAI is CargoHelicopterWarehouseHarborAI || buildingAI is CargoTramWarehouseHarborAI)
            {
                OnSetTargetBasePatch.OnSetTarget(__instance);
                CargoFerryWarehouseHarborAI cargoFerryWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoFerryWarehouseHarborAI;
                CargoHelicopterWarehouseHarborAI cargoHelicopterWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoHelicopterWarehouseHarborAI;
                CargoTramWarehouseHarborAI cargoTramWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoTramWarehouseHarborAI;
                if(cargoFerryWarehouseHarborAI != null)
                {
                    ___m_resourcePanel.isVisible = cargoFerryWarehouseHarborAI.m_storageType == TransferManager.TransferReason.None;
                }
                if (cargoHelicopterWarehouseHarborAI != null)
                {
                    ___m_resourcePanel.isVisible = cargoHelicopterWarehouseHarborAI.m_storageType == TransferManager.TransferReason.None;
                }
                if (cargoTramWarehouseHarborAI != null)
                {
                    ___m_resourcePanel.isVisible = cargoTramWarehouseHarborAI.m_storageType == TransferManager.TransferReason.None;
                }
                __instance.component.height = ((!___m_resourcePanel.isVisible) ? (___m_originalHeight - ___m_resourcePanel.height) : ___m_originalHeight);
                if (___m_resourcePanel.isVisible)
                {
                    int num = 0;
                    TransferManager.TransferReason[] transferReasons = ___m_transferReasons;
                    foreach (TransferManager.TransferReason transferReason in transferReasons)
                    {
                        if (transferReason == cargoFerryWarehouseHarborAI.GetTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]))
                        {
                            ___m_dropdownResource.selectedIndex = num;
                            break;
                        }
                        if (transferReason == cargoHelicopterWarehouseHarborAI.GetTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]))
                        {
                            ___m_dropdownResource.selectedIndex = num;
                            break;
                        }
                        if (transferReason == cargoTramWarehouseHarborAI.GetTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]))
                        {
                            ___m_dropdownResource.selectedIndex = num;
                            break;
                        }
                        num++;
                    }
                }
                ___m_dropdownMode.selectedIndex = (int)AccessTools.PropertyGetter(typeof(WarehouseWorldInfoPanel), "warehouseMode").Invoke(__instance, null);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(BuildingWorldInfoPanel), "OnSetTarget")]
    class OnSetTargetBasePatch
    {
        [HarmonyReversePatch]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnSetTarget(BuildingWorldInfoPanel instance)
        {
        }
    }
}
