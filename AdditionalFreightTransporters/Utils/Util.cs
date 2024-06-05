using System;
using System.Linq;
using System.Reflection;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;

namespace AdditionalFreightTransporters.Utils
{
    public static class Util
    {
        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                           | BindingFlags.Static;
            var field = type.GetField(fieldName, bindFlags);
            if (field == null)
            {
                throw new Exception($"Type '{type}' doesn't have field '{fieldName}");
            }
            return field.GetValue(instance);
        }
        
        public static bool IsModActive(string modNamePart)
        {
            try
            {
                var plugins = PluginManager.instance.GetPluginsInfo();
                return (from plugin in plugins.Where(p => p.isEnabled)
                    select plugin.GetInstances<IUserMod>()
                    into instances
                    where instances.Any()
                    select instances[0].Name
                    into name
                    where name != null && name.Contains(modNamePart)
                    select name).Any();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to detect if mod with name containing {modNamePart} is active");
                Debug.LogException(e);
                return false;
            }
        }

        public static bool IsModActive(ulong modId)
        {
            try
            {
                var plugins = PluginManager.instance.GetPluginsInfo();
                return (from plugin in plugins.Where(p => p.isEnabled)
                    select plugin.publishedFileID
                    into workshopId
                    where workshopId.AsUInt64 == modId
                    select workshopId).Any();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to detect if mod {modId} is active");
                Debug.LogException(e);
                return false;
            }
        }

        public static bool IsCargoStation(Building station)
        {
            if (station.m_flags == Building.Flags.None)
            {
                return false;
            }
            if (station.Info == null)
            {
                return false;
            }
            return station.Info.m_buildingAI is CargoStationAI;
        }

        public static ushort StationBuildingIdByPosition(Vector3 position)
        {
            for (ushort i = 0; i < BuildingManager.instance.m_buildings.m_size; i++)
            {
                var building = BuildingManager.instance.m_buildings.m_buffer[i];
                if (!IsCargoStation(building))
                {
                    continue;
                }
                if (position.Equals(building.m_position))
                {
                    return i;
                }
            }
            throw new Exception(string.Format("No building was found for position {0}", position));
        }
    }
}