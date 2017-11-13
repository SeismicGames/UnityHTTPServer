using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Sockets;
using Seismic;

namespace UnityHTTP
{
    public class HTTPServer
    {
        private readonly HttpListener _httpListener;
        private readonly Dictionary<Regex, ARoute> _routes = new Dictionary<Regex, ARoute>();
        private const string _logTag = "HttpServer";
        
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

        private readonly string _title;
        public string Title
        {
            get { return _title;  }
        }

        // construtor
        private HTTPServer()
        {
            // test if available
            if (!HttpListener.IsSupported)
            {
                Log.Warn(_logTag,"HTTPListener is not supported");
                return;
            }
            Log.Info(_logTag,"HTTPListener is supported");

            _title = Application.productName;

            // set up HTTPListener
            int port = 9090;
            while(PortInUse(port)) ++port;
            Log.Info(_logTag, "Using port {0}", port);

            _host = string.Format("http://{0}:{1}/", Network.player.ipAddress, port);
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(_host);
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        }
        
        public static bool PortInUse(int port)
        {
            try
            {
                var tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                tcpListener.Stop();
            }
            catch (SocketException)
            {
                return true;
            }

            return false;
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
            StartListening(new List<ARoute>
            {
                new LogRoute()
            });
        }

        public void StartListening(params ARoute[] routes)
        {
            List<ARoute> routeList = new List<ARoute>(routes) { new LogRoute() };
            StartListening(routeList);
        }

        public void StartListening(List<ARoute> routes)
        {
            // make sure the LogRoute is in the list
            bool containsLog = false;
            for (int i = 0; i < routes.Count; i++)
            {
                if (routes[i].GetType() != typeof(LogRoute))
                {
                    continue;
                }

                containsLog = true;
                break;
            }

            if (!containsLog)
            {
                routes.Add(new LogRoute());
            }

            // set up routes
            for (int i = 0; i < routes.Count; i++)
            {
                _routes[routes[i].GetPattern()] = routes[i];
                _navLinks.Add(routes[i]);
            }

            if (_httpListener == null)
            {
                return;
            }

            _httpListener.Start();
            Log.Info(_logTag, "Listening on " + _host);
            _httpListener.BeginGetContext(HTTPCallback, _httpListener);
        }

        // HTTP listener callback
        private void HTTPCallback(IAsyncResult result)
        {
            HttpListener thisListener = (HttpListener) result.AsyncState;

            HttpListenerContext context = thisListener.EndGetContext(result);
            HttpListenerRequest request = context.Request;

            using (HttpListenerResponse response = context.Response)
            {

                if (request.Url.AbsolutePath.Equals(@"/"))
                {
                    response.Redirect(@"/log/");

                    // start listener again
                    _httpListener.BeginGetContext(HTTPCallback, _httpListener);
                    return;
                }

                foreach (Regex regex in _routes.Keys)
                {
                    Match match = regex.Match(request.Url.AbsolutePath);
                    if (!match.Success) continue;

                    try
                    {
                        // parse id and data
                        string data = "";
                        if (request.HttpMethod.ToUpper().Equals("POST") ||
                            request.HttpMethod.ToUpper().Equals("PUT"))
                        {
                            using (StreamReader reader =
                                new StreamReader(request.InputStream, request.ContentEncoding))
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

                    }
                    catch (Exception e)
                    {
                        Log.Error(_logTag, "HTTPServer error: {0}", e.Message);
                        ARoute.SendResponse(response, e.Message, 500);
                    }

                    // start listener again
                    _httpListener.BeginGetContext(HTTPCallback, _httpListener);
                    return;
                }

                // return 404 and start listener again
                ARoute.SendResponse(response, "", 404);
                _httpListener.BeginGetContext(HTTPCallback, _httpListener);
            }
        }
    }
}
