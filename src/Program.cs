using System.Net.Http;
using System;
using System.Diagnostics;
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
            /*
            Server server = new Server(IPAddress.Parse("127.0.0.1"), 8887, IPAddress.Parse("127.0.0.1"), 8889);

            Dictionary<string, string> mockerOptions = new() { { MockMatcher.ForHost.GetOptionsKey(), "duckduckgo.com" } };

            MockTransformer mockTransformer = new MockTransformer(null, null, null, null, null, "qwant.com");
            Dictionary<string, object> mockerActionOptions = new() { { MockAction.AutoTransformRequestOrResponse.GetOptionsKey(), mockTransformer } };

            MockerRule rule1 = new MockerRule(MockHttpMethod.Any, MockMatcher.ForHost, mockerOptions, MockAction.TimeoutWithNoResponse, mockerActionOptions);
            server.HttpRules.Add(rule1);


            System.Console.WriteLine(server.Start());

            Console.ReadKey();

            server.Stop();*/

            /*
            Dictionary<string, string> headers = new()
            {
                { "host", "host.com" },
                { "referer", "duckduckgo.com" },
                { "token", "loginToken" },
                { "dnt", "1"},
                { "location", "world"},
                { "city", "universe"},
                { "music", "anything" }
            };

            string body = "dsjhfopwiehf9p29ry829hfe9bnfenbdspfhvdsiophgfvipwehf9802hdsjhfopwiehf9p29ry829hfe9bnfenbdspfhvdsiophgfvipwehf9802h\ndsjhfopwiehf9p29ry829hfe9bnfenbdspfhvdsiophgfvipwehf9802h\tdsjhfopwiehf9p29ry829hfe9bnfenbdspfhvdsiophgfvipwehf9802h\n\ndsjhfopwiehf9p29ry829hfe9bnfenbdspfhvdsiophgfvipwehf9802h";

            HttpResponse response = new HttpResponse(200, headers, body, HttpContentType.TextPlain, false, 1.1);
            await HttpExporter.ExportResponseToHRES(response, "response.hres");

            Dictionary<string, string> headersRequest = new()
            {
                { "host", "host.com" },
                { "referer", "duckduckgo.com" },
                { "token", "loginToken" },
                { "dnt", "1"},
                { "location", "world"},
                { "city", "universe"},
                { "music", "anything" }
            };

            string bodyRequest = "dsjhfopwiehf9p29ry829hfe9bnfenbdspfhvdsiophgfvipwehf9802hdsjhfopwiehf9p29ry829hfe9bnfenbdspfhvdsiophgfvipwehf9802h\ndsjhfopwiehf9p29ry829hfe9bnfenbdspfhvdsiophgfvipwehf9802h\tdsjhfopwiehf9p29ry829hfe9bnfenbdspfhvdsiophgfvipwehf9802h\n\ndsjhfopwiehf9p29ry829hfe9bnfenbdspfhvdsiophgfvipwehf9802h";

            HttpRequest request = new HttpRequest(HttpMethod.Get, "https://www.google.com", headersRequest, bodyRequest, HttpContentType.TextPlain, false, 1.1);
            await HttpExporter.ExportRequestToHREQ(request, "request.hreq");*/


            HttpResponse? response = await HttpImporter.ImportResponseFromHRES("response.hres");

            if (response != null)
            {
                System.Console.WriteLine(response.HttpMethodVersion);
                System.Console.WriteLine(response.StatusCode);
                System.Console.WriteLine(response.BodyString);
            }
        }
    }
#nullable disable
}

// TODO Look for features at Fiddler Everywhere, Fiddler Classic, Wireshark, Http ToolKit, Postman and other tools like that.
// TODO Add python scripting.
// TODO add option to export and load requests and responses from files.
// TODO add a rule only requests (block all the requests besides the ones allowed in the rule).
// TODO stylize the files. (use a code formatter after backing everything up just in case.)
// TODO make mocker able to stop even doing a tunnel connect to a host.
// TODO find a way to bypass HPKP to fix ssl pinning errors
// TODO integrate mocker with tunnel connect request for like: block connection to specific host.
// TODO implement tool like netdiscover in arch linux
// TODO flooding utility and remote flooding maybe (by using a packet sniffer and changing the source ip of the request
//            so the response of the request is sent to a different ip)