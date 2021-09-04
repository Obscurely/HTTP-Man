using System;
using System.Net;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
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

namespace HTTPMan
{
    public class Server
    {
        private readonly ProxyServer _proxyServer = new();
        private bool _isServerStarted = false;
        private bool _isSystemProxySet = false;
        private readonly ExplicitProxyEndPoint _explicitEndPoint;
        private readonly TransparentProxyEndPoint _transparentEndPoint;
        private List<TunnelConnectSessionEventArgs> _tunnelConnectRequests = new();
        private List<SessionEventArgs> _httpRequests = new();
        private List<SessionEventArgs> _httpResponses = new();

        private ProxyServer ProxyServer { get { return _proxyServer; } }
        private bool IsServerStarted { get { return _isServerStarted; } set { _isServerStarted = value; } }
        private bool IsSystemProxySet { get { return _isSystemProxySet; } set { _isSystemProxySet = value; } }
        private ExplicitProxyEndPoint ExplicitEndPoint { get { return _explicitEndPoint; } }
        private TransparentProxyEndPoint TransparentEndPoint { get { return _transparentEndPoint; } }
        public List<TunnelConnectSessionEventArgs> TunnelConnectRequests { get { return _tunnelConnectRequests; } }
        public List<SessionEventArgs> HttpRequests { get { return _httpRequests; } }
        public List<SessionEventArgs> HttpResponses { get { return _httpResponses; } }

        public Server(IPAddress explicitEndPointIP, int explicitEndPointPort, IPAddress transparentEndPointIP, int transparentEndPointPort)
        {
            _explicitEndPoint = new(explicitEndPointIP, explicitEndPointPort, true);
            _transparentEndPoint = new(transparentEndPointIP, transparentEndPointPort, true) { GenericCertificateName = "google.com" };
        }

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

        public byte[] GetRootCertificate()
        {
            return ProxyServer.CertificateManager.RootCertificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert);
        }

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

        private async Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            TunnelConnectRequests.Add(e); //Stores Tunnel Connect Request.
        }

        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            HttpRequests.Add(e); // Stores Http Request.
        }

        private async Task OnResponse(object sender, SessionEventArgs e)
        {
            HttpResponses.Add(e); // Stores Http Response.
        }

        // Allows overriding default certificate validation logic.
        private Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            // set IsValid to true/false based on Certificate Errors.
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                e.IsValid = true;

            return Task.CompletedTask;
        }

        // Allows overriding default client certificate selection logic during mutual authentication.
        private Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}