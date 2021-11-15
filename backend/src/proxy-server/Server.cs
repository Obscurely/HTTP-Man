using System;
using System.Net;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Exceptions;
using Titanium.Web.Proxy.Helpers;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Http2;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.Network;
using Titanium.Web.Proxy.StreamExtended;

#pragma warning disable CS1998 // disables the warning "async method lacks await operators and will run synchronously" since titanium web proxy requires the handlers to be async.

namespace HTTPMan
{
    /// <summary>
    /// The proxy server that intercepts the requests.
    /// </summary>
    public class Server
    {
        // Fields
        private readonly ProxyServer _proxyServer = new();
        private bool _isServerStarted;
        private bool _isSystemProxySet;
        private readonly ExplicitProxyEndPoint _explicitEndPoint;
        private readonly TransparentProxyEndPoint _transparentEndPoint;
        private List<TunnelConnectSessionEventArgs> _tunnelConnectRequests = new();
        private List<SessionEventArgs> _httpRequests = new();
        private List<SessionEventArgs> _httpResponses = new();
        private List<MockerRule> _httpRules = new();
        private readonly Mocker _httpMocker = new();

        // Properties
        private ProxyServer ProxyServer { get { return _proxyServer; } }
        private bool IsServerStarted { get { return _isServerStarted; } set { _isServerStarted = value; } }
        private bool IsSystemProxySet { get { return _isSystemProxySet; } set { _isSystemProxySet = value; } }
        private ExplicitProxyEndPoint ExplicitEndPoint { get { return _explicitEndPoint; } }
        private TransparentProxyEndPoint TransparentEndPoint { get { return _transparentEndPoint; } }
        public List<TunnelConnectSessionEventArgs> TunnelConnectRequests { get { return _tunnelConnectRequests; } }
        public List<SessionEventArgs> HttpRequests { get { return _httpRequests; } }
        public List<SessionEventArgs> HttpResponses { get { return _httpResponses; } }
        public List<MockerRule> HttpRules { get { return _httpRules; } }
        public Mocker HttpMocker { get { return _httpMocker; } }

        // Constructors
        /// <summary>
        /// Creates a Server (closed) with the given IP and PORT.
        /// </summary>
        /// <param name="explicitEndPointIP">The IP for the proxy's explicit end point.</param>
        /// <param name="explicitEndPointPort">The PORT for the proxy's explicit end point.</param>
        /// <param name="transparentEndPointIP">The IP for the proxy's transparent end point.</param>
        /// <param name="transparentEndPointPort">The PORT for the proxy's transparent end point.</param>
        public Server(IPAddress explicitEndPointIP, int explicitEndPointPort, IPAddress transparentEndPointIP, int transparentEndPointPort)
        {
            _explicitEndPoint = new(explicitEndPointIP, explicitEndPointPort, true);
            _transparentEndPoint = new(transparentEndPointIP, transparentEndPointPort, true) { GenericCertificateName = "google.com" };
        }

        // Methods
        /// <summary>
        /// Starts the server on the specified ips and ports.
        /// </summary>
        /// <returns>True if server was successfully started and false if it failed to start.</returns>
        public bool Start()
        {
            // Adds the handlers for the proxy.
            ProxyServer.BeforeRequest += OnRequest;
            ProxyServer.BeforeResponse += OnResponse;
            ProxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
            ProxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;

            // Fired when a CONNECT request is received.
            ExplicitEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;

            // Explicit endpoint.
            ProxyServer.AddEndPoint(ExplicitEndPoint);

            // Transparent endpoint.
            ProxyServer.AddEndPoint(TransparentEndPoint);

            // Proxy Server Start.
            ProxyServer.Start();

            IsServerStarted = true; // stores up that the server has successfully started (here nothing can go wrong anymore).

            return true;
        }

        /// <summary>
        /// Stops the current running server if any.
        /// </summary>
        /// <returns>Return true if server was stopped and false if server couldn't be stopped or wasn't even running.</returns>
        public bool Stop()
        {
            if (IsServerStarted)
            {
                ExplicitEndPoint.BeforeTunnelConnectRequest -= OnBeforeTunnelConnectRequest;
                ProxyServer.BeforeRequest -= OnRequest;
                ProxyServer.BeforeResponse -= OnResponse;
                ProxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
                ProxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;

                ProxyServer.Stop();

                return true; // returns when the server successfully closed.
            }
            else
            {
                return false; // returns when the server wasn't started.
            }
        }

        /// <summary>
        /// Returns a byte array containing the root certificate used by the proxy for intercepting network traffic.
        /// </summary>
        /// <returns>A byte array containing the root certificate.</returns>
        public byte[] GetRootCertificate()
        {
            return ProxyServer.CertificateManager.RootCertificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert);
        }

        /// <summary>
        /// Prompts user to trust the root certificate used by the proxy if on windows. Only works on Windows.
        /// </summary>
        /// <returns>True if the user is running windows and it was prompted to trust the certificate and false if it's running something else.</returns>
        public bool TrustCertificate()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ProxyServer.CertificateManager.TrustRootCertificate(true);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets proxy's ip and port as system's proxy if on windows. Only works on Windows.
        /// </summary>
        /// <returns>Return true if the user is on windows and the proxy was set and false if the user is on another platform.</returns>
        public bool SetAsSystemProxy()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ProxyServer.SetAsSystemHttpProxy(ExplicitEndPoint);
                ProxyServer.SetAsSystemHttpsProxy(ExplicitEndPoint);
                IsSystemProxySet = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gives back a string array containing the names of adapters used by the proxy.
        /// </summary>
        /// <returns>A string array containing the names of adapters used by the proxy.</returns>
        public string[] GetListenerDevices()
        {
            if (IsServerStarted)
            {
                string[] devices = new string[ProxyServer.ProxyEndPoints.Count];
                for (int i = 0; i < ProxyServer.ProxyEndPoints.Count; i++)
                {
                    devices[i] = ProxyServer.ProxyEndPoints[i].GetType().Name;
                }

                return devices;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the ips and ports the proxy is listening to.
        /// </summary>
        /// <returns>An array of (string, string) containing the ips and ports the proxy is listening to.</returns>
        public (string, string)[] GetListenerIpsAndPorts()
        {
            if (IsServerStarted)
            {
                (string, string)[] ipsAndPorts = new (string, string)[ProxyServer.ProxyEndPoints.Count];
                for (int i = 0; i < ProxyServer.ProxyEndPoints.Count; i++)
                {
                    ipsAndPorts[i] = (ProxyServer.ProxyEndPoints[i].IpAddress.ToString(), ProxyServer.ProxyEndPoints[i].Port.ToString());
                }

                return ipsAndPorts;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Intercepts all the tunnel connect requests and allows modifying them before they are sent.
        /// </summary>
        /// <param name="sender">Object defined by the proxy.</param>
        /// <param name="e">Param which contains all the info of the request and is used to edit it.</param>
        /// <returns>Nothing.</returns>
        private async Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            e.DecryptSsl = true; // Decrypts ssl in order to see and modify the requests and responses.
            TunnelConnectRequests.Add(e); //Stores Tunnel Connect Request.

            // HACK debug line, prints out to console tunnel request's urls.
            System.Console.WriteLine(e.HttpClient.ConnectRequest.Url);
        }

        /// <summary>
        /// Intercepts all http requests and allows modifying before they are sent.
        /// </summary>
        /// <param name="sender">Object defined by the proxy.</param>
        /// <param name="e">Param which contains all the info of the request and is used to edit it.</param>
        /// <returns>Nothing.</returns>
        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            // HACK debug line, prints out to console request's urls.
            System.Console.WriteLine(e.HttpClient.Request.Url);

            if (HttpRules.Count != 0)
            {
                for (int i = 0; i < HttpRules.Count; i++)
                {
                    if ((HttpRules[i].Method.GetString().ToUpper() != e.HttpClient.Request.Method.ToUpper() && HttpRules[i].Method != MockHttpMethod.Any) || !HttpRules[i].IsValid)
                    {
                        continue;
                    } 
                    if (!HttpRules[i].IsForRequest)
                    {
                        continue;
                    }

                    e = await HttpMocker.Mock(HttpRules[i], e, true);
                }
            }

            HttpRequests.Add(e); // Stores Http Request.
            e.UserData = e.HttpClient.Request; // Stores request in order to find it from the request handler.
        }

        /// <summary>
        /// Intercepts all http responses and allows modifying them before they are received.
        /// </summary>
        /// <param name="sender">Object defined by the proxy.</param>
        /// <param name="e">Param which contains all the info of the request and is used to edit it.</param>
        /// <returns>Nothing.</returns>
        private async Task OnResponse(object sender, SessionEventArgs e)
        {
            if (HttpRules.Count != 0)
            {
                for (int i = 0; i < HttpRules.Count; i++)
                {
                    if ((HttpRules[i].Method.GetString().ToUpper() != e.HttpClient.Request.Method.ToUpper() && HttpRules[i].Method != MockHttpMethod.Any) || !HttpRules[i].IsValid)
                    {
                        continue;
                    }
                    if (!HttpRules[i].IsForResponse)
                    {
                        continue;
                    }

                    e = await HttpMocker.Mock(HttpRules[i], e, false);
                }
            }

            HttpResponses.Add(e); // Stores Http Response.
        }

        /// <summary>
        /// Allows overriding default certificate validation logic.
        /// </summary>
        /// <param name="sender">Object defined by the proxy.</param>
        /// <param name="e">Param which contains all the info of the request and is used to edit it.</param>
        /// <returns>Nothing.</returns>
        private Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            // set IsValid to true/false based on Certificate Errors.
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                e.IsValid = true;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Allows overriding default client certificate selection logic during mutual authentication.
        /// </summary>
        /// <param name="sender">Object defined by the proxy.</param>
        /// <param name="e">Param which contains all the info of the request and is used to edit it.</param>
        /// <returns>Nothing.</returns>
        private Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}