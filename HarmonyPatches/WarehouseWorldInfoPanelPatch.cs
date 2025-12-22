using AdditionalFreightTransporters.AI;
using ColossalFramework.Globalization;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using HarmonyLib;
using ICities;
using System;

namespace AdditionalFreightTransporters.HarmonyPatches
{
    [HarmonyPatch(typeof(WarehouseWorldInfoPanel))]
    public static class WarehouseWorldInfoPanelPatch
    {
        [HarmonyPatch(typeof(WarehouseWorldInfoPanel), "OnSetTarget")]
        [HarmonyPrefix]
        public static bool OnSetTarget(WarehouseWorldInfoPanel __instance, ref UIPanel ___m_resourcePanel, ref float ___m_originalHeight, ref TransferManager.TransferReason[] ___m_transferReasons, ref InstanceID ___m_InstanceID, ref UIDropDown ___m_dropdownResource, ref UIDropDown ___m_dropdownMode)
        {
            var buildingAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI;
            if (buildingAI is CargoFerryWarehouseHarborAI || buildingAI is CargoHelicopterWarehouseDepotAI || buildingAI is CargoTramWarehouseDepotAI)
            {
                OnSetTargetBasePatch.OnSetTarget(__instance);
                CargoFerryWarehouseHarborAI cargoFerryWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoFerryWarehouseHarborAI;
                CargoHelicopterWarehouseDepotAI cargoHelicopterWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoHelicopterWarehouseDepotAI;
                CargoTramWarehouseDepotAI cargoTramWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoTramWarehouseDepotAI;
                if (cargoFerryWarehouseHarborAI != null)
                {
                    ___m_resourcePanel.isVisible = cargoFerryWarehouseHarborAI.m_storageType == TransferManager.TransferReason.None;
                }
                else if (cargoHelicopterWarehouseHarborAI != null)
                {
                    ___m_resourcePanel.isVisible = cargoHelicopterWarehouseHarborAI.m_storageType == TransferManager.TransferReason.None;
                }
                else if (cargoTramWarehouseHarborAI != null)
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
                        if (cargoFerryWarehouseHarborAI != null)
                        {
                            if (transferReason == cargoFerryWarehouseHarborAI.GetTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]))
                            {
                                ___m_dropdownResource.selectedIndex = num;
                                break;
                            }
                        }
                        else if (cargoHelicopterWarehouseHarborAI != null)
                        {
                            if (transferReason == cargoHelicopterWarehouseHarborAI.GetTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]))
                            {
                                ___m_dropdownResource.selectedIndex = num;
                                break;
                            }
                        }
                        else if (cargoTramWarehouseHarborAI != null)
                        {
                            if (transferReason == cargoTramWarehouseHarborAI.GetTransferReason(___m_InstanceID.Building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building]))
                            {
                                ___m_dropdownResource.selectedIndex = num;
                                break;
                            }
                        }
                        num++;
                    }
                }
                ___m_dropdownMode.selectedIndex = (int)AccessTools.PropertyGetter(typeof(WarehouseWorldInfoPanel), "warehouseMode").Invoke(__instance, null);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(WarehouseWorldInfoPanel), "UpdateBindings")]
        [HarmonyPrefix]
        public static bool UpdateBindings(WarehouseWorldInfoPanel __instance, ref InstanceID ___m_InstanceID, ref UILabel ___m_Type, ref UILabel ___m_Status, ref UILabel ___m_Upkeep, ref UISprite ___m_Thumbnail, ref UILabel ___m_BuildingDesc, ref UIButton ___m_RebuildButton, ref UIPanel ___m_ActionPanel, ref UIProgressBar ___m_resourceProgressBar, ref UILabel ___m_resourceLabel, ref UIPanel ___m_emptyingOldResource, ref UILabel ___m_resourceDescription, ref UISprite ___m_resourceSprite, ref UIPanel ___m_buffer, ref UILabel ___m_capacityLabel, ref UILabel ___m_Info, ref UILabel ___m_OverWorkSituation, ref UILabel ___m_UneducatedPlaces, ref UILabel ___m_EducatedPlaces, ref UILabel ___m_WellEducatedPlaces, ref UILabel ___m_HighlyEducatedPlaces, ref UILabel ___m_UneducatedWorkers, ref UILabel ___m_EducatedWorkers, ref UILabel ___m_WellEducatedWorkers, ref UILabel ___m_HighlyEducatedWorkers, ref UILabel ___m_JobsAvailLegend, ref UIRadialChart ___m_WorkPlacesEducationChart, ref UIRadialChart ___m_WorkersEducationChart, ref UILabel ___m_workersInfoLabel)
        {
            var buildingAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI;
            if (buildingAI is CargoFerryWarehouseHarborAI || buildingAI is CargoHelicopterWarehouseDepotAI || buildingAI is CargoTramWarehouseDepotAI)
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
                CargoHelicopterWarehouseDepotAI cargoHelicopterWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoHelicopterWarehouseDepotAI;
                CargoTramWarehouseDepotAI cargoTramWarehouseHarborAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI as CargoTramWarehouseDepotAI;
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
                UpdateWorkers(building, ref instance.m_buildings.m_buffer[building], ref ___m_OverWorkSituation, ref ___m_UneducatedPlaces, ref ___m_EducatedPlaces, ref ___m_WellEducatedPlaces, ref ___m_HighlyEducatedPlaces, ref ___m_UneducatedWorkers, ref ___m_EducatedWorkers, ref ___m_WellEducatedWorkers, ref ___m_HighlyEducatedWorkers, ref ___m_JobsAvailLegend, ref ___m_WorkPlacesEducationChart, ref ___m_WorkersEducationChart, ref ___m_workersInfoLabel);
                ___m_Info.text = buildingAI.GetLocalizedStats(building, ref instance.m_buildings.m_buffer[building]);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(WarehouseWorldInfoPanel), "OnDropdownResourceChanged")]
        [HarmonyPrefix]
        public static bool OnDropdownResourceChanged(WarehouseWorldInfoPanel __instance, UIComponent component, int index, ref InstanceID ___m_InstanceID, ref TransferManager.TransferReason[] ___m_transferReasons)
        {
            var buildingAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[___m_InstanceID.Building].Info.m_buildingAI;
            var buildingId = ___m_InstanceID.Building;
            var material = ___m_transferReasons[index];
            if (buildingAI is CargoFerryWarehouseHarborAI cargoFerryWarehouseHarborAI)
            {
                Singleton<SimulationManager>.instance.AddAction(delegate
                {
                    cargoFerryWarehouseHarborAI.SetTransferReason(buildingId, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId], material);
                });
                return false;
            }
            else if (buildingAI is CargoHelicopterWarehouseDepotAI cargoHelicopterWarehouseHarborAI)
            {
                Singleton<SimulationManager>.instance.AddAction(delegate
                {
                    cargoHelicopterWarehouseHarborAI.SetTransferReason(buildingId, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId], material);
                });
                return false;
            }
            else if (buildingAI is CargoTramWarehouseDepotAI cargoTramWarehouseHarborAI)
            {
                Singleton<SimulationManager>.instance.AddAction(delegate
                {
                    cargoTramWarehouseHarborAI.SetTransferReason(buildingId, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId], material);
                });
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

        private static void UpdateWorkers(ushort buildingID, ref Building building, ref UILabel m_OverWorkSituation, ref UILabel m_UneducatedPlaces, ref UILabel m_EducatedPlaces, ref UILabel m_WellEducatedPlaces, ref UILabel m_HighlyEducatedPlaces, ref UILabel m_UneducatedWorkers, ref UILabel m_EducatedWorkers, ref UILabel m_WellEducatedWorkers, ref UILabel m_HighlyEducatedWorkers, ref UILabel m_JobsAvailLegend, ref UIRadialChart m_WorkPlacesEducationChart, ref UIRadialChart m_WorkersEducationChart, ref UILabel m_workersInfoLabel)
        {
            if (!Singleton<CitizenManager>.exists)
            {
                return;
            }
            CitizenManager instance = Singleton<CitizenManager>.instance;
            uint num = building.m_citizenUnits;
            int num2 = 0;
            int num3 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            int num10 = 0;
            int num11 = 0;
            int num12 = 0;
            var buildingAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.m_buildingAI;
            if (buildingAI is CargoFerryWarehouseHarborAI cargoFerryWarehouseHarborAI)
            {
                num5 = cargoFerryWarehouseHarborAI.m_workPlaceCount0;
                num6 = cargoFerryWarehouseHarborAI.m_workPlaceCount1;
                num7 = cargoFerryWarehouseHarborAI.m_workPlaceCount2;
                num8 = cargoFerryWarehouseHarborAI.m_workPlaceCount3;
            }
            else if (buildingAI is CargoHelicopterWarehouseDepotAI cargoHelicopterWarehouseHarborAI)
            {
                num5 = cargoHelicopterWarehouseHarborAI.m_workPlaceCount0;
                num6 = cargoHelicopterWarehouseHarborAI.m_workPlaceCount1;
                num7 = cargoHelicopterWarehouseHarborAI.m_workPlaceCount2;
                num8 = cargoHelicopterWarehouseHarborAI.m_workPlaceCount3;
            }
            else if (buildingAI is CargoTramWarehouseDepotAI cargoTramWarehouseHarborAI)
            {
                num5 = cargoTramWarehouseHarborAI.m_workPlaceCount0;
                num6 = cargoTramWarehouseHarborAI.m_workPlaceCount1;
                num7 = cargoTramWarehouseHarborAI.m_workPlaceCount2;
                num8 = cargoTramWarehouseHarborAI.m_workPlaceCount3;
            }
            int num4 = num5 + num6 + num7 + num8;
            while (num != 0)
            {
                uint nextUnit = instance.m_units.m_buffer[num].m_nextUnit;
                if ((instance.m_units.m_buffer[num].m_flags & CitizenUnit.Flags.Work) != 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        uint citizen = instance.m_units.m_buffer[num].GetCitizen(i);
                        if (citizen != 0 && !instance.m_citizens.m_buffer[citizen].Dead && (instance.m_citizens.m_buffer[citizen].m_flags & Citizen.Flags.MovingIn) == 0)
                        {
                            num3++;
                            switch (instance.m_citizens.m_buffer[citizen].EducationLevel)
                            {
                                case Citizen.Education.Uneducated:
                                    num9++;
                                    break;
                                case Citizen.Education.OneSchool:
                                    num10++;
                                    break;
                                case Citizen.Education.TwoSchools:
                                    num11++;
                                    break;
                                case Citizen.Education.ThreeSchools:
                                    num12++;
                                    break;
                            }
                        }
                    }
                }
                num = nextUnit;
                if (++num2 > 524288)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
            int num13 = 0;
            int num14 = num5 - num9;
            if (num10 > num6)
            {
                num13 += Mathf.Max(0, Mathf.Min(num14, num10 - num6));
            }
            num14 += num6 - num10;
            if (num11 > num7)
            {
                num13 += Mathf.Max(0, Mathf.Min(num14, num11 - num7));
            }
            num14 += num7 - num11;
            if (num12 > num8)
            {
                num13 += Mathf.Max(0, Mathf.Min(num14, num12 - num8));
            }
            string format = Locale.Get((num13 != 1) ? "ZONEDBUILDING_OVEREDUCATEDWORKERS" : "ZONEDBUILDING_OVEREDUCATEDWORKER");
            m_OverWorkSituation.text = StringUtils.SafeFormat(format, num13);
            m_OverWorkSituation.isVisible = num13 > 0;
            m_UneducatedPlaces.text = num5.ToString();
            m_EducatedPlaces.text = num6.ToString();
            m_WellEducatedPlaces.text = num7.ToString();
            m_HighlyEducatedPlaces.text = num8.ToString();
            m_UneducatedWorkers.text = num9.ToString();
            m_EducatedWorkers.text = num10.ToString();
            m_WellEducatedWorkers.text = num11.ToString();
            m_HighlyEducatedWorkers.text = num12.ToString();
            m_JobsAvailLegend.text = (num4 - (num9 + num10 + num11 + num12)).ToString();
            int num15 = GetValue(num5, num4);
            int value = GetValue(num6, num4);
            int value2 = GetValue(num7, num4);
            int value3 = GetValue(num8, num4);
            int num16 = num15 + value + value2 + value3;
            if (num16 != 0 && num16 != 100)
            {
                num15 = 100 - (value + value2 + value3);
            }
            m_WorkPlacesEducationChart.SetValues(num15, value, value2, value3);
            int value4 = GetValue(num9, num4);
            int value5 = GetValue(num10, num4);
            int value6 = GetValue(num11, num4);
            int value7 = GetValue(num12, num4);
            int num18 = value4 + value5 + value6 + value7;
            int num17 = 100 - num18;
            m_WorkersEducationChart.SetValues(value4, value5, value6, value7, num17);
            m_workersInfoLabel.text = StringUtils.SafeFormat(Locale.Get("ZONEDBUILDING_WORKERS"), num3, num4);
        }

        private static int GetValue(int value, int total)
        {
            float num = (float)value / (float)total;
            return Mathf.Clamp(Mathf.FloorToInt(num * 100f), 0, 100);
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