using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "TransformReferenceSO", menuName = "Runtime/Transform Reference")]
    public class TransformReferenceSO : ScriptableObject
    {
        public Transform Value { get; private set; }

        public void Register(Transform target) => Value = target;

        public void Unregister(Transform target)
        {
            if (Value == target) Value = null;
        }
    }
}
