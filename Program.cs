using System.Net.Http;
using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

#pragma warning disable CS1998 // disables the warning "async method lacks await operators and will run synchronously" as the main method is always async for developing purposes.

namespace HTTPMan
{
#nullable enable
    class Program
    {
        static async Task Main(string[] args)
        {
            Server server = new Server(IPAddress.Parse("127.0.0.1"), 8887, IPAddress.Parse("127.0.0.1"), 8889);

            Dictionary<string, string> mockerOptions = new() { { MockMatcher.ForHost.GetOptionsKey(), "duckduckgo.com" } };

            MockTransformer mockTransformer = new MockTransformer(null, null, null, null, null, "qwant.com");
            Dictionary<string, object> mockerActionOptions = new() { { MockAction.AutoTransformRequestOrResponse.GetOptionsKey(), mockTransformer } };

            MockerRule rule1 = new MockerRule(MockHttpMethod.Any, MockMatcher.ForHost, mockerOptions, MockAction.TimeoutWithNoResponse, mockerActionOptions);
            server.HttpRules.Add(rule1);


            System.Console.WriteLine(server.Start());

            Console.ReadKey();

            server.Stop();
        }
    }
#nullable disable
}

// TODO Look for features at Fiddler Everywhere, Fiddler Classic, Wireshark, Http ToolKit, Postman and other tools like that.
// TODO Look for other client. functions.
// TODO Add python scripting.
// TODO add option to export and load requests and responses from files.
// TODO add extra mock features based on what titanium web proxy gives.
// TODO add a rule only requests (block all the requests besides the ones allowed in the rule).
// TODO add stop from connecting to specific hosts.
// TODO stylize the files.
// TODO fix ssl pinning errors
// TODO make mocker able to stop even doing a tunnel connect to a host.
// TODO find a way to bypass HPKP