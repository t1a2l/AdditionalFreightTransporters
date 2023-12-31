using AdditionalFreightTransporters.AI;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using HarmonyLib;
using ICities;
using System.Runtime.CompilerServices;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(WarehouseWorldInfoPanel))]
    public static class WarehouseUpdateBindingsPatch
    {
        [HarmonyPatch(typeof(WarehouseWorldInfoPanel), "UpdateBindings")]
        [HarmonyPrefix]
        public static bool UpdateBindings(WarehouseWorldInfoPanel __instance, ref InstanceID ___m_InstanceID, ref UILabel ___m_Type, ref UILabel ___m_Status, ref UILabel ___m_Upkeep,  ref UISprite ___m_Thumbnail, ref UILabel ___m_BuildingDesc,  ref UIButton ___m_RebuildButton, ref UIPanel ___m_ActionPanel, ref UIProgressBar ___m_resourceProgressBar, ref UILabel ___m_resourceLabel, ref UIPanel ___m_emptyingOldResource, ref UILabel ___m_resourceDescription, ref UISprite ___m_resourceSprite, ref UIPanel ___m_buffer, ref UILabel ___m_capacityLabel, ref UILabel ___m_Info)
        {
            var buildingAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI;
            if (buildingAI is CargoFerryWarehouseHarborAI || buildingAI is CargoHelicopterWarehouseHarborAI || buildingAI is CargoTramWarehouseHarborAI)
            {
                UpdateBindingsBasePatch.UpdateBindings(__instance);
                ushort building = ___m_InstanceID.Building;
                BuildingManager instance = Singleton<BuildingManager>.instance;
                Building building2 = instance.m_buildings.m_buffer[building];
                BuildingInfo info = building2.Info;
                ___m_Type.text = Singleton<BuildingManager>.instance.GetDefaultBuildingName(building, InstanceID.Empty);
                ___m_Status.text = buildingAI.GetLocalizedStatus(building, ref instance.m_buildings.m_buffer[___m_InstanceID.Building]);
                ___m_Upkeep.text = LocaleFormatter.FormatUpkeep(buildingAI.GetResourceRate(building, ref instance.m_buildings.m_buffer[building], EconomyManager.Resource.Maintenance), isDistanceBased: false);
                ___m_Thumbnail.atlas = info.m_Atlas;
                ___m_Thumbnail.spriteName = info.m_Thumbnail;
                if (___m_Thumbnail.atlas != null && !string.IsNullOrEmpty(___m_Thumbnail.spriteName))
                {
                    UITextureAtlas.SpriteInfo spriteInfo = ___m_Thumbnail.atlas[___m_Thumbnail.spriteName];
                    if (spriteInfo != null)
                    {
                        ___m_Thumbnail.size = spriteInfo.pixelSize;
                    }
                }
                ___m_BuildingDesc.text = info.GetLocalizedDescriptionShort();
                if ((building2.m_flags & Building.Flags.Collapsed) != 0)
                {
                    ___m_RebuildButton.tooltip = ((!IsDisasterServiceRequired(___m_InstanceID)) ? LocaleFormatter.FormatCost(buildingAI.GetRelocationCost(), isDistanceBased: false) : Locale.Get("CITYSERVICE_TOOLTIP_DISASTERSERVICEREQUIRED"));
                    ___m_RebuildButton.isVisible = Singleton<LoadingManager>.instance.SupportsExpansion(Expansion.NaturalDisasters);
                    ___m_RebuildButton.isEnabled = __instance.CanRebuild();
                    ___m_ActionPanel.isVisible = false;
                }
                else
                {
                    ___m_RebuildButton.isVisible = false;
                    ___m_ActionPanel.isVisible = true;
                }
                
                CargoFerryWarehouseHarborAI cargoFerryWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoFerryWarehouseHarborAI;
                CargoHelicopterWarehouseHarborAI cargoHelicopterWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoHelicopterWarehouseHarborAI;
                CargoTramWarehouseHarborAI cargoTramWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoTramWarehouseHarborAI;
                if (cargoFerryWarehouseHarborAI != null)
                {
                    int num = building2.m_customBuffer1 * 100;
                    ___m_resourceProgressBar.value = (float)num / (float)cargoFerryWarehouseHarborAI.m_storageCapacity;
                    TransferManager.TransferReason transferReason = cargoFerryWarehouseHarborAI.GetTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]);
                    TransferManager.TransferReason actualTransferReason = cargoFerryWarehouseHarborAI.GetActualTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]);
                    ___m_resourceProgressBar.progressColor = IndustryWorldInfoPanel.instance.GetResourceColor(actualTransferReason);
                    ___m_resourceLabel.text = Locale.Get("WAREHOUSEPANEL_RESOURCE", actualTransferReason.ToString());
                    ___m_emptyingOldResource.isVisible = transferReason != actualTransferReason;
                    ___m_resourceDescription.isVisible = transferReason != TransferManager.TransferReason.None;
                    ___m_resourceDescription.text = WarehouseWorldInfoPanel.GenerateResourceDescription(transferReason, isForWarehousePanel: true);
                    ___m_resourceSprite.spriteName = IndustryWorldInfoPanel.ResourceSpriteName(actualTransferReason);
                    string text = StringUtils.SafeFormat(Locale.Get("INDUSTRYPANEL_BUFFERTOOLTIP"), IndustryWorldInfoPanel.FormatResource((uint)num), IndustryWorldInfoPanel.FormatResourceWithUnit((uint)cargoFerryWarehouseHarborAI.m_storageCapacity, actualTransferReason));
                    ___m_buffer.tooltip = text;
                    ___m_capacityLabel.text = text;
                }
                else if (cargoHelicopterWarehouseHarborAI != null)
                {
                    int num = building2.m_customBuffer1 * 100;
                    ___m_resourceProgressBar.value = (float)num / (float)cargoHelicopterWarehouseHarborAI.m_storageCapacity;
                    TransferManager.TransferReason transferReason = cargoHelicopterWarehouseHarborAI.GetTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]);
                    TransferManager.TransferReason actualTransferReason = cargoHelicopterWarehouseHarborAI.GetActualTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]);
                    ___m_resourceProgressBar.progressColor = IndustryWorldInfoPanel.instance.GetResourceColor(actualTransferReason);
                    ___m_resourceLabel.text = Locale.Get("WAREHOUSEPANEL_RESOURCE", actualTransferReason.ToString());
                    ___m_emptyingOldResource.isVisible = transferReason != actualTransferReason;
                    ___m_resourceDescription.isVisible = transferReason != TransferManager.TransferReason.None;
                    ___m_resourceDescription.text = WarehouseWorldInfoPanel.GenerateResourceDescription(transferReason, isForWarehousePanel: true);
                    ___m_resourceSprite.spriteName = IndustryWorldInfoPanel.ResourceSpriteName(actualTransferReason);
                    string text = StringUtils.SafeFormat(Locale.Get("INDUSTRYPANEL_BUFFERTOOLTIP"), IndustryWorldInfoPanel.FormatResource((uint)num), IndustryWorldInfoPanel.FormatResourceWithUnit((uint)cargoHelicopterWarehouseHarborAI.m_storageCapacity, actualTransferReason));
                    ___m_buffer.tooltip = text;
                    ___m_capacityLabel.text = text;
                }
                else if (cargoTramWarehouseHarborAI != null)
                {
                    int num = building2.m_customBuffer1 * 100;
                    ___m_resourceProgressBar.value = (float)num / (float)cargoTramWarehouseHarborAI.m_storageCapacity;
                    TransferManager.TransferReason transferReason = cargoTramWarehouseHarborAI.GetTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]);
                    TransferManager.TransferReason actualTransferReason = cargoTramWarehouseHarborAI.GetActualTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]);
                    ___m_resourceProgressBar.progressColor = IndustryWorldInfoPanel.instance.GetResourceColor(actualTransferReason);
                    ___m_resourceLabel.text = Locale.Get("WAREHOUSEPANEL_RESOURCE", actualTransferReason.ToString());
                    ___m_emptyingOldResource.isVisible = transferReason != actualTransferReason;
                    ___m_resourceDescription.isVisible = transferReason != TransferManager.TransferReason.None;
                    ___m_resourceDescription.text = WarehouseWorldInfoPanel.GenerateResourceDescription(transferReason, isForWarehousePanel: true);
                    ___m_resourceSprite.spriteName = IndustryWorldInfoPanel.ResourceSpriteName(actualTransferReason);
                    string text = StringUtils.SafeFormat(Locale.Get("INDUSTRYPANEL_BUFFERTOOLTIP"), IndustryWorldInfoPanel.FormatResource((uint)num), IndustryWorldInfoPanel.FormatResourceWithUnit((uint)cargoTramWarehouseHarborAI.m_storageCapacity, actualTransferReason));
                    ___m_buffer.tooltip = text;
                    ___m_capacityLabel.text = text;
                }
                ___m_Info.text = buildingAI.GetLocalizedStats(building, ref instance.m_buildings.m_buffer[building]);
                return false;
            }
            return true;
        }

        private static bool IsDisasterServiceRequired(InstanceID m_InstanceID)
        {
            ushort building = m_InstanceID.Building;
            if (building != 0)
            {
                return Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_levelUpProgress != byte.MaxValue;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(BuildingWorldInfoPanel), "UpdateBindings")]
    class UpdateBindingsBasePatch
    {
        [HarmonyReversePatch]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void UpdateBindings(BuildingWorldInfoPanel instance)
        {
        }
    }
}
