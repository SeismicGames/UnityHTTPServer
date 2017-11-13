using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DotLiquid;

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

        public abstract void Dispose();

        public abstract string GetTitle();

        public abstract string GetPath();

        public abstract Regex GetPattern();

        protected Hash GetHTMLMenu(ARoute that, string content, string title = null)
        {
            List <Dictionary<string, object>> links = new List<Dictionary<string, object>>();
            foreach (var navLink in HTTPServer.Instance.NavLinks)
            {
                string style = navLink.GetType() == that.GetType() ? "nav-link active" : "nav-link";
                links.Add(new Dictionary<string, object>()
                {
                    {"class", style},
                    {"title", navLink.GetTitle()},
                    {"target",  navLink.GetPath()}
                });
            }
            
            IDictionary<string, object> output = new Dictionary<string, object>
            {
                {"title", title ?? HTTPServer.Instance.Title},
                {"content", content},
                {"nav_links", links}
            };
            
            return Hash.FromDictionary(output);
        }

        public static void SendResponse(HttpListenerResponse response, string html, int code)
        {
            byte[] responseBuffer = Encoding.UTF8.GetBytes(html);

            // send response
            response.StatusCode = code;
            response.ContentType = "text/html";
            response.ContentLength64 = responseBuffer.Length;
            using (Stream responseStream = response.OutputStream)
            {
                responseStream.Write(responseBuffer, 0, responseBuffer.Length);
            }
        }

        public static Template GetHTMLTemplate()
        {
            return Template.Parse(@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset = ""utf-8"">
    <meta name = ""viewport"" content = ""width=device-width, initial-scale=1, shrink-to-fit=no"">
    
    <!--Bootstrap CSS-->
    <link rel = ""stylesheet"" href = ""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta/css/bootstrap.min.css"" integrity = ""sha384-/Y6pD6FV/Vv2HJnA6t+vslU6fwYXjCFtcEpHbNJ0lyAFsXTsjBbfaDjzALeQsN6M"" crossorigin = ""anonymous"">         
    <title>{{ title }}</title>
    <style>
    body {
        padding-top: 1.5rem;
        padding-bottom: 1.5rem;
    }
    
    .header {
        margin-bottom: 2rem;
        padding-bottom: 1rem;
        border-bottom: .05rem solid #e5e5e5;
    }
    </style>
</head>
           
<body>
    <div class=""container"">
        <div class=""header clearfix"">
            <nav>
                <ul class=""nav nav-pills float-right"">
                {% for nav in nav_links %}
                    <li class=""nav-item"">
                        <a class=""{{ nav.class }}"" href=""{{ nav.target }}"">{{ nav.title }}</a>
                    </li>
                {% endfor %}
                </ul>
            </nav>
            <h3 class=""text-muted"">{{ title }}</h3>
        </div>
        
        {{ content }}
    </div>

    <!--Optional JavaScript-->
    <!--jQuery first, then Popper.js, then Bootstrap JS-->
    <script src = ""https://code.jquery.com/jquery-3.2.1.slim.min.js"" integrity = ""sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN"" crossorigin = ""anonymous"" ></script> 
    <script src = ""https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js"" integrity = ""sha384-b/U6ypiBEHpOf/4+1nzFpr53nxSS+GLCkfwBdFNTxtclqqenISfwAzpKaMNFNmj4"" crossorigin = ""anonymous"" ></script>
    <script src = ""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta/js/bootstrap.min.js"" integrity = ""sha384-h0AbiXch4ZDo7tp9hKZ4TsHbi047NrKGLO3SEJAg45jXxnGIfYzk4Si90RDIqNm1"" crossorigin = ""anonymous"" ></script>    
</body>
</html>");
        }
    }
}



