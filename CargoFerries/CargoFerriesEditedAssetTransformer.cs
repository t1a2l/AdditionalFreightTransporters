using UnityEngine;

namespace CargoFerries
{
    public static class CargoFerriesEditedAssetTransformer
    {
        public static void ToBargeHarbor()
        {
            var buildingInfo = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            if (buildingInfo?.m_buildingAI is not CargoHarborAI cargoHarborAI)
            {
                Debug.LogWarning("Barges: Current asset is not a building or is not CargoHarborAI");
                return;
            }
            buildingInfo.m_isCustomContent = true;
            buildingInfo.m_class = ItemClasses.cargoFerryFacility;
            cargoHarborAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Ferry");
        }

        public static void ToBarge() {
            var vehicleInfo = ToolsModifierControl.toolController?.m_editPrefabInfo as VehicleInfo;
            if (vehicleInfo?.m_vehicleAI is not CargoShipAI cargoShipAI)
            {
                Debug.LogWarning("Barges: Current asset is not a vehicle or is not CargoShipAI");
                return;
            }
            vehicleInfo.m_vehicleType = VehicleInfo.VehicleType.Ferry;
            vehicleInfo.m_class = ItemClasses.cargoFerryVehicle;
            vehicleInfo.m_isCustomContent = true;
            cargoShipAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Ferry");
        }

        public static void ToCargoHelicopterDepot()
        {
            var buildingInfo = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            if (buildingInfo?.m_buildingAI is not DepotAI depotAI)
            {
                Debug.LogWarning("CargoHelicopter: Current asset is not a building or is not DepotAI or is not HelicopterDepotAI");
                return;
            }
            buildingInfo.m_isCustomContent = true;
            buildingInfo.m_class = ItemClasses.cargoHelicopterFacility;
            depotAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Helicopter");
        }

        public static void ToCargoHelicopter()
        {
            var vehicleInfo = ToolsModifierControl.toolController?.m_editPrefabInfo as VehicleInfo;
            if (vehicleInfo?.m_vehicleAI is not PassengerHelicopterAI passengerHelicopterAI)
            {
                Debug.LogWarning("CargoHelicopter: Current asset is not a vehicle or is not PassengerHelicopterAI");
                return;
            }
            vehicleInfo.m_vehicleType = VehicleInfo.VehicleType.Helicopter;
            vehicleInfo.m_class = ItemClasses.cargoHelicopterVehicle;
            vehicleInfo.m_isCustomContent = true;
            passengerHelicopterAI.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Helicopter");
        }
    }
}