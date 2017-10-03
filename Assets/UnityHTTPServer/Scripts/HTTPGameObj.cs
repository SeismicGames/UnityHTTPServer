using UnityEngine;

namespace UnityHTTP
{
    public class HTTPGameObj : MonoBehaviour
    {
        private HTTPGameObj _instance;

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject); // There can be only one
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // TODO: find a dynamic way to add handlers
            Application.logMessageReceivedThreaded += LogRoute.HandleLog;
        }

        void Start()
        {
            HTTPServer.Instance.StartListening();
        }

        void OnDestroy()
        {
            if (_instance != this)
            {
                return;
            }

            _instance = null;

            HTTPServer.Instance.Dispose();

            // TODO: find a dynamic way to add handlers
            Application.logMessageReceivedThreaded -= LogRoute.HandleLog;
        }
    }
}
