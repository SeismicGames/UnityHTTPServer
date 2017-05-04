using UnityEngine;
using System;
using System.IO;
using System.Text;

namespace UnityHTTP
{
    public class HTTPLogGameObj : MonoBehaviour
    {
        private HTTPLogGameObj _instance;

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject); // There can be only one
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Application.logMessageReceivedThreaded += HandleLog;
        }

        void Start()
        {
            HTTPLogServer.Instance.StartListening();
        }

        void OnDestroy()
        {
            if (_instance != this)
            {
                return;
            }

            _instance = null;

            HTTPLogServer.Instance.Dispose();
            Application.logMessageReceivedThreaded -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType logType)
        {
            // write log to file
            StringBuilder sb = new StringBuilder();
            string style = string.Empty;
            switch (logType)
            {
                case LogType.Error:
                    style = "style=\"background-color: #ffffff; color: #ff0000;\"";
                    break;
                case LogType.Warning:
                    style = "style=\"background-color: #ffffff; color: #ff9900;\"";
                    break;
                default:
                    style = "style=\"background-color: #ffffff; color: #000000;\"";
                    break;
            }

            sb.Append(string.Format("<div {0}>", style));
            sb.Append(string.Format("[{0}] [{1}] - {2}", DateTime.Now, logType, logString));
            sb.Append("</div><br />");
            sb.Append(string.Format("<div {0}>", style));
            sb.Append(stackTrace);
            sb.Append("</div><br />");

            lock (HTTPLogServer.LOG_FILENAME)
            {
                using (StreamWriter file = new StreamWriter(HTTPLogServer.LOG_FILENAME, true))
                {
                    file.Write(sb.ToString());
                }
            }
        }
    }
}