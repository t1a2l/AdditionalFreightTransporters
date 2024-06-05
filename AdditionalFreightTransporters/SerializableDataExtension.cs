using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ICities;

namespace AdditionalFreightTransporters
{
    public class SerializableDataExtension : SerializableDataExtensionBase
    {
        public static Dictionary<ushort, byte> RawState;
        private const string DataId = "StationsFineTuning";

        public override void OnLoadData()
        {
            var data = serializableDataManager.LoadData(DataId);
            if (data == null)
            {
                RawState = null;
                return;
            }
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream(data);
            RawState = (Dictionary<ushort, byte>)binFormatter.Deserialize(mStream);
        }

        public override void OnSaveData()
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            var rawState = new Dictionary<ushort, byte>();
            foreach (var pair in Configuration.GetState())
            {
                try
                {
                    var id = Utils.Util.StationBuildingIdByPosition(pair.Key);
                    rawState[id] = pair.Value;
                }
                catch
                {
                    // ignored
                }
            }
            binFormatter.Serialize(mStream, rawState);
            serializableDataManager.SaveData(DataId, mStream.ToArray());
        }
    }
}