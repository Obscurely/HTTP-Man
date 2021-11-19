using System.Net;
using HTTPMan.Proxy;

namespace HTTPMan.Models
{
    public class Utils
    {
        private Server _proxyServer = new(IPAddress.Parse("127.0.0.1"), 8887, IPAddress.Parse("127.0.0.1"), 8889);
        
        public Server ProxyServer { get { return _proxyServer; } }
    }
}