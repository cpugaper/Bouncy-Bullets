using UnityEngine;
using UnityEditor;
using Game.Core;

namespace Game.Editor
{
    public static class DevTools
    {
        [MenuItem("Tools/Delete Save File")]
        private static void DeleteSave()
        {
            SaveSystem.Delete();
            Debug.Log("Save file deleted");
        }
    }
}
