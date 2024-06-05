using ColossalFramework;
using System;
using UnityEngine;

namespace AdditionalFreightTransporters.AI
{
    //based of CargoHarborAI but without animals, connections & checking height
    public class CargoHelicopterWarehouseDepotAI : CargoHelicopterDepotAI
    {
        public override Color GetColor(ushort buildingID, ref Building data, InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subInfoMode)
        {
            if (infoMode == InfoManager.InfoMode.Transport)
            {
                TransportInfo transportInfo = m_transportInfo;
                if (m_transportInfo2 != null && m_transportInfo2.m_class.m_subService == m_info.m_class.m_subService)
                {
                    transportInfo = m_transportInfo2;
                }
                if ((((data.m_parentBuilding != 0) ? Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_parentBuilding].m_flags : data.m_flags) & Building.Flags.Active) != 0)
                {
                    return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)transportInfo.m_transportType];
                }
                return Singleton<TransportManager>.instance.m_properties.m_inactiveColors[(int)transportInfo.m_transportType];
            }
            if (GetParentWarehouse(ref data, out var warehouseAI))
            {
                return warehouseAI.GetColor(data.m_parentBuilding, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_parentBuilding], infoMode, subInfoMode);
            }
            return base.GetColor(buildingID, ref data, infoMode, subInfoMode);
        }

        public override void CreateBuilding(ushort buildingID, ref Building data)
        {
            base.CreateBuilding(buildingID, ref data);
            data.m_productionRate = 100;
        }

        public override void BuildingLoaded(ushort buildingID, ref Building data, uint version)
        {
            base.BuildingLoaded(buildingID, ref data, version);
            data.m_productionRate = 100;
        }

        public override void SimulationStep(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            Vector3 vector = buildingData.CalculatePosition(m_spawnPosition);
            Vector3 vector2 = vector - buildingData.m_position;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort num = buildingData.m_ownVehicles;
            int num2 = 0;
            while (num != 0)
            {
                Vehicle.Flags flags = instance.m_vehicles.m_buffer[num].m_flags;
                flags &= Vehicle.Flags.Spawned | Vehicle.Flags.Stopped | Vehicle.Flags.WaitingLoading;
                if (flags == (Vehicle.Flags.Spawned | Vehicle.Flags.Stopped | Vehicle.Flags.WaitingLoading))
                {
                    if (instance.m_vehicles.m_buffer[num].Info.m_class.m_subService == m_transportInfo.m_class.m_subService)
                    {
                        Vector3 lastFramePosition = instance.m_vehicles.m_buffer[num].GetLastFramePosition();
                        Vector3 vector3 = lastFramePosition - vector;
                        if (vector3.x * vector2.x + vector3.z * vector2.z < 0f)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag2 = true;
                        }
                    }
                    else
                    {
                        flag3 = true;
                    }
                }
                num = instance.m_vehicles.m_buffer[num].m_nextOwnVehicle;
                if (++num2 > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
            num = buildingData.m_guestVehicles;
            num2 = 0;
            while (num != 0)
            {
                Vehicle.Flags flags2 = instance.m_vehicles.m_buffer[num].m_flags;
                flags2 &= Vehicle.Flags.Spawned | Vehicle.Flags.Stopped | Vehicle.Flags.WaitingLoading;
                if (flags2 == (Vehicle.Flags.Spawned | Vehicle.Flags.WaitingLoading))
                {
                    if (instance.m_vehicles.m_buffer[num].Info.m_class.m_subService == m_transportInfo.m_class.m_subService)
                    {
                        Vector3 lastFramePosition2 = instance.m_vehicles.m_buffer[num].GetLastFramePosition();
                        Vector3 vector4 = lastFramePosition2 - vector;
                        if (vector4.x * vector2.x + vector4.z * vector2.z < 0f)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag2 = true;
                        }
                    }
                    else
                    {
                        flag3 = true;
                    }
                }
                num = instance.m_vehicles.m_buffer[num].m_nextGuestVehicle;
                if (++num2 > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
            if (flag)
            {
                buildingData.m_flags |= Building.Flags.Loading1;
            }
            else
            {
                buildingData.m_flags &= ~Building.Flags.Loading1;
            }
            if (flag2)
            {
                buildingData.m_flags |= Building.Flags.Loading2;
            }
            else
            {
                buildingData.m_flags &= ~Building.Flags.Loading2;
            }
            if (flag3)
            {
                buildingData.m_flags |= Building.Flags.SecondaryLoading;
            }
            else
            {
                buildingData.m_flags &= ~Building.Flags.SecondaryLoading;
            }
            base.SimulationStep(buildingID, ref buildingData, ref frameData);
        }

        public override void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (GetParentWarehouse(ref data, out var warehouseAI) && material == warehouseAI.GetActualTransferReason(data.m_parentBuilding, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_parentBuilding]))
            {
                for (int i = 0; i < offer.Amount; i++)
                {
                    VehicleInfo transferVehicleService = WarehouseAI.GetTransferVehicleService(material, ItemClass.Level.Level1, ref Singleton<SimulationManager>.instance.m_randomizer);
                    if (transferVehicleService is null)
                    {
                        continue;
                    }
                    Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                    if (Singleton<VehicleManager>.instance.CreateVehicle(out var vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, transferVehicleService, data.m_position, material, transferToSource: false, transferToTarget: true))
                    {
                        transferVehicleService.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[vehicle], buildingID);
                        transferVehicleService.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[vehicle], material, offer);
                        ushort building = offer.Building;
                        if (building != 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_flags & Building.Flags.IncomingOutgoing) != 0)
                        {
                            transferVehicleService.m_vehicleAI.GetSize(vehicle, ref vehicles.m_buffer[vehicle], out var size, out var _);
                            ExportResource(data.m_parentBuilding, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_parentBuilding], material, size);
                        }
                        Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_parentBuilding].m_outgoingProblemTimer = 0;
                    }
                }
            }
            else
            {
                base.StartTransfer(buildingID, ref data, material, offer);
            }
        }

        public override void ModifyMaterialBuffer(ushort buildingID, ref Building data, TransferManager.TransferReason material, ref int amountDelta)
        {
            if (GetParentWarehouse(ref data, out var warehouseAI))
            {
                warehouseAI.ModifyMaterialBuffer(data.m_parentBuilding, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_parentBuilding], material, ref amountDelta);
            }
            else
            {
                amountDelta = 0;
            }
        }

        public override bool UseTruckDelivery(ushort buildingAI, ref Building data)
        {
            return false;
        }

        private bool GetParentWarehouse(ref Building data, out WarehouseAI warehouseAI)
        {
            warehouseAI = null;
            if (data.m_parentBuilding != 0)
            {
                BuildingInfo info = Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_parentBuilding].Info;
                if (info != null)
                {
                    warehouseAI = info.m_buildingAI as WarehouseAI;
                }
            }
            return warehouseAI != null;
        }
    }
}
