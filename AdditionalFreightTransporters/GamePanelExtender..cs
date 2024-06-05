using System;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace AdditionalFreightTransporters
{
    public class GamePanelExtender : MonoBehaviour
    {
        private bool _initialized;
        private CityServiceWorldInfoPanel _cityServiceInfoPanel;
        private WarehouseWorldInfoPanel _warehouseWorldInfoPanel;
        private UISprite _swapSprite;
        private UILabel _swapLabel;
        private UISprite _swapSprite1;
        private UILabel _swapLabel1;

        public void OnDestroy()
        {
            if (_swapSprite != null)
            {
                if (_cityServiceInfoPanel != null)
                {
                    _cityServiceInfoPanel.component.RemoveUIComponent(_swapSprite);
                }
                Destroy(_swapSprite.gameObject);
                _swapSprite = null;
            }
            if (_swapSprite1 != null)
            {
                if (_warehouseWorldInfoPanel != null)
                {
                    _warehouseWorldInfoPanel.component.RemoveUIComponent(_swapSprite1);
                }
                Destroy(_swapSprite1.gameObject);
                _swapSprite1 = null;
            }
            if (_swapLabel != null)
            {
                if (_cityServiceInfoPanel != null)
                {
                    _cityServiceInfoPanel.component.RemoveUIComponent(_swapLabel);
                }
                Destroy(_swapLabel.gameObject);
                _swapLabel = null;
            }
            if (_swapLabel1 != null)
            {
                if (_warehouseWorldInfoPanel != null)
                {
                    _warehouseWorldInfoPanel.component.RemoveUIComponent(_swapLabel1);
                }
                Destroy(_swapLabel1.gameObject);
                _swapLabel1 = null;
            }
            _initialized = false;
        }

        public void Update()
        {
            if (!_initialized)
            {
                var go = GameObject.Find("(Library) CityServiceWorldInfoPanel");
                if (go != null)
                {
                    var infoPanel = go.GetComponent<CityServiceWorldInfoPanel>();
                    if (infoPanel != null)
                    {
                        _cityServiceInfoPanel = infoPanel;
                        _swapSprite = Utils.UiUtil.CreateSwapSptite(_cityServiceInfoPanel.component, CityServiceSwapHandler, new Vector3(162, 240));
                        _swapLabel = Utils.UiUtil.CreateLabel("Swap spawn and unspawn positions", _cityServiceInfoPanel.component, new Vector3(178, 240));
                    }
                }
                var go1 = GameObject.Find("(Library) WarehouseWorldInfoPanel");
                if (go1 != null)
                {
                    var infoPanel1 = go1.GetComponent<WarehouseWorldInfoPanel>();
                    if (infoPanel1 != null)
                    {
                        _warehouseWorldInfoPanel = infoPanel1;
                        _swapSprite1 = Utils.UiUtil.CreateSwapSptite(_warehouseWorldInfoPanel.component, WarehouseSwapHandler, new Vector3(162, 240));
                        _swapLabel1 = Utils.UiUtil.CreateLabel("Swap spawn and unspawn positions", _warehouseWorldInfoPanel.component, new Vector3(178, 240));
                        _swapSprite1.relativePosition = new Vector3(130, 550);
                        _swapLabel1.relativePosition = new Vector3(148, 550);
                    }
                }  
                if(_cityServiceInfoPanel != null || _warehouseWorldInfoPanel != null)
                {
                    _initialized = true;
                }
            }
            if (_cityServiceInfoPanel != null && _cityServiceInfoPanel.component.isVisible)
            {
                SetUpCityServiceSwapButton();
            }
            if (_warehouseWorldInfoPanel != null && _warehouseWorldInfoPanel.component.isVisible)
            {
                SetUpWarehouseSwapButton();
            }
        }

        private void SetUpCityServiceSwapButton()
        {
            var instance = (InstanceID)Utils.Util.GetInstanceField(typeof(CityServiceWorldInfoPanel), _cityServiceInfoPanel, "m_InstanceID");
            var id = instance.Building;
            var data = Singleton<BuildingManager>.instance.m_buildings.m_buffer[id];
            var isCargoStation = Utils.Util.IsCargoStation(data);
            _swapSprite.isVisible = isCargoStation;
            _swapLabel.isVisible = isCargoStation;
            if (isCargoStation)
            {
                _swapSprite.spriteName = Configuration.IsStationInverted(data) ? "check-checked" : "check-unchecked";
            }
        }

        private void SetUpWarehouseSwapButton()
        {
            var instance = (InstanceID)Utils.Util.GetInstanceField(typeof(WarehouseWorldInfoPanel), _warehouseWorldInfoPanel, "m_InstanceID");
            var id = instance.Building;
            var data = Singleton<BuildingManager>.instance.m_buildings.m_buffer[id];
            var isCargoStation = Utils.Util.IsCargoStation(data);
            _swapSprite1.isVisible = isCargoStation;
            _swapLabel1.isVisible = isCargoStation;
            if (isCargoStation)
            {
                _swapSprite1.spriteName = Configuration.IsStationInverted(data) ? "check-checked" : "check-unchecked";
            }
        }

        private void CityServiceBuildingHandler(Action<ushort> action)
        {
            var instance = (InstanceID)Utils.Util.GetInstanceField(typeof(ZonedBuildingWorldInfoPanel), _cityServiceInfoPanel, "m_InstanceID");
            var id = instance.Building;
            Singleton<SimulationManager>.instance.AddAction(() =>
            {
                action.Invoke(id);
            });
        }

        private void WarehouseBuildingHandler(Action<ushort> action)
        {
            var instance = (InstanceID)Utils.Util.GetInstanceField(typeof(ZonedBuildingWorldInfoPanel), _warehouseWorldInfoPanel, "m_InstanceID");
            var id = instance.Building;
            Singleton<SimulationManager>.instance.AddAction(() =>
            {
                action.Invoke(id);
            });
        }

        private void CityServiceSwapHandler(UIComponent component, UIMouseEventParameter param)
        {
            CityServiceBuildingHandler(SwapBuildingPositions);
        }

        private void WarehouseSwapHandler(UIComponent component, UIMouseEventParameter param)
        {
            WarehouseBuildingHandler(SwapBuildingPositions);
        }

        private static void SwapBuildingPositions(ushort id)
        {
            Configuration.InvertState(Singleton<BuildingManager>.instance.m_buildings.m_buffer[id]);
        }
    }

}