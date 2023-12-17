using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AdditionalFreightTransporters
{
    public static class ItemClasses
    {
        public static readonly ItemClass cargoFerryFacility = CreateFerryItemClass("Ferry Cargo Facility");
        public static readonly ItemClass cargoFerryVehicle = CreateFerryItemClass("Ferry Cargo Vehicle");

        public static readonly ItemClass cargoHelicopterFacility = CreateHelicopterItemClass("Helicopter Cargo Facility");
        public static readonly ItemClass cargoHelicopterVehicle = CreateHelicopterItemClass("Helicopter Cargo Vehicle");

        public static readonly ItemClass cargoTramFacility = CreateTramItemClass("Tram Cargo Facility");
        public static readonly ItemClass cargoTramVehicle = CreateTramItemClass("Tram Cargo Vehicle");

        public static void Register()
        {
            var dictionary = ((Dictionary<string, ItemClass>)typeof(ItemClassCollection).GetField("m_classDict", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
            if (!dictionary.ContainsKey(cargoFerryFacility.name))
            {
                dictionary.Add(cargoFerryFacility.name, cargoFerryFacility);
            }
            if (!dictionary.ContainsKey(cargoFerryVehicle.name))
            {
                dictionary.Add(cargoFerryVehicle.name, cargoFerryVehicle);
            }
            if (!dictionary.ContainsKey(cargoHelicopterFacility.name))
            {
                dictionary.Add(cargoHelicopterFacility.name, cargoHelicopterFacility);
            }
            if (!dictionary.ContainsKey(cargoHelicopterVehicle.name))
            {
                dictionary.Add(cargoHelicopterVehicle.name, cargoHelicopterVehicle);
            }
            if (!dictionary.ContainsKey(cargoTramFacility.name))
            {
                dictionary.Add(cargoTramFacility.name, cargoTramFacility);
            }
            if (!dictionary.ContainsKey(cargoTramVehicle.name))
            {
                dictionary.Add(cargoTramVehicle.name, cargoTramVehicle);
            }
        }

        public static void Unregister()
        {
            var dictionary = ((Dictionary<string, ItemClass>)typeof(ItemClassCollection).GetField("m_classDict", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
            dictionary.Remove(cargoFerryFacility.name);
            dictionary.Remove(cargoFerryVehicle.name);
            dictionary.Remove(cargoHelicopterFacility.name);
            dictionary.Remove(cargoHelicopterVehicle.name);
            dictionary.Remove(cargoTramFacility.name);
            dictionary.Remove(cargoTramVehicle.name);
        }
        
        private static ItemClass CreateFerryItemClass(string name)
        {
            var createInstance = ScriptableObject.CreateInstance<ItemClass>();
            createInstance.name = name;
            createInstance.m_level = ItemClass.Level.Level5;
            createInstance.m_service = ItemClass.Service.PublicTransport;
            createInstance.m_subService = ItemClass.SubService.PublicTransportShip;
            return createInstance;
        }

        private static ItemClass CreateHelicopterItemClass(string name)
        {
            var createInstance = ScriptableObject.CreateInstance<ItemClass>();
            createInstance.name = name;
            createInstance.m_level = ItemClass.Level.Level5;
            createInstance.m_service = ItemClass.Service.PublicTransport;
            createInstance.m_subService = ItemClass.SubService.PublicTransportPlane;
            return createInstance;
        }

        private static ItemClass CreateTramItemClass(string name)
        {
            var createInstance = ScriptableObject.CreateInstance<ItemClass>();
            createInstance.name = name;
            createInstance.m_level = ItemClass.Level.Level5;
            createInstance.m_service = ItemClass.Service.PublicTransport;
            createInstance.m_subService = ItemClass.SubService.PublicTransportTram;
            return createInstance;
        }
    }
}