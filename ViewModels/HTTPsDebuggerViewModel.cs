using HTTPMan.Proxy;
using HTTPMan.Models;

namespace HTTPMan.ViewModels
{
    public class HTTPsDebuggerViewModel : ViewModelBase
    {
        private readonly Server _proxyServer;

        public Server ProxyServer { get { return _proxyServer; } }

        public HTTPsDebuggerViewModel(Utils utils)
        {
            _proxyServer = utils.ProxyServer;
        }
    }
}