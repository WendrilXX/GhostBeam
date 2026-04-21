using UnityEngine;

namespace GhostBeam.Utilities
{
    public class PooledObject : MonoBehaviour
    {
        private System.Action<PooledObject> onRelease;

        public void Initialize(System.Action<PooledObject> releaseCallback)
        {
            onRelease = releaseCallback;
        }

        public void ReleaseToPool()
        {
            onRelease?.Invoke(this);
        }
    }
}
