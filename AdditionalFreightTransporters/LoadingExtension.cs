using AdditionalFreightTransporters.OptionsFramework;
using CitiesHarmony.API;
using ICities;
using UnityEngine;

namespace AdditionalFreightTransporters
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static GameObject _gameObject;

        private static GameObject _gameObject2;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            ItemClasses.Register();
            if (loading.currentMode != AppMode.Game)
            {
                return;
            }
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                return;
            }
            if (Utils.Util.IsModActive(1764208250))
            {
                Debug.LogWarning("AdditionalFreightTransporters: More Vehicles is enabled, applying compatibility workaround");
                Mod.MaxVehicleCount = ushort.MaxValue + 1;
            }
            else
            {
                Debug.Log("AdditionalFreightTransporters: More Vehicles is not enabled");
                Mod.MaxVehicleCount = VehicleManager.MAX_VEHICLE_COUNT;
            }
            if (Utils.Util.IsModActive("Vehicle Selector"))
            {
                Debug.Log("AdditionalFreightTransporters: Vehicle Selector is detected! CargoTruckAI.ChangeVehicleType() won't be patched");
            } 
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (!OptionsWrapper<Options>.Options.EnableWarehouseAI)
            {
                return;
            }
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                return;
            }
            if (_gameObject != null)
            {
                return;
            }
            _gameObject = new GameObject("AdditionalFreightTransporters");
            _gameObject2 = new GameObject("SpawnPositionInverterPanelExtender");
            _gameObject2.AddComponent<GamePanelExtender>();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            Configuration.Reset();
            if (_gameObject != null)
            {
                Object.Destroy(_gameObject);
                _gameObject = null;
            }
            if (_gameObject2 != null)
            {
                Object.Destroy(_gameObject2);
                _gameObject2 = null;
            }
        } 

        public override void OnReleased()
        {
            base.OnReleased();
            ItemClasses.Unregister();
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                return;
            }
        }
    }
}