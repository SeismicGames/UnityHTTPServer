using System.Net;

namespace UnityHTTP
{
    public abstract class ARoute
    {
        public void HandleRequest(string method, HttpListenerResponse response, string id, string data)
        {
            switch (method)
            {
                case "GET":
                    Get(response);
                    break;
                case "POST":
                    Post(response, data);
                    break;
                case "PUT":
                    Put(response, id, data);
                    break;
                case "DELETE":
                    Delete(response, id);
                    break;
            }
        }

        protected abstract void Get(HttpListenerResponse response);

        protected abstract void Post(HttpListenerResponse response, string data);

        protected abstract void Put(HttpListenerResponse response, string id, string data);

        protected abstract void Delete(HttpListenerResponse response, string id);

    }
}
