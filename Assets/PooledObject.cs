using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool ownerPool;

    public void ReturnToPool()
    {
        if (ownerPool != null)
        {
            ownerPool.Return(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
