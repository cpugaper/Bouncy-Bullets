using UnityEngine;

namespace Game.UI
{
    public class HideOnWeb : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer) gameObject.SetActive(false);
        }
    }
}
