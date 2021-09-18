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
            Dictionary<string, string> mockerOptions = new() { { "regexPattern", "https://discordapp.com/api/v9/channels/.+?/messages" } };
            Dictionary<string, string> newBody = new()
            {
                { "content", "test" }
            };
            MockTransformer transformer = new MockTransformer(null, null, newBody.ToJsonString(), HttpContentType.ApplicationJson, null, null, null, null, null, null, null, null, null, null);
            Dictionary<string, object> mockerActionOptions = new() { { "transformer", transformer } };
            MockerRule rule1 = new MockerRule(MockHttpMethod.Post, MockMatcher.ForUrlsMatchingRegex, mockerOptions, MockAction.AutoTransformRequestOrResponse, mockerActionOptions);
            server.HttpRules.Add(rule1);
            System.Console.WriteLine(server.Start());

            Console.ReadKey();

            server.Stop();
        }

        static void Test(object test)
        {
            Dictionary<string, string> testDict = (Dictionary<string, string>)test;
            System.Console.WriteLine(testDict["email"]);
            System.Console.WriteLine(testDict["phone"]);
            System.Console.WriteLine(testDict["america"]);
        }
    }
#nullable disable
}

// TODO: Look for features at Fiddler Everywhere, Fiddler Classic, Wireshark, Http ToolKit, Postman and other tools like that.
// TODO: Look for other client. functions.
// TODO: Add python scripting.
// TODO: add mock action that changes body or headers or both of the mocked/filtered request.
// TODO: add option to export and load requests and responses from files.
// TODO: add extra mock features based on what titanium web proxy gives.
// TODO: add a rule only requests (block all the requests besides the ones allowed in the rule).
// TODO: add stop from connecting to specific hosts.
// TODO: stylize the files.