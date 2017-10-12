using UnityEngine;

namespace UnityHTTP
{
    public abstract class HTTPGameObj : MonoBehaviour
    {
        private void Awake()
        {
            AwakeServer();
        }

        private void Start()
        {
            StartServer();
        }

        private void OnDestroy()
        {
            DestroyServer();

            HTTPServer.Instance.Dispose();
        }

        protected virtual void AwakeServer()
        {
            
        }

        protected virtual void StartServer()
        {
            HTTPServer.Instance.StartListening();
        }

        protected virtual void DestroyServer()
        {

        }
    }
}
