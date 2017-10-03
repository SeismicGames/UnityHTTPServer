using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DotLiquid;
using UnityEngine;

namespace UnityHTTP
{
    public class LogRoute : ARoute
    {
        public static readonly string LOG_FILENAME = Application.persistentDataPath + Path.DirectorySeparatorChar + "unity.log";

        protected override void Get(HttpListenerResponse response)
        {
            string logContent = File.ReadAllText(LOG_FILENAME);
            Template template = GetHTMLTemplate();
            string html = template.Render(GetHTMLHash(this, logContent));
            byte[] responseBuffer = Encoding.UTF8.GetBytes(html);

            // send response
            response.StatusCode = 200;
            response.ContentType = "text/html";
            response.ContentLength64 = responseBuffer.Length;
            using (Stream responseStream = response.OutputStream)
            {
                responseStream.Write(responseBuffer, 0, responseBuffer.Length);
            }
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

        public override string GetTitle()
        {
            return @"Logs";
        }

        public override string GetPath()
        {
            return @"/log/";
        }

        public override Regex GetPattern()
        {
            return new Regex(@"^/log/?([a-zA-Z0-9\-]*)/?");
        }

        public override void Dispose()
        {
            if (File.Exists(LOG_FILENAME))
            {
                File.Delete(LOG_FILENAME);
            }
        }

        public static void HandleLog(string logString, string stackTrace, LogType logType)
        {
            // write log to file
            string color;
            switch (logType)
            {
                case LogType.Error:
                    color = "text-danger";
                    break;
                case LogType.Warning:
                    color = "text-warning";
                    break;
                default:
                    color = "";
                    break;
            }

            IDictionary<string, object> output = new Dictionary<string, object>
            {
                {"id", Guid.NewGuid().ToString() },
                {"color", color},
                {"title", string.Format("[{0}] [{1}] - {2}", DateTime.Now, logType, logString)},
                {"log",  stackTrace.Replace("\n", "<br />").Trim()}
            };

            Template template = Template.Parse(GetEntryTemplate());
            string html = template.Render(Hash.FromDictionary(output));

            lock (LOG_FILENAME)
            {
                using (StreamWriter file = new StreamWriter(LOG_FILENAME, true))
                {
                    file.Write(html);
                }
            }
        }

        static string GetEntryTemplate()
        {
            return @"
<div class=""card"">
    <div class=""card-header"" role=""tab"" id=""heading{{ id }}"">
        <h5 class=""mb-0"">
            <a class=""{{ color }}"" data-toggle=""collapse"" data-parent=""#accordion"" href=""#collapse{{ id }}"" aria-expanded=""true"" aria-controls=""collapse{{ id }}"">
                {{ title }}
            </a>
        </h5>
    </div>

    <div id = ""collapse{{ id }}"" class=""collapse"" role=""tabpanel"" aria-labelledby=""heading{{ id }}"">
        <div class=""card-block p-3 {{ color }}"">
            {{ log }}
        </div>
    </div>
</div>";
        }
    }
}
