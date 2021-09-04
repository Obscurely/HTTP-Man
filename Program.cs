using System.Net.Http;
using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HTTPMan
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine(HttpMethod.Get.ToString().Equals("GET"));
            /*
            Server server = new Server(IPAddress.Parse("127.0.0.1"), 8866, IPAddress.Parse("127.0.0.1"), 8867);
            server.Start();
            server.TrustCertificate();
            Console.ReadKey();
            server.Stop();*/
        }
    }
}

// TODO: Look for features at Fiddler Everywhere, Fiddler Classic, Wireshark, Http ToolKit, Postman and other tools like that.
// TODO: Look for other client. functions.
// TODO: Add python scripting.
// TODO: add mock action that changes body or headers or both of the mocked/filtered request.
// TODO: add option to export and load requests and responses from files.
// TODO: add extra mock features based on what titanium web proxy gives.