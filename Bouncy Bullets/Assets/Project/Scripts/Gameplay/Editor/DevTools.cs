using UnityEngine;
using UnityEditor;

public static class DevTools
{
    [MenuItem("Tools/Delete Save File")]
    private static void DeleteSave()
    {
        SaveSystem.Delete();
        Debug.Log("Save file deleted");
    }
}
