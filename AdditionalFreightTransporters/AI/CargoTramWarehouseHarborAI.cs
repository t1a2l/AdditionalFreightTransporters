using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.Math;
using System;
using UnityEngine;

namespace AdditionalFreightTransporters.AI
{
    //based of CargoHarborAI but without animals, connections & checking height
    public class CargoTramWarehouseHarborAI : CargoTramDepotAI
    {
        [CustomizableProperty("Truck Count")] public int m_truckCount = 25;

        [CustomizableProperty("Storage Capacity")]
        public int m_storageCapacity = 350000;

        [CustomizableProperty("Storage Type")]
        public TransferManager.TransferReason m_storageType = TransferManager.TransferReason.None;

        protected override string GetLocalizedStatusActive(ushort buildingID, ref Building data)
        {
            if (IsFull(buildingID, ref data))
            {
                return Locale.Get("BUILDING_STATUS_FULL");
            }
            if ((data.m_flags & Building.Flags.Downgrading) != 0)
            {
                return Locale.Get("BUILDING_STATUS_EMPTYING");
            }
            return base.GetLocalizedStatusActive(buildingID, ref data);
        }

        public override void CreateBuilding(ushort buildingID, ref Building data)
        {
            base.CreateBuilding(buildingID, ref data);
            data.m_seniors = byte.MaxValue;
            data.m_adults = byte.MaxValue;
            if (GetTransferReason(buildingID, ref data) == TransferManager.TransferReason.None)
            {
                data.m_problems = Notification.AddProblems(data.m_problems, Notification.Problem1.ResourceNotSelected);
            }
        }

        public override void SimulationStep(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            base.SimulationStep(buildingID, ref buildingData, ref frameData);
            CheckCapacity(buildingID, ref buildingData);
            if ((Singleton<SimulationManager>.instance.m_currentFrameIndex & 0xFFF) >= 3840)
            {
                buildingData.m_finalExport = buildingData.m_tempExport;
                buildingData.m_finalImport = buildingData.m_tempImport;
                buildingData.m_tempExport = 0;
                buildingData.m_tempImport = 0;
            }
        }

        public override void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (material != GetActualTransferReason(buildingID, ref data))
            {
                base.StartTransfer(buildingID, ref data, material, offer);
                return;
            }
            VehicleInfo transferVehicleService = GetTransferVehicleService(material, ItemClass.Level.Level1, ref Singleton<SimulationManager>.instance.m_randomizer);
            if (transferVehicleService == null)
            {
                return;
            }
            Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
            if (Singleton<VehicleManager>.instance.CreateVehicle(out var vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, transferVehicleService, data.m_position, material, transferToSource: false, transferToTarget: true))
            {
                transferVehicleService.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[vehicle], buildingID);
                transferVehicleService.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[vehicle], material, offer);
                VehicleManager.instance.m_vehicles.m_buffer[vehicle].m_touristCount = 1;
                ushort building = offer.Building;
                if (building != 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_flags & Building.Flags.IncomingOutgoing) != 0)
                {
                    transferVehicleService.m_vehicleAI.GetSize(vehicle, ref vehicles.m_buffer[vehicle], out var size, out var _);
                    ExportResource(buildingID, ref data, material, size);
                }
                data.m_outgoingProblemTimer = 0;
            }
        }

        public static VehicleInfo GetTransferVehicleService(TransferManager.TransferReason material, ItemClass.Level level, ref Randomizer randomizer)
        {
            ItemClass.Service service = ItemClass.Service.Industrial;
            ItemClass.SubService subService = ItemClass.SubService.None;
            switch (material)
            {
                case TransferManager.TransferReason.Ore:
                case TransferManager.TransferReason.Coal:
                case TransferManager.TransferReason.Glass:
                case TransferManager.TransferReason.Metals:
                    subService = ItemClass.SubService.IndustrialOre;
                    break;
                case TransferManager.TransferReason.Logs:
                case TransferManager.TransferReason.Lumber:
                case TransferManager.TransferReason.Paper:
                case TransferManager.TransferReason.PlanedTimber:
                    subService = ItemClass.SubService.IndustrialForestry;
                    break;
                case TransferManager.TransferReason.Oil:
                case TransferManager.TransferReason.Petrol:
                case TransferManager.TransferReason.Petroleum:
                case TransferManager.TransferReason.Plastics:
                    subService = ItemClass.SubService.IndustrialOil;
                    break;
                case TransferManager.TransferReason.Grain:
                case TransferManager.TransferReason.Food:
                case TransferManager.TransferReason.Flours:
                    subService = ItemClass.SubService.IndustrialFarming;
                    break;
                case TransferManager.TransferReason.AnimalProducts:
                    service = ItemClass.Service.PlayerIndustry;
                    subService = ItemClass.SubService.PlayerIndustryFarming;
                    break;
                case TransferManager.TransferReason.Fish:
                    service = ItemClass.Service.Fishing;
                    break;
                case TransferManager.TransferReason.Goods:
                    subService = ItemClass.SubService.IndustrialGeneric;
                    break;
                case TransferManager.TransferReason.LuxuryProducts:
                    service = ItemClass.Service.PlayerIndustry;
                    break;
                default:
                    return null;
            }
            return Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, service, subService, level);
        }

        public override void ModifyMaterialBuffer(ushort buildingID, ref Building data, TransferManager.TransferReason material, ref int amountDelta)
        {
            if (material == GetActualTransferReason(buildingID, ref data))
            {
                int num = data.m_customBuffer1 * 100;
                amountDelta = Mathf.Clamp(amountDelta, -num, m_storageCapacity - num);
                data.m_customBuffer1 = (ushort)((num + amountDelta) / 100);
            }
            else
            {
                base.ModifyMaterialBuffer(buildingID, ref data, material, ref amountDelta);
            }
        }

        public override void BuildingDeactivated(ushort buildingID, ref Building data)
        {
            TransferManager.TransferReason actualTransferReason = GetActualTransferReason(buildingID, ref data);
            if (actualTransferReason != TransferManager.TransferReason.None)
            {
                TransferManager.TransferOffer offer = default;
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveIncomingOffer(actualTransferReason, offer);
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(actualTransferReason, offer);
                RemoveGuestVehicles(buildingID, ref data, actualTransferReason);
            }

            base.BuildingDeactivated(buildingID, ref data);
        }

        private void RemoveGuestVehicles(ushort buildingID, ref Building data, TransferManager.TransferReason material)
        {
            Vehicle[] buffer = Singleton<VehicleManager>.instance.m_vehicles.m_buffer;
            ushort num = data.m_guestVehicles;
            int num2 = 0;
            while (num != 0)
            {
                ushort nextGuestVehicle = buffer[num].m_nextGuestVehicle;
                if (buffer[num].m_targetBuilding == buildingID && (TransferManager.TransferReason)buffer[num].m_transferType == material)
                {
                    VehicleInfo info = buffer[num].Info;
                    if (info != null)
                    {
                        info.m_vehicleAI.SetTarget(num, ref buffer[num], 0);
                    }
                }
                num = nextGuestVehicle;
                if (++num2 > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        private void CheckCapacity(ushort buildingID, ref Building buildingData)
        {
            int num = buildingData.m_customBuffer1 * 100;
            if (num * 3 >= m_storageCapacity * 2)
            {
                if ((buildingData.m_flags & Building.Flags.CapacityFull) != Building.Flags.CapacityFull)
                {
                    buildingData.m_flags |= Building.Flags.CapacityFull;
                }
            }
            else if (num * 3 >= m_storageCapacity)
            {
                if ((buildingData.m_flags & Building.Flags.CapacityFull) != Building.Flags.CapacityStep2)
                {
                    buildingData.m_flags = (buildingData.m_flags & ~Building.Flags.CapacityFull) | Building.Flags.CapacityStep2;
                }
            }
            else if (num >= GetMaxLoadSize())
            {
                if ((buildingData.m_flags & Building.Flags.CapacityFull) != Building.Flags.CapacityStep1)
                {
                    buildingData.m_flags = (buildingData.m_flags & ~Building.Flags.CapacityFull) | Building.Flags.CapacityStep1;
                }
            }
            else if ((buildingData.m_flags & Building.Flags.CapacityFull) != 0)
            {
                buildingData.m_flags &= ~Building.Flags.CapacityFull;
            }
        }

        protected override void ProduceGoods(ushort buildingID, ref Building buildingData, ref Building.Frame frameData, int productionRate, int finalProductionRate, ref Citizen.BehaviourData behaviour, int aliveWorkerCount, int totalWorkerCount, int workPlaceCount, int aliveVisitorCount, int totalVisitorCount, int visitPlaceCount)
        {
            DistrictManager instance = Singleton<DistrictManager>.instance;
            byte b = instance.GetPark(buildingData.m_position);
            if (b != 0 && !instance.m_parks.m_buffer[b].IsIndustry)
            {
                b = 0;
            }
            if (finalProductionRate != 0)
            {
                HandleDead(buildingID, ref buildingData, ref behaviour, totalWorkerCount);
                TransferManager.TransferReason actualTransferReason = GetActualTransferReason(buildingID, ref buildingData);
                TransferManager.TransferReason transferReason = GetTransferReason(buildingID, ref buildingData);
                if (actualTransferReason != TransferManager.TransferReason.None)
                {
                    int maxLoadSize = GetMaxLoadSize();
                    bool flag = IsFull(buildingID, ref buildingData);
                    int num = buildingData.m_customBuffer1 * 100;
                    int num2 = (finalProductionRate * m_truckCount + 99) / 100;
                    int count = 0;
                    int cargo = 0;
                    int capacity = 0;
                    int outside = 0;
                    CalculateOwnVehicles(buildingID, ref buildingData, actualTransferReason, ref count, ref cargo, ref capacity, ref outside);
                    buildingData.m_tempExport = (byte)Mathf.Clamp(outside, buildingData.m_tempExport, 255);
                    int count2 = 0;
                    int cargo2 = 0;
                    int capacity2 = 0;
                    int outside2 = 0;
                    CalculateGuestVehicles(buildingID, ref buildingData, actualTransferReason, ref count2, ref cargo2, ref capacity2, ref outside2);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(outside2, buildingData.m_tempImport, 255);
                    if (b != 0)
                    {
                        instance.m_parks.m_buffer[b].AddBufferStatus(actualTransferReason, num, cargo2, m_storageCapacity);
                    }
                    ushort num3 = buildingID;
                    bool flag2 = num3 == buildingID;
                    if (transferReason == actualTransferReason)
                    {
                        if (num >= maxLoadSize && (count < num2 || !flag2))
                        {
                            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                            if ((buildingData.m_flags & Building.Flags.Filling) != 0)
                            {
                                offer.Priority = 0;
                            }
                            else if ((buildingData.m_flags & Building.Flags.Downgrading) != 0)
                            {
                                offer.Priority = Mathf.Clamp(num / Mathf.Max(1, m_storageCapacity >> 2) + 2, 0, 2);
                                if (!flag2)
                                {
                                    offer.Priority += 2;
                                }
                            }
                            else
                            {
                                offer.Priority = Mathf.Clamp(num / Mathf.Max(1, m_storageCapacity >> 2) - 1, 0, 2);
                            }
                            offer.Building = num3;
                            offer.Position = buildingData.m_position;
                            offer.Amount = ((!flag2) ? Mathf.Clamp(num / maxLoadSize, 1, 15) : Mathf.Min(Mathf.Max(1, num / maxLoadSize), num2 - count));
                            offer.Active = true;
                            offer.Exclude = flag2;
                            offer.Unlimited = !flag2;
                            Singleton<TransferManager>.instance.AddOutgoingOffer(actualTransferReason, offer);
                        }
                        if ((buildingData.m_flags & Building.Flags.Downgrading) != 0)
                        {
                            Vehicle[] buffer = Singleton<VehicleManager>.instance.m_vehicles.m_buffer;
                            ushort num5 = buildingData.m_guestVehicles;
                            int num6 = 0;
                            while (num5 != 0 && cargo2 > 0 && (float)(num + cargo2) > (float)m_storageCapacity * 0.2f + (float)maxLoadSize)
                            {
                                ushort nextGuestVehicle = buffer[num5].m_nextGuestVehicle;
                                if (buffer[num5].m_targetBuilding == buildingID && (TransferManager.TransferReason)buffer[num5].m_transferType == actualTransferReason)
                                {
                                    VehicleInfo info2 = buffer[num5].Info;
                                    if (info2 != null)
                                    {
                                        cargo2 = Mathf.Max(0, cargo2 - buffer[num5].m_transferSize);
                                        info2.m_vehicleAI.SetTarget(num5, ref buffer[num5], 0);
                                    }
                                }
                                num5 = nextGuestVehicle;
                                if (++num6 > 16384)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                    break;
                                }
                            }
                        }
                        num += cargo2;
                        if (num < m_storageCapacity)
                        {
                            TransferManager.TransferOffer offer2 = default(TransferManager.TransferOffer);
                            bool flag3 = true;
                            if ((buildingData.m_flags & Building.Flags.Downgrading) != 0)
                            {
                                if ((float)num < (float)m_storageCapacity * 0.2f)
                                {
                                    offer2.Priority = 0;
                                }
                                else
                                {
                                    flag3 = false;
                                }
                            }
                            else if ((buildingData.m_flags & Building.Flags.Filling) != 0)
                            {
                                offer2.Priority = Mathf.Clamp((m_storageCapacity - num) / Mathf.Max(1, m_storageCapacity >> 2) + 1, 0, 2);
                                if (!flag2)
                                {
                                    offer2.Priority += 2;
                                }
                            }
                            else
                            {
                                offer2.Priority = Mathf.Clamp((m_storageCapacity - num) / Mathf.Max(1, m_storageCapacity >> 2) - 1, 0, 2);
                            }
                            if (flag3)
                            {
                                offer2.Building = num3;
                                offer2.Position = buildingData.m_position;
                                offer2.Amount = Mathf.Max(1, (m_storageCapacity - num) / maxLoadSize);
                                offer2.Active = false;
                                offer2.Exclude = flag2;
                                offer2.Unlimited = !flag2;
                                Singleton<TransferManager>.instance.AddIncomingOffer(actualTransferReason, offer2);
                            }
                        }
                    }
                    else if (num > 0 && (count < num2 || !flag2))
                    {
                        TransferManager.TransferOffer offer3 = default(TransferManager.TransferOffer);
                        offer3.Priority = 8;
                        offer3.Building = num3;
                        offer3.Position = buildingData.m_position;
                        offer3.Amount = ((!flag2) ? Mathf.Clamp(num / maxLoadSize, 1, 15) : Mathf.Min(Mathf.Max(1, num / maxLoadSize), num2 - count));
                        offer3.Active = true;
                        offer3.Exclude = flag2;
                        offer3.Unlimited = !flag2;
                        Singleton<TransferManager>.instance.AddOutgoingOffer(actualTransferReason, offer3);
                    }
                }
                if (actualTransferReason != transferReason && buildingData.m_customBuffer1 == 0)
                {
                    buildingData.m_adults = buildingData.m_seniors;
                    SetContentFlags(buildingID, ref buildingData, transferReason);
                }
                int num7 = finalProductionRate * m_noiseAccumulation / 100;
                if (num7 != 0)
                {
                    Singleton<ImmaterialResourceManager>.instance.AddResource(ImmaterialResourceManager.Resource.NoisePollution, num7, buildingData.m_position, m_noiseRadius);
                }
            }
            base.ProduceGoods(buildingID, ref buildingData, ref frameData, productionRate, finalProductionRate, ref behaviour, aliveWorkerCount, totalWorkerCount, workPlaceCount, aliveVisitorCount, totalVisitorCount, visitPlaceCount);
        }

        public override string GetDebugString(ushort buildingID, ref Building data)
        {
            TransferManager.TransferReason actualTransferReason = GetActualTransferReason(buildingID, ref data);
            if (actualTransferReason == TransferManager.TransferReason.None)
            {
                return base.GetDebugString(buildingID, ref data);
            }
            int count = 0;
            int cargo = 0;
            int capacity = 0;
            int outside = 0;
            CalculateGuestVehicles(buildingID, ref data, actualTransferReason, ref count, ref cargo, ref capacity, ref outside);
            int num = data.m_customBuffer1 * 100;
            return StringUtils.SafeFormat("{0}\n{1}: {2} (+{3})", base.GetDebugString(buildingID, ref data), actualTransferReason, num, cargo);
        }

        public override void SetEmptying(ushort buildingID, ref Building data, bool emptying)
        {
            if (emptying)
            {
                data.m_flags = (data.m_flags & ~Building.Flags.Filling) | Building.Flags.Downgrading;
            }
            else
            {
                data.m_flags &= ~Building.Flags.Downgrading;
            }
        }

        public override void SetFilling(ushort buildingID, ref Building data, bool filling)
        {
            if (filling)
            {
                data.m_flags = (data.m_flags & ~Building.Flags.Downgrading) | Building.Flags.Filling;
            }
            else
            {
                data.m_flags &= ~Building.Flags.Filling;
            }
        }

        public void SetTransferReason(ushort buildingID, ref Building data, TransferManager.TransferReason material)
        {
            if (m_storageType != TransferManager.TransferReason.None)
            {
                return;
            }
            TransferManager.TransferReason seniors = (TransferManager.TransferReason)data.m_seniors;
            if (material != seniors)
            {
                if (seniors != TransferManager.TransferReason.None)
                {
                    TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                    offer.Building = buildingID;
                    Singleton<TransferManager>.instance.RemoveIncomingOffer(seniors, offer);
                    CancelIncomingTransfer(buildingID, ref data, seniors);
                }
                data.m_seniors = (byte)material;
                if (data.m_customBuffer1 == 0)
                {
                    data.m_adults = (byte)material;
                    SetContentFlags(buildingID, ref data, material);
                }
            }
            Notification.ProblemStruct problems = data.m_problems;
            if (material == TransferManager.TransferReason.None)
            {
                data.m_problems = Notification.AddProblems(data.m_problems, Notification.Problem1.ResourceNotSelected);
            }
            else
            {
                data.m_problems = Notification.RemoveProblems(data.m_problems, Notification.Problem1.ResourceNotSelected);
            }
            if (data.m_problems != problems)
            {
                Singleton<BuildingManager>.instance.UpdateNotifications(buildingID, problems, data.m_problems);
            }
        }

        private void SetContentFlags(ushort buildingID, ref Building data, TransferManager.TransferReason material)
        {
            switch (material)
            {
                case TransferManager.TransferReason.AnimalProducts:
                case TransferManager.TransferReason.Fish:
                    data.m_flags = (data.m_flags & ~Building.Flags.Content01_Forbid) | Building.Flags.LevelUpEducation;
                    break;
                case TransferManager.TransferReason.Coal:
                    data.m_flags = (data.m_flags & ~Building.Flags.Content02_Forbid) | Building.Flags.LevelUpLandValue;
                    break;
                case TransferManager.TransferReason.Flours:
                    data.m_flags = (data.m_flags & ~Building.Flags.Content03_Forbid) | Building.Flags.Content03;
                    break;
                case TransferManager.TransferReason.Food:
                    data.m_flags = (data.m_flags & ~Building.Flags.Content04_Forbid) | Building.Flags.Loading1;
                    break;
                case TransferManager.TransferReason.Petroleum:
                    data.m_flags = (data.m_flags & ~Building.Flags.Content05_Forbid) | Building.Flags.Content05;
                    break;
                case TransferManager.TransferReason.Glass:
                    data.m_flags = (data.m_flags & ~Building.Flags.Content06_Forbid) | Building.Flags.Content06;
                    break;
                case TransferManager.TransferReason.Goods:
                    data.m_flags = (data.m_flags & ~Building.Flags.Loading2) | Building.Flags.Content07;
                    break;
                case TransferManager.TransferReason.Lumber:
                    data.m_flags = (data.m_flags & ~Building.Flags.Content07) | Building.Flags.Loading2;
                    break;
                case TransferManager.TransferReason.LuxuryProducts:
                    data.m_flags = (data.m_flags & ~Building.Flags.Content06) | Building.Flags.Content06_Forbid;
                    break;
                case TransferManager.TransferReason.Metals:
                    data.m_flags = (data.m_flags & ~Building.Flags.Content05) | Building.Flags.Content05_Forbid;
                    break;
                case TransferManager.TransferReason.Paper:
                    data.m_flags = (data.m_flags & ~Building.Flags.Loading1) | Building.Flags.Content04_Forbid;
                    break;
                case TransferManager.TransferReason.Petrol:
                    data.m_flags = (data.m_flags & ~Building.Flags.Content03) | Building.Flags.Content03_Forbid;
                    break;
                case TransferManager.TransferReason.Plastics:
                    data.m_flags = (data.m_flags & ~Building.Flags.LevelUpLandValue) | Building.Flags.Content02_Forbid;
                    break;
                case TransferManager.TransferReason.PlanedTimber:
                    data.m_flags = (data.m_flags & ~Building.Flags.LevelUpEducation) | Building.Flags.Content01_Forbid;
                    break;
                default:
                    data.m_flags &= ~Building.Flags.ContentMask;
                    break;
            }
        }

        private void CancelIncomingTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort num = data.m_guestVehicles;
            int num2 = 0;
            while (num != 0)
            {
                ushort nextGuestVehicle = instance.m_vehicles.m_buffer[num].m_nextGuestVehicle;
                if ((TransferManager.TransferReason)instance.m_vehicles.m_buffer[num].m_transferType == material && (instance.m_vehicles.m_buffer[num].m_flags & (Vehicle.Flags.TransferToTarget | Vehicle.Flags.GoingBack)) == Vehicle.Flags.TransferToTarget && instance.m_vehicles.m_buffer[num].m_targetBuilding == buildingID)
                {
                    VehicleInfo info = instance.m_vehicles.m_buffer[num].Info;
                    info.m_vehicleAI.SetTarget(num, ref instance.m_vehicles.m_buffer[num], 0);
                }
                num = nextGuestVehicle;
                if (++num2 > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        public override bool IsFull(ushort buildingID, ref Building data)
        {
            int num = data.m_customBuffer1 * 100;
            return num >= m_storageCapacity;
        }

        public override bool CanBeRelocated(ushort buildingID, ref Building data)
        {
            int num = data.m_customBuffer1 * 100;
            return num == 0;
        }

        public override string GetLocalizedTooltip()
        {
            string text = LocaleFormatter.FormatGeneric("AIINFO_WATER_CONSUMPTION", GetWaterConsumption() * 16) + Environment.NewLine + LocaleFormatter.FormatGeneric("AIINFO_ELECTRICITY_CONSUMPTION", GetElectricityConsumption() * 16);
            string text2 = LocaleFormatter.FormatGeneric("AIINFO_CAPACITY", m_storageCapacity);
            text2 = text2 + Environment.NewLine + LocaleFormatter.FormatGeneric("AIINFO_INDUSTRY_VEHICLE_COUNT", m_truckCount);
            string baseTooltip = TooltipHelper.Append(base.GetLocalizedTooltip(), TooltipHelper.Format(LocaleFormatter.Info1, text, LocaleFormatter.Info2, text2));
            string addTooltip = TooltipHelper.Format("arrowVisible", "false", "input1Visible", "true", "input2Visible", "false", "input3Visible", "false", "input4Visible", "false", "outputVisible", "false");
            string addTooltip2 = TooltipHelper.Format("input1", IndustryWorldInfoPanel.ResourceSpriteName(m_storageType, isStorageBuilding: true), "input2", string.Empty, "input3", string.Empty, "input4", string.Empty, "output", string.Empty);
            baseTooltip = TooltipHelper.Append(baseTooltip, addTooltip);
            return TooltipHelper.Append(baseTooltip, addTooltip2);
        }

        public override string GetLocalizedStats(ushort buildingID, ref Building data)
        {
            string text = string.Empty;
            TransferManager.TransferReason actualTransferReason = GetActualTransferReason(buildingID, ref data);
            if (actualTransferReason != TransferManager.TransferReason.None)
            {
                int budget = Singleton<EconomyManager>.instance.GetBudget(m_info.m_class);
                int productionRate = PlayerBuildingAI.GetProductionRate(100, budget);
                int num = (productionRate * m_truckCount + 99) / 100;
                int count = 0;
                int cargo = 0;
                int capacity = 0;
                int outside = 0;
                CalculateOwnVehicles(buildingID, ref data, actualTransferReason, ref count, ref cargo, ref capacity, ref outside);
                int num2 = data.m_customBuffer1 * 100;
                int num3 = 0;
                if (num2 != 0)
                {
                    num3 = Mathf.Max(1, num2 * 100 / m_storageCapacity);
                }
                text = text + LocaleFormatter.FormatGeneric("AIINFO_FULL", num3) + Environment.NewLine;
                text += LocaleFormatter.FormatGeneric("AIINFO_INDUSTRY_VEHICLES", count, num);
            }
            return text;
        }

        public TransferManager.TransferReason GetTransferReason(ushort buildingID, ref Building data)
        {
            if (m_storageType != TransferManager.TransferReason.None)
            {
                return m_storageType;
            }
            return (TransferManager.TransferReason)data.m_seniors;
        }

        public TransferManager.TransferReason GetActualTransferReason(ushort buildingID, ref Building data)
        {
            if (m_storageType != TransferManager.TransferReason.None)
            {
                return m_storageType;
            }
            return (TransferManager.TransferReason)data.m_adults;
        }

        private int GetMaxLoadSize()
        {
            return 8000;
        }

        //copied from CommonBuildingAI, except for the marked line
        protected new void CalculateOwnVehicles(ushort buildingID, ref Building data, TransferManager.TransferReason material, ref int count, ref int cargo, ref int capacity, ref int outside)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort num = data.m_ownVehicles;
            int num2 = 0;
            while (num != 0)
            {
                //added check for tourist count to distinguish between really own vehicles and transit vehicles
                if ((TransferManager.TransferReason)instance.m_vehicles.m_buffer[num].m_transferType == material && instance.m_vehicles.m_buffer[num].m_touristCount == 1)
                {
                    VehicleInfo info = instance.m_vehicles.m_buffer[num].Info;
                    info.m_vehicleAI.GetSize(num, ref instance.m_vehicles.m_buffer[num], out var size, out var max);
                    cargo += Mathf.Min(size, max);
                    capacity += max;
                    count++;
                    if ((instance.m_vehicles.m_buffer[num].m_flags & (Vehicle.Flags.Importing | Vehicle.Flags.Exporting)) != 0)
                    {
                        outside++;
                    }
                }
                num = instance.m_vehicles.m_buffer[num].m_nextOwnVehicle;
                if (++num2 > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }
    }
}