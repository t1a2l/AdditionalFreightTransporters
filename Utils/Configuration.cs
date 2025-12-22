using System.Collections.Generic;
using UnityEngine;

namespace AdditionalFreightTransporters.Utils
{
    public static class Configuration
    {
        private static Dictionary<Vector3, byte> _state;

        public static void Reset()
        {
            _state = null;
        }

        public static bool IsStationInverted(Building station)
        {
            var state = GetState();
            if (!Util.IsCargoStation(station))
            {
                return false;
            }
            if (!state.TryGetValue(station.m_position, out byte flag))
            {
                return false;
            }
            return flag == 1;
        }

        public static void InvertState(Building station)
        {
            if (!Util.IsCargoStation(station))
            {
                return;
            }
            var state = GetState();
            state[station.m_position] = (byte)(IsStationInverted(station) ? 0 : 1);
        }

        public static Dictionary<Vector3, byte> GetState()
        {
            if (_state != null)
            {
                return _state;
            }
            _state = [];
            if (SerializableDataExtension.RawState == null)
            {
                return _state;
            }
            foreach (var pair in SerializableDataExtension.RawState)
            {
                var building = BuildingManager.instance.m_buildings.m_buffer[pair.Key];
                if (!Util.IsCargoStation(building))
                {
                    continue;
                }
                _state[building.m_position] = pair.Value;
            }
            return _state;
        }
    }
}