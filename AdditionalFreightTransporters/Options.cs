using System.Xml.Serialization;
using AdditionalFreightTransporters.Attributes;
using AdditionalFreightTransporters.OptionsFramework.Attibutes;

namespace AdditionalFreightTransporters
{
    [Options("Barges-Options")]
    public class Options
    {
        [HideInGameOrEditorCondition]
        [Checkbox("Built-in warehouse for barge harbors (Industries DLC is required)")]
        public bool EnableWarehouseAI { get; set; } = true;
        
        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To barge harbor", null, 
            nameof(AdditionalFreightTransportersEditedAssetTransformer), nameof(AdditionalFreightTransportersEditedAssetTransformer.ToBargeHarbor))]
        public object ToBargeHarborButton { get; set; } = null;
        
        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To barge vehicle", null, 
            nameof(AdditionalFreightTransportersEditedAssetTransformer), nameof(AdditionalFreightTransportersEditedAssetTransformer.ToBargeVehicle))]
        public object ToBargeButton { get; set; } = null;

        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To cargo helicopter depot", null,
            nameof(AdditionalFreightTransportersEditedAssetTransformer), nameof(AdditionalFreightTransportersEditedAssetTransformer.ToCargoHelicopterDepot))]
        public object ToCargoHelicopterDepotButton { get; set; } = null;

        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To cargo helicopter vehicle", null,
            nameof(AdditionalFreightTransportersEditedAssetTransformer), nameof(AdditionalFreightTransportersEditedAssetTransformer.ToCargoHelicopterVehicle))]
        public object ToCargoHelicopterVehicleButton { get; set; } = null;

        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To cargo tram depot", null,
            nameof(AdditionalFreightTransportersEditedAssetTransformer), nameof(AdditionalFreightTransportersEditedAssetTransformer.ToCargoHelicopterDepot))]
        public object ToCargoTramDepotButton { get; set; } = null;

        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To cargo tram vehicle", null,
            nameof(AdditionalFreightTransportersEditedAssetTransformer), nameof(AdditionalFreightTransportersEditedAssetTransformer.ToCargoHelicopterVehicle))]
        public object ToCargoTramVehicleButton { get; set; } = null;
    }
}