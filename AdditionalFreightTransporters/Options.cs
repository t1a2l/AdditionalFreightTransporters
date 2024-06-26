﻿using System.Xml.Serialization;
using AdditionalFreightTransporters.Attributes;
using AdditionalFreightTransporters.OptionsFramework.Attibutes;

namespace AdditionalFreightTransporters
{
    [Options("Barges-Options")]
    public class Options
    {
        [HideInGameOrEditorCondition]
        [Checkbox("Built-in warehouse for barge harbors, helicopter and tram facilities (Industries DLC is required)")]
        public bool EnableWarehouseAI { get; set; } = true;
        
        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To barge harbor", null, nameof(EditedAssetTransformer), nameof(EditedAssetTransformer.ToBargeHarborFacility))]
        public object ToBargeHarborButton { get; set; } = null;
        
        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To barge vehicle", null, nameof(EditedAssetTransformer), nameof(EditedAssetTransformer.ToBargeVehicle))]
        public object ToBargeVehicleButton { get; set; } = null;

        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To cargo helicopter depot", null, nameof(EditedAssetTransformer), nameof(EditedAssetTransformer.ToCargoHelicopterFacility))]
        public object ToCargoHelicopterDepotButton { get; set; } = null;

        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To cargo helicopter vehicle", null, nameof(EditedAssetTransformer), nameof(EditedAssetTransformer.ToCargoHelicopterVehicle))]
        public object ToCargoHelicopterVehicleButton { get; set; } = null;

        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To cargo tram depot", null, nameof(EditedAssetTransformer), nameof(EditedAssetTransformer.ToCargoHelicopterFacility))]
        public object ToCargoTramDepotButton { get; set; } = null;

        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To cargo tram vehicle", null, nameof(EditedAssetTransformer), nameof(EditedAssetTransformer.ToCargoHelicopterVehicle))]
        public object ToCargoTramVehicleButton { get; set; } = null;
    }
}