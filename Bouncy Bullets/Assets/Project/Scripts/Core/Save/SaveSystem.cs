using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Game.Core
{
    public static class SaveSystem
    {

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SyncFiles();
#endif

        private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");

        public static SaveData Load()
        {
            try
            {
                if (!File.Exists(FilePath)) return new SaveData();
                return JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath)) ?? new SaveData();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Save could not be read, using new data: {e.Message}");
                return new SaveData();
            }
        }

        public static void Save(SaveData data)
        {
            try
            {
                string tempPath = FilePath + ".tmp";
                File.WriteAllText(tempPath, JsonUtility.ToJson(data));

                if (File.Exists(FilePath)) File.Replace(tempPath, FilePath, null);
                else File.Move(tempPath, FilePath);

#if UNITY_WEBGL && !UNITY_EDITOR
                SyncFiles();
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"Game could not be saved: {e.Message}");
            }
        }

        public static void Delete()
        {
            try
            {
                if (File.Exists(FilePath)) File.Delete(FilePath);

#if UNITY_WEBGL && !UNITY_EDITOR
                SyncFiles();
#endif
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Save could not be deleted: {e.Message}");
            }
        }
    }
}
