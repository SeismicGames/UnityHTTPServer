using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using DotLiquid;
using UnityEngine;

namespace UnityHTTP
{
    public abstract class ARoute
    {
        [LiquidType("Content", "NavLinks")]
        public class Menu
        {
            public List<NavLink> NavLinks { get; set; }
            public string Content { get; set; }
        }

        [LiquidType("Title", "Target", "Class")]
        public class NavLink
        {
            public string Title { get; set; }
            public string Target { get; set; }
            public string Class { get; set; }
        }

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

        protected Hash GetHTMLHash(ARoute that, string content)
        {
            List <NavLink> links = new List<NavLink>();
            foreach (var navLink in HTTPServer.Instance.NavLinks)
            {
                string style = navLink.GetType() == that.GetType() ? "nav-link active" : "nav-link";
                links.Add(new NavLink
                {
                    Class = style,
                    Title = navLink.GetTitle(),
                    Target = navLink.GetPath()
                });
            }
            
            return Hash.FromAnonymousObject(new 
            {
                menu = new Menu
                {
                    Content = content,
                    NavLinks = links
                }
            });
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
</head>
           
<body>
    <ul class=""nav nav-tabs"">
    {% for nav in menu.nav_links %}
        <li class=""nav-item"">
            <a class=""{{ nav.class }}"" href=""{{ nav.target }}"">{{ nav.title }}</a>
        </li>
    {% endfor %}
    </ul>

    {{ menu.content }}

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
