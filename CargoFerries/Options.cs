using System.Xml.Serialization;
using CargoFerries.Attributes;
using CargoFerries.OptionsFramework.Attibutes;

namespace CargoFerries
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
            nameof(CargoFerriesEditedAssetTransformer), nameof(CargoFerriesEditedAssetTransformer.ToBargeHarbor))]
        public object ToBargeHarborButton { get; set; } = null;
        
        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To barge", null, 
            nameof(CargoFerriesEditedAssetTransformer), nameof(CargoFerriesEditedAssetTransformer.ToBarge))]
        public object ToBargeButton { get; set; } = null;

        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To cargo helicopter depot", null,
            nameof(CargoFerriesEditedAssetTransformer), nameof(CargoFerriesEditedAssetTransformer.ToCargoHelicopterDepot))]
        public object ToCargoHelicopterDepotButton { get; set; } = null;

        [HideWhenNotInAssetEditorCondition]
        [XmlIgnore]
        [Button("To cargo helicopter", null,
            nameof(CargoFerriesEditedAssetTransformer), nameof(CargoFerriesEditedAssetTransformer.ToCargoHelicopter))]
        public object ToCargoHelicopterButton { get; set; } = null;
    }
}