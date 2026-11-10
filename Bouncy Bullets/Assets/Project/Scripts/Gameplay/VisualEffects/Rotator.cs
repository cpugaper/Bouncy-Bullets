using UnityEngine;

namespace Game.Gameplay
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 50f, 0f);

        void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
