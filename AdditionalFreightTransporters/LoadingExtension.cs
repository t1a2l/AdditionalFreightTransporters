using AdditionalFreightTransporters.OptionsFramework;
using CitiesHarmony.API;
using ICities;
using UnityEngine;
using Util = AdditionalFreightTransporters.Utils.Util;

namespace AdditionalFreightTransporters
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static GameObject _gameObject;
        
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
            if (Util.IsModActive(1764208250))
            {
                Debug.LogWarning("AdditionalFreightTransporters: More Vehicles is enabled, applying compatibility workaround");
                Mod.MaxVehicleCount = ushort.MaxValue + 1;
            }
            else
            {
                Debug.Log("AdditionalFreightTransporters: More Vehicles is not enabled");
                Mod.MaxVehicleCount = VehicleManager.MAX_VEHICLE_COUNT;
            }
            if (Util.IsModActive("Vehicle Selector"))
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
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (_gameObject == null)
            {
                return;
            }
            Object.Destroy(_gameObject);
            _gameObject = null;
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