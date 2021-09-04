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
            Requester requester = new();

            Dictionary<string, object>? headers = new()
            {
                { "host", "www.youtube.com" },
                { "key2", "12343452" },
                { "key3", "false" },
                { "key4", "null" }
            };

            System.Console.WriteLine(headers.ToJsonString());
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
// TODO: Add python scripting