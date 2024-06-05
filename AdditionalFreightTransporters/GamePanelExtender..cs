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
        private UISprite _swapSprite;
        private UILabel _swapLabel;

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
            if (_swapLabel != null)
            {
                if (_cityServiceInfoPanel != null)
                {
                    _cityServiceInfoPanel.component.RemoveUIComponent(_swapLabel);
                }
                Destroy(_swapLabel.gameObject);
                _swapLabel = null;
            }
            _initialized = false;
        }

        public void Update()
        {

            if (!_initialized)
            {
                var go = GameObject.Find("(Library) CityServiceWorldInfoPanel");
                if (go == null)
                {
                    return;
                }
                var infoPanel = go.GetComponent<CityServiceWorldInfoPanel>();
                if (infoPanel == null)
                {
                    return;
                }
                _cityServiceInfoPanel = infoPanel;
                _swapSprite = Utils.UiUtil.CreateSwapSptite(_cityServiceInfoPanel.component, SwapHandler, new Vector3(162, 240));
                _swapLabel = Utils.UiUtil.CreateLabel("Swap spawn and unspawn positions", _cityServiceInfoPanel.component, new Vector3(178, 240));
                _initialized = true;
            }
            if (!_cityServiceInfoPanel.component.isVisible)
            {
                return;
            }
            SetUpSwapButton();
        }

        private void SetUpSwapButton()
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

        private void BuildingHandler(Action<ushort> action)
        {
            var instance = (InstanceID)Utils.Util.GetInstanceField(typeof(ZonedBuildingWorldInfoPanel), _cityServiceInfoPanel, "m_InstanceID");
            var id = instance.Building;
            Singleton<SimulationManager>.instance.AddAction(() =>
            {
                action.Invoke(id);
            });
        }

        private void SwapHandler(UIComponent component, UIMouseEventParameter param)
        {
            BuildingHandler(SwapBuildingPositions);
        }

        private static void SwapBuildingPositions(ushort id)
        {
            Configuration.InvertState(Singleton<BuildingManager>.instance.m_buildings.m_buffer[id]);
        }
    }

}