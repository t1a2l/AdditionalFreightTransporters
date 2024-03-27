using ColossalFramework;
using System;
using UnityEngine;

namespace AdditionalFreightTransporters
{
    public class Settings
    {
        public const string settingsFileName = "AFT_Settings";

        public static SavedInt DelayBarge = new("DelayBarge", settingsFileName, 4, true);
        public static SavedInt DelayHelicopter = new("DelayHelicopter", settingsFileName, 4, true);

        public static SavedBool BargeWaitUntilFull = new("BargeWaitUntilFull", settingsFileName, false, true);

        public static void Init()
        {
            try
            {
                // Creating setting file
                if (GameSettings.FindSettingsFileByName(settingsFileName) == null)
                {
                    GameSettings.AddSettingsFile([new SettingsFile() { fileName = settingsFileName }]);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Could not load/create the setting file.");
                Debug.LogException(e);
            }
        }

        
    }
}
