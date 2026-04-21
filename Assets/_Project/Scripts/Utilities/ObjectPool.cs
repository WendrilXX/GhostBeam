using UnityEngine;
using System;
using System.Collections.Generic;

namespace GhostBeam.Utilities
{
    public class ObjectPool<T> where T : class
    {
        private Queue<T> available = new();
        private HashSet<T> inUse = new();
        private Func<T> createFunc;
        private Action<T> onGetAction;
        private Action<T> onReleaseAction;

        public ObjectPool(Func<T> create, Action<T> onGet = null, Action<T> onRelease = null, int initialSize = 10)
        {
            createFunc = create;
            onGetAction = onGet;
            onReleaseAction = onRelease;

            for (int i = 0; i < initialSize; i++)
            {
                available.Enqueue(create());
            }
        }

        public T Get()
        {
            T obj;
            
            // Try to get from available pool, but skip destroyed objects
            while (available.Count > 0)
            {
                obj = available.Dequeue();
                
                // Check if object still exists (only for GameObjects)
                if (obj is GameObject gameObj && gameObj == null)
                {
                    continue; // Skip destroyed objects
                }
                
                inUse.Add(obj);
                onGetAction?.Invoke(obj);
                return obj;
            }
            
            // Create new if none available
            obj = createFunc();
            inUse.Add(obj);
            onGetAction?.Invoke(obj);
            return obj;
        }

        public void Release(T obj)
        {
            if (inUse.Remove(obj))
            {
                onReleaseAction?.Invoke(obj);
                available.Enqueue(obj);
            }
        }

        public int AvailableCount => available.Count;
        public int InUseCount => inUse.Count;
    }
}
