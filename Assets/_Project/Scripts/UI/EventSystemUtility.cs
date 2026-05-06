using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace GhostBeam.UI
{
    public static class EventSystemUtility
    {
        public static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                EnsureInputModule(EventSystem.current.gameObject);
                return;
            }

            var eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            EnsureInputModule(eventSystemObj);
        }

        private static void EnsureInputModule(GameObject eventSystemObj)
        {
#if ENABLE_INPUT_SYSTEM
            if (eventSystemObj.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystemObj.AddComponent<InputSystemUIInputModule>();
            }
#else
            if (eventSystemObj.GetComponent<StandaloneInputModule>() == null)
            {
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
#endif
        }
    }
}
