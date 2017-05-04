using System.IO;
using System.Net;
using System.Text;

namespace UnityHTTP
{
    public class LogRoute : ARoute
    {
        protected override void Get(HttpListenerResponse response)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><body style=\"font-family: Courier New;\">");
            sb.Append(File.ReadAllText(HTTPLogServer.LOG_FILENAME));
            sb.Append("</body></html>");
            byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(sb.ToString());

            // send response
            response.StatusCode = 200;
            response.ContentType = "text/html";
            response.ContentLength64 = responseBuffer.Length;
            Stream responseStream = response.OutputStream;
            responseStream.Write(responseBuffer, 0, responseBuffer.Length);
            responseStream.Close();
            response.Close();
        }

        protected override void Post(HttpListenerResponse response, string data)
        {
            // not implemented
        }

        protected override void Put(HttpListenerResponse response, string id, string data)
        {
            // not implemented
        }

        protected override void Delete(HttpListenerResponse response, string id)
        {
            // not implemented
        }
    }
}
