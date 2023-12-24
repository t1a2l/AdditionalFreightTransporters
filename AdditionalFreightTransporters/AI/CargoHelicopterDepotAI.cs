using System;
using UnityEngine;

namespace AdditionalFreightTransporters.AI
{

    //based of CargoStationAI but without animals, connections & checking height
    public class CargoHelicopterDepotAI : CargoStationAI
    {
        [NonSerialized] protected float m_quayOffset;

        public static bool IsCargoHelicopterDepot(Building station)
        {
            if (station.m_flags == Building.Flags.None)
            {
                return false;
            }
            if (station.Info == null)
            {
                return false;
            }
            return station.Info.m_buildingAI is CargoHelicopterDepotAI;
        }

        public override void InitializePrefab()
        {
            base.InitializePrefab();
            float a = m_info.m_generatedInfo.m_max.z - 7f;
            if (m_info.m_paths != null)
            {
                for (int index1 = 0; index1 < m_info.m_paths.Length; ++index1)
                {
                    if (m_info.m_paths[index1].m_netInfo != null && m_info.m_paths[index1].m_netInfo.m_class.m_service == ItemClass.Service.Road && m_info.m_paths[index1].m_nodes != null)
                    {
                        for (int index2 = 0; index2 < m_info.m_paths[index1].m_nodes.Length; ++index2)
                        {
                            a = Mathf.Min(a, -16f - m_info.m_paths[index1].m_netInfo.m_halfWidth - m_info.m_paths[index1].m_nodes[index2].z);
                        }
                    }
                }
            }
            m_quayOffset = a;
        }

        public override bool RequireRoadAccess() => true;
    }
}