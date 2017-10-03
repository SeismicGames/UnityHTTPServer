using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace UnityHTTP
{
    public class HTTPServer
    {
        private readonly HttpListener _httpListener;
        private readonly Dictionary<Regex, ARoute> _routes = new Dictionary<Regex, ARoute>();
        
        // Singleton method
        private static HTTPServer _instance;
        public static HTTPServer Instance
        {
            get { return _instance ?? (_instance = new HTTPServer()); }
        }

        // class parameter methods
        private readonly string _host = "";
        public string Host
        {
            get { return _host; }
        }

        private readonly List<ARoute> _navLinks = new List<ARoute>();
        public List<ARoute> NavLinks
        {
            get { return _navLinks; }
        }

        // construtor
        private HTTPServer()
        {
            // test if available
            if (!HttpListener.IsSupported)
            {
                Debug.Log("HTTPListener is not supported");
                return;
            }
            Debug.Log("HTTPListener is supported");

            // set up HTTPListener
            _host = "http://" + Network.player.ipAddress + ":9090/";
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(_host);
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            // set up routes
            // log route
            LogRoute logRoute = new LogRoute();
            _routes[logRoute.GetPattern()] = logRoute;
            _navLinks.Add(logRoute);
        }

        // class methods
        public void Dispose()
        {
            if (_httpListener != null)
            {
                _httpListener.Close();
            }

            foreach (var routePair in _routes)
            {
                routePair.Value.Dispose();
            }
            _instance = null;
        }

        public void StartListening()
        {
            if (_httpListener != null)
            {
                _httpListener.Start();
                Debug.Log("Listening on " + _host);
                _httpListener.BeginGetContext(HTTPCallback, _httpListener);
            }
        }

        // HTTP listener callback
        private void HTTPCallback(IAsyncResult result)
        {
            HttpListener thisListener = (HttpListener) result.AsyncState;

            HttpListenerContext context = thisListener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            foreach (Regex regex in _routes.Keys)
            {
                // TODO: list available paths if path == /, maybe?
                Match match = regex.Match(request.Url.AbsolutePath);
                if (match.Success)
                {
                    // parse id and data
                    string data = "";
                    if (request.HttpMethod.ToUpper().Equals("POST") || request.HttpMethod.ToUpper().Equals("PUT"))
                    {
                        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            data = reader.ReadToEnd();
                        }
                    }

                    string id = "";
                    if (match.Groups.Count > 1)
                    {
                        id = match.Groups[1].Value;
                    }

                    // fire response handler
                    _routes[regex].HandleRequest(request.HttpMethod.ToUpper(), response, id, data);
                    _httpListener.BeginGetContext(HTTPCallback, _httpListener);
                    return;
                }
            }

            // return 404
            response.StatusCode = 404;
            response.Close();

            // start listener again
            _httpListener.BeginGetContext(HTTPCallback, _httpListener);
        }
    }
}
