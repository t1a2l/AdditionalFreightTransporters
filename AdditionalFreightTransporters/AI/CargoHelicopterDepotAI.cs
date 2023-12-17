using System;
using UnityEngine;

namespace AdditionalFreightTransporters.AI
{

    //based of CargoHarborAI but without animals, connections & checking height
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
            float a = this.m_info.m_generatedInfo.m_max.z - 7f;
            if (this.m_info.m_paths != null)
            {
                for (int index1 = 0; index1 < this.m_info.m_paths.Length; ++index1)
                {
                    if (this.m_info.m_paths[index1].m_netInfo != null &&
                        this.m_info.m_paths[index1].m_netInfo.m_class.m_service == ItemClass.Service.Road &&
                        this.m_info.m_paths[index1].m_nodes != null)
                    {
                        for (int index2 = 0; index2 < this.m_info.m_paths[index1].m_nodes.Length; ++index2)
                            a = Mathf.Min(a,
                                -16f - this.m_info.m_paths[index1].m_netInfo.m_halfWidth -
                                this.m_info.m_paths[index1].m_nodes[index2].z);
                    }
                }
            }

            this.m_quayOffset = a;
        }

        public override bool RequireRoadAccess() => true;
    }
}