using AdditionalFreightTransporters.Utils;
using UnityEngine;

namespace AdditionalFreightTransporters
{
    public static class EditedAssetTransformer
    {
        public static void ToBargeHarborFacility()
        {
            var buildingInfo = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            if (buildingInfo?.m_buildingAI is not CargoHarborAI cargoHarborAI)
            {
                Debug.LogWarning("AdditionalFreightTransporters: Current asset is not a building or is not CargoHarborAI");
                return;
            }
            buildingInfo.m_isCustomContent = true;
            buildingInfo.m_class = ItemClasses.cargoFerryFacility;
            cargoHarborAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Ferry");
        }

        public static void ToBargeVehicle() {
            var vehicleInfo = ToolsModifierControl.toolController?.m_editPrefabInfo as VehicleInfo;
            if (vehicleInfo?.m_vehicleAI is not CargoShipAI cargoShipAI)
            {
                Debug.LogWarning("AdditionalFreightTransporters: Current asset is not a vehicle or is not CargoShipAI");
                return;
            }
            vehicleInfo.m_vehicleType = VehicleInfo.VehicleType.Ferry;
            vehicleInfo.m_class = ItemClasses.cargoFerryVehicle;
            vehicleInfo.m_isCustomContent = true;
            cargoShipAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Ferry");
        }

        public static void ToCargoHelicopterFacility()
        {
            var buildingInfo = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            if (buildingInfo?.m_buildingAI is not DepotAI && buildingInfo?.m_buildingAI is not HelicopterDepotAI)
            {
                Debug.LogWarning("AdditionalFreightTransporters: Current asset is not a building or is not DepotAI or is not HelicopterDepotAI");
                return;
            }
            buildingInfo.m_isCustomContent = true;
            buildingInfo.m_class = ItemClasses.cargoHelicopterFacility;
            if (buildingInfo?.m_buildingAI is DepotAI depotAI)
            {
                depotAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Helicopter");
            }
            else if (buildingInfo?.m_buildingAI is HelicopterDepotAI)
            {
                var oldAi = buildingInfo.GetComponent<HelicopterDepotAI>();
                Object.DestroyImmediate(oldAi);
                var ai = buildingInfo.gameObject.AddComponent<DepotAI>();
                PrefabUtil.TryCopyAttributes(oldAi, ai, false);
                ai.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Helicopter");
            }
        }

        public static void ToCargoHelicopterVehicle()
        {
            var vehicleInfo = ToolsModifierControl.toolController?.m_editPrefabInfo as VehicleInfo;
            if (vehicleInfo?.m_vehicleAI is not PassengerHelicopterAI && vehicleInfo?.m_vehicleAI is not PoliceCopterAI
                && vehicleInfo?.m_vehicleAI is not AmbulanceCopterAI && vehicleInfo?.m_vehicleAI is not FireCopterAI
                && vehicleInfo?.m_vehicleAI is not DisasterResponseCopterAI)
            {
                Debug.LogWarning("AdditionalFreightTransporters: Current asset is not a vehicle or is not PassengerHelicopterAI or is not PoliceCopterAI or is not AmbulanceCopterAI or is not FireCopterAI or is not DisasterResponseCopterAI");
                return;
            }
            vehicleInfo.m_vehicleType = VehicleInfo.VehicleType.Helicopter;
            vehicleInfo.m_class = ItemClasses.cargoHelicopterVehicle;
            vehicleInfo.m_isCustomContent = true;
            if (vehicleInfo?.m_vehicleAI is PassengerHelicopterAI passengerHelicopterAI)
            {
                passengerHelicopterAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Helicopter");
            }
            else if (vehicleInfo?.m_vehicleAI is HelicopterAI || vehicleInfo?.m_vehicleAI is PoliceCopterAI || vehicleInfo?.m_vehicleAI is AmbulanceCopterAI || 
                vehicleInfo?.m_vehicleAI is FireCopterAI || vehicleInfo?.m_vehicleAI is DisasterResponseCopterAI)
            {
                var oldAi = vehicleInfo.GetComponent<PrefabAI>();
                Object.DestroyImmediate(oldAi);
                var ai = vehicleInfo.gameObject.AddComponent<PassengerHelicopterAI>();
                PrefabUtil.TryCopyAttributes(oldAi, ai, false);
                ai.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Helicopter");
            }
        }

        public static void ToCargoTramFacility()
        {
            var buildingInfo = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            if (buildingInfo?.m_buildingAI is not DepotAI depotAI)
            {
                Debug.LogWarning("AdditionalFreightTransporters: Current asset is not a building or is not DepotAI");
                return;
            }
            buildingInfo.m_isCustomContent = true;
            buildingInfo.m_class = ItemClasses.cargoTramFacility;
            depotAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Tram");
        }

        public static void ToCargoTramVehicle()
        {
            var vehicleInfo = ToolsModifierControl.toolController?.m_editPrefabInfo as VehicleInfo;
            if (vehicleInfo?.m_vehicleAI is not TramAI tramAI)
            {
                Debug.LogWarning("AdditionalFreightTransporters: Current asset is not a vehicle or is not TramAI");
                return;
            }
            vehicleInfo.m_vehicleType = VehicleInfo.VehicleType.Tram;
            vehicleInfo.m_class = ItemClasses.cargoTramVehicle;
            vehicleInfo.m_isCustomContent = true;
            tramAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Tram");
        }
    }
}