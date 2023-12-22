using AdditionalFreightTransporters.OptionsFramework;
using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;

namespace AdditionalFreightTransporters.AI
{
    public class CargoTramAI : TramAI
    {
        [CustomizableProperty("Cargo capacity")]
        public int m_cargoCapacity = 1;

        public CargoTramAI()
        {
            m_passengerCapacity = 0;
        }

        public override VehicleInfo.VehicleCategory vehicleCategory => VehicleInfo.VehicleCategory.Tram;

        public override void CreateVehicle(ushort vehicleID, ref Vehicle data)
        {
            base.CreateVehicle(vehicleID, ref data);
            data.m_flags |= Vehicle.Flags.WaitingTarget;
            data.m_flags |= Vehicle.Flags.WaitingCargo;
            data.m_flags |= Vehicle.Flags.WaitingLoading;
            data.m_flags |= Vehicle.Flags.Stopped;
        }

        public override void LoadVehicle(ushort vehicleID, ref Vehicle data)
        {
            base.LoadVehicle(vehicleID, ref data);
            LoadVehicle_TramAI(vehicleID, ref data);
            if (data.m_sourceBuilding != 0)
            {
                Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_sourceBuilding].AddOwnVehicle(vehicleID, ref data);
            }
            if (data.m_targetBuilding == 0)
            {
                return;
            }
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_targetBuilding].AddGuestVehicle(vehicleID, ref data);
        }

        private void LoadVehicle_TramAI(ushort vehicleID, ref Vehicle data)
        {
            if (data.m_path != 0U || (data.m_flags & Vehicle.Flags.WaitingPath) == 0)
            {
                return;
            }
            data.m_flags &= ~Vehicle.Flags.WaitingPath;
        }

        public override void SetSource(ushort vehicleID, ref Vehicle data, ushort sourceBuilding)
        {
            RemoveSource(vehicleID, ref data);
            data.m_sourceBuilding = sourceBuilding;
            if (sourceBuilding == 0)
            {
                return;
            }
            data.Unspawn(vehicleID);
            BuildingManager instance = Singleton<BuildingManager>.instance;
            instance.m_buildings.m_buffer[sourceBuilding].Info.m_buildingAI.CalculateSpawnPosition(sourceBuilding, ref instance.m_buildings.m_buffer[sourceBuilding], ref Singleton<SimulationManager>.instance.m_randomizer, m_info, out Vector3 position, out Vector3 target);
            Quaternion rotation = Quaternion.identity;
            Vector3 forward = target - position;
            if (forward.sqrMagnitude > 0.00999999977648258)
            {
                rotation = Quaternion.LookRotation(forward);
            }
            data.m_frame0 = new Vehicle.Frame(position, rotation);
            data.m_frame1 = data.m_frame0;
            data.m_frame2 = data.m_frame0;
            data.m_frame3 = data.m_frame0;
            data.m_targetPos0 = (Vector4)(position + Vector3.ClampMagnitude(target - position, 0.5f));
            data.m_targetPos0.w = 2f;
            data.m_targetPos1 = data.m_targetPos0;
            data.m_targetPos2 = data.m_targetPos0;
            data.m_targetPos3 = data.m_targetPos0;
            FrameDataUpdated(vehicleID, ref data, ref data.m_frame0);
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[sourceBuilding].AddOwnVehicle(vehicleID, ref data);
            if ((Singleton<BuildingManager>.instance.m_buildings.m_buffer[sourceBuilding].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.None)
            {
                return;
            }
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != 0)
            {
                data.m_flags |= Vehicle.Flags.Importing;
            }
            else
            {
                if ((data.m_flags & Vehicle.Flags.TransferToSource) == 0)
                {
                    return;
                }
                data.m_flags |= Vehicle.Flags.Exporting;
            }
        }

        public override void SetTarget(ushort vehicleID, ref Vehicle data, ushort targetBuilding)
        {
            if (targetBuilding != data.m_targetBuilding)
            {
                RemoveTarget(vehicleID, ref data);
                data.m_targetBuilding = targetBuilding;
                data.m_flags &= ~Vehicle.Flags.WaitingTarget;
                data.m_waitCounter = 0;
                if (targetBuilding != 0)
                {
                    Singleton<BuildingManager>.instance.m_buildings.m_buffer[targetBuilding].AddGuestVehicle(vehicleID, ref data);
                    if ((Singleton<BuildingManager>.instance.m_buildings.m_buffer[targetBuilding].m_flags & Building.Flags.IncomingOutgoing) != Building.Flags.None)
                    {
                        if ((data.m_flags & Vehicle.Flags.TransferToTarget) != 0)
                        {
                            data.m_flags |= Vehicle.Flags.Exporting;
                        }
                        else if ((data.m_flags & Vehicle.Flags.TransferToSource) != 0)
                        {
                            data.m_flags |= Vehicle.Flags.Importing;
                        }
                    }
                }
                else
                {
                    data.m_flags |= Vehicle.Flags.GoingBack;
                } 
            }
            if ((data.m_flags & Vehicle.Flags.WaitingCargo) != 0 || StartPathFind(vehicleID, ref data))
            {
                return;
            }
            data.Unspawn(vehicleID);
        }

        private void RemoveTarget(ushort vehicleID, ref Vehicle data)
        {
            if (data.m_targetBuilding == 0)
            {
                return;
            }
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)data.m_targetBuilding].RemoveGuestVehicle(vehicleID, ref data);
            data.m_targetBuilding = 0;
        }

        public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
        {
            if ((data.m_flags & Vehicle.Flags.WaitingCargo) != 0)
            {
                bool flag = Singleton<SimulationManager>.instance.m_randomizer.Int32(2U) == 0;
                if (!flag && data.m_sourceBuilding != 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_sourceBuilding].m_flags & Building.Flags.Active) == Building.Flags.None)
                {
                    flag = true;
                }
                if (!flag && data.m_targetBuilding != 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_targetBuilding].m_flags & Building.Flags.Active) == Building.Flags.None)
                {
                    flag = true;
                }
                if (!flag)
                {
                    data.m_waitCounter = data.m_transferSize < m_cargoCapacity ? (byte)Mathf.Min(data.m_waitCounter + 1, byte.MaxValue) : byte.MaxValue;
                    if (data.m_waitCounter == byte.MaxValue && ((data.m_flags & Vehicle.Flags.Spawned) != 0) || CanSpawnAt(data.GetLastFramePosition()))
                    {
                        data.m_flags &= ~Vehicle.Flags.WaitingCargo;
                        data.m_flags |= Vehicle.Flags.Leaving;
                        data.m_waitCounter = 0;
                        StartPathFind(vehicleID, ref data);
                    }
                }
            }
            else if ((data.m_flags & Vehicle.Flags.Stopped) != 0)
            {
                if ((data.m_flags & Vehicle.Flags.Spawned) != 0 && ++data.m_waitCounter == 16)
                {
                    data.m_flags &= ~(Vehicle.Flags.Stopped | Vehicle.Flags.WaitingLoading);
                    data.m_flags |= Vehicle.Flags.Leaving;
                    data.m_waitCounter = 0;
                }
            }
            else if ((data.m_flags & Vehicle.Flags.GoingBack) == 0)
            {
                SetTarget(vehicleID, ref data, 0);
            }
            base.SimulationStep(vehicleID, ref data, physicsLodRefPos);
        }

        public override void UpdateBuildingTargetPositions(
          ushort vehicleID,
          ref Vehicle vehicleData,
          Vector3 refPos,
          ushort leaderID,
          ref Vehicle leaderData,
          ref int index,
          float minSqrDistance)
        {
            //do nothing
        }

        public override void ReleaseVehicle(ushort vehicleID, ref Vehicle data)
        {
            RemoveSource(vehicleID, ref data);
            RemoveTarget(vehicleID, ref data);
            base.ReleaseVehicle(vehicleID, ref data);
        }

        private void RemoveSource(ushort vehicleID, ref Vehicle data)
        {
            if (data.m_sourceBuilding == 0)
            {
                return;
            }
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_sourceBuilding].RemoveOwnVehicle(vehicleID, ref data);
            data.m_sourceBuilding = 0;
        }

        public override bool ArriveAtDestination(ushort vehicleID, ref Vehicle vehicleData)
        {
            if ((vehicleData.m_flags & Vehicle.Flags.WaitingTarget) != 0)
            {
                return false;
            }

            if ((vehicleData.m_flags & Vehicle.Flags.WaitingLoading) != 0)
            {
                vehicleData.m_waitCounter = (byte)Mathf.Min(vehicleData.m_waitCounter + 1, byte.MaxValue);
                if (vehicleData.m_waitCounter < 16)
                {
                    return false;
                }
                if (vehicleData.m_targetBuilding != 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[vehicleData.m_targetBuilding].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.None)
                {
                    ushort nextCargoParent = CargoTruckAI.FindNextCargoParent(vehicleData.m_targetBuilding, m_info.m_class.m_service, m_info.m_class.m_subService);
                    if (nextCargoParent != 0)
                    {
                        ushort targetBuilding = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[nextCargoParent].m_targetBuilding;
                        if (targetBuilding != 0)
                        {
                            CargoTruckAI.SwitchCargoParent(nextCargoParent, vehicleID);
                            vehicleData.m_waitCounter = 0;
                            vehicleData.m_flags &= ~Vehicle.Flags.WaitingLoading;
                            SetTarget(vehicleID, ref vehicleData, targetBuilding);
                            return (vehicleData.m_flags & Vehicle.Flags.Spawned) == 0;
                        }
                    }
                }
                Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
                return true;
            }
            return (vehicleData.m_flags & Vehicle.Flags.GoingBack) != 0 ? ArriveAtSource(vehicleID, ref vehicleData) : ArriveAtTarget(vehicleID, ref vehicleData);
        }

        public override void GetSize(ushort vehicleID, ref Vehicle data, out int size, out int max)
        {
            size = data.m_transferSize;
            max = m_cargoCapacity;
        }

        private bool ArriveAtSource(ushort vehicleID, ref Vehicle data)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort vehicle = data.m_firstCargo;
            data.m_firstCargo = 0;
            int num = 0;
            while (vehicle != 0)
            {
                ushort nextCargo = instance.m_vehicles.m_buffer[vehicle].m_nextCargo;
                instance.m_vehicles.m_buffer[vehicle].m_nextCargo = 0;
                instance.m_vehicles.m_buffer[vehicle].m_cargoParent = 0;
                instance.ReleaseVehicle(vehicle);
                vehicle = nextCargo;
                if (++num > AdditionalFreightTransportersMod.MaxVehicleCount)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
                    break;
                }
            }
            data.m_waitCounter = 0;
            data.m_flags |= Vehicle.Flags.WaitingLoading;
            return false;
        }

        private bool ArriveAtTarget(ushort vehicleID, ref Vehicle data)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort vehicleID1 = data.m_firstCargo;
            data.m_firstCargo = 0;
            int num = 0;
            while (vehicleID1 != 0)
            {
                ushort nextCargo = instance.m_vehicles.m_buffer[vehicleID1].m_nextCargo;
                instance.m_vehicles.m_buffer[vehicleID1].m_nextCargo = 0;
                instance.m_vehicles.m_buffer[vehicleID1].m_cargoParent = 0;
                VehicleInfo info = instance.m_vehicles.m_buffer[vehicleID1].Info;
                if (data.m_targetBuilding != 0)
                {
                    if (data.m_targetBuilding == instance.m_vehicles.m_buffer[vehicleID1].m_targetBuilding
                        && OptionsWrapper<Options>.Options.EnableWarehouseAI)
                    {
                        info.m_vehicleAI.ArriveAtDestination(vehicleID1, ref instance.m_vehicles.m_buffer[vehicleID1]);
                        instance.ReleaseVehicle(vehicleID1);
                    }
                    else
                    {
                        if (OptionsWrapper<Options>.Options.EnableWarehouseAI && info.m_vehicleAI is CargoTruckAI cargoTruckAI)
                        {
                            //we compensate the removal that will happen in SetSource() of CargoTruckAI
                            var amountDelta = -Mathf.Min(0, instance.m_vehicles.m_buffer[vehicleID1].m_transferSize - cargoTruckAI.m_cargoCapacity);
                            var buildingAI = BuildingManager.instance.m_buildings.m_buffer[data.m_targetBuilding].Info.m_buildingAI;
                            buildingAI.ModifyMaterialBuffer(data.m_targetBuilding, ref BuildingManager.instance.m_buildings.m_buffer[data.m_targetBuilding], (TransferManager.TransferReason)instance.m_vehicles.m_buffer[vehicleID1].m_transferType, ref amountDelta);
                        }
                        info.m_vehicleAI.SetSource(vehicleID1, ref instance.m_vehicles.m_buffer[vehicleID1], data.m_targetBuilding);
                        info.m_vehicleAI.SetTarget(vehicleID1, ref instance.m_vehicles.m_buffer[vehicleID1], instance.m_vehicles.m_buffer[vehicleID1].m_targetBuilding);
                    }
                }
                vehicleID1 = nextCargo;
                if (++num > AdditionalFreightTransportersMod.MaxVehicleCount)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
                    break;
                }
            }
            data.m_waitCounter = 0;
            data.m_flags |= Vehicle.Flags.WaitingLoading;
            return false;
        }

        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData)
        {
            if (vehicleData.m_leadingVehicle == 0)
            {
                Vector3 startPos = (vehicleData.m_flags & Vehicle.Flags.Reversed) == 0 ? (Vector3)vehicleData.m_targetPos0 : (Vector3)Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicleData.GetLastVehicle(vehicleID)].m_targetPos0;
                if ((vehicleData.m_flags & Vehicle.Flags.GoingBack) != 0)
                {
                    if (vehicleData.m_sourceBuilding != 0)
                    {
                        BuildingManager instance = Singleton<BuildingManager>.instance;
                        BuildingInfo info = instance.m_buildings.m_buffer[vehicleData.m_sourceBuilding].Info;
                        Randomizer randomizer = new(vehicleID);
                        info.m_buildingAI.CalculateSpawnPosition(vehicleData.m_sourceBuilding, ref instance.m_buildings.m_buffer[vehicleData.m_sourceBuilding], ref randomizer, m_info, out Vector3 position, out Vector3 _);
                        var startPathFind = this.StartPathFind(vehicleID, ref vehicleData, startPos, position);
                        return startPathFind;
                    }
                }
                else if (vehicleData.m_targetBuilding != 0)
                {
                    BuildingManager instance = Singleton<BuildingManager>.instance;
                    BuildingInfo info = instance.m_buildings.m_buffer[vehicleData.m_targetBuilding].Info;
                    Randomizer randomizer = new(vehicleID);
                    info.m_buildingAI.CalculateSpawnPosition(vehicleData.m_targetBuilding, ref instance.m_buildings.m_buffer[vehicleData.m_targetBuilding], ref randomizer, m_info, out Vector3 position, out Vector3 _);
                    var startPathFind = StartPathFind(vehicleID, ref vehicleData, startPos, position);
                    return startPathFind;
                }
            }
            return false;
        }

        public override void BuildingRelocated(ushort vehicleID, ref Vehicle data, ushort building)
        {
            base.BuildingRelocated(vehicleID, ref data, building);
            if (building == data.m_sourceBuilding)
            {
                if ((data.m_flags & Vehicle.Flags.GoingBack) == 0)
                {
                    return;
                }
                InvalidPath(vehicleID, ref data, vehicleID, ref data);
            }
            else
            {
                if (building != data.m_targetBuilding || (data.m_flags & Vehicle.Flags.GoingBack) != 0)
                {
                    return;
                }
                InvalidPath(vehicleID, ref data, vehicleID, ref data);
            }
        }

        //UI stuff

        public override Color GetColor(ushort vehicleID, ref Vehicle data, InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subInfoMode)
        {
            switch (infoMode)
            {
                case InfoManager.InfoMode.Transport:
                    return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)m_transportInfo.m_transportType];
                case InfoManager.InfoMode.Connections:
                    TransferManager.TransferReason transferType = (TransferManager.TransferReason)data.m_transferType;
                    if (subInfoMode == InfoManager.SubInfoMode.Default && (data.m_flags & Vehicle.Flags.Importing) != 0 && transferType != TransferManager.TransferReason.None)
                        return Singleton<TransferManager>.instance.m_properties.m_resourceColors[(int)transferType];
                    return subInfoMode == InfoManager.SubInfoMode.WaterPower && (data.m_flags & Vehicle.Flags.Exporting) != 0 && transferType != TransferManager.TransferReason.None ? Singleton<TransferManager>.instance.m_properties.m_resourceColors[(int)transferType] : Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                case InfoManager.InfoMode.TrafficRoutes:
                    if (subInfoMode == InfoManager.SubInfoMode.Default)
                    {
                        InstanceID empty = InstanceID.Empty;
                        empty.Vehicle = vehicleID;
                        return Singleton<NetManager>.instance.PathVisualizer.IsPathVisible(empty) ? Singleton<InfoManager>.instance.m_properties.m_routeColors[3] : Singleton<InfoManager>.instance.m_properties.m_neutralColor;
                    }
                    break;
            }
            return base.GetColor(vehicleID, ref data, infoMode, subInfoMode);
        }

        public override string GetLocalizedStatus(ushort vehicleID, ref Vehicle data, out InstanceID target)
        {
            if ((data.m_flags & Vehicle.Flags.WaitingCargo) != 0)
            {
                target = InstanceID.Empty;
                return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOSHIP_LOADING");
            }
            if ((data.m_flags & Vehicle.Flags.GoingBack) != 0)
            {
                target = InstanceID.Empty;
                return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOTRUCK_RETURN");
            }
            if (data.m_targetBuilding != 0)
            {
                target = InstanceID.Empty;
                target.Building = data.m_targetBuilding;
                return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOSHIP_TRANSPORT");
            }
            target = InstanceID.Empty;
            return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CONFUSED");
        }

        public override string GetLocalizedStatus(
          ushort parkedVehicleID,
          ref VehicleParked data,
          out InstanceID target)
        {
            target = InstanceID.Empty;
            return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOSHIP_LOADING");
        }

        public override void GetBufferStatus(
          ushort vehicleID,
          ref Vehicle data,
          out string localeKey,
          out int current,
          out int max)
        {
            localeKey = "Default";
            current = 0;
            max = m_cargoCapacity;
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort num1 = data.m_firstCargo;
            int num2 = 0;
            while (num1 != 0)
            {
                ++current;
                num1 = instance.m_vehicles.m_buffer[num1].m_nextCargo;
                if (++num2 > AdditionalFreightTransportersMod.MaxVehicleCount)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
                    break;
                }
            }
            if ((data.m_flags & Vehicle.Flags.DummyTraffic) == 0)
            {
                return;
            }
            Randomizer randomizer = new(vehicleID);
            current = randomizer.Int32(max >> 1, max);
        }

        public override InstanceID GetTargetID(ushort vehicleID, ref Vehicle vehicleData)
        {
            InstanceID result = default;
            result.Building = vehicleData.m_targetBuilding;
            return result;
        }
    }
}