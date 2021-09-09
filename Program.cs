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
            Dictionary<string, string> test1 = new()
            {
                { "test1", "test1" },
                { "test2", "test2" },
                { "test3", "test3" },
            };
            Dictionary<string, string> test2 = new()
            {
                { "test1", "test1" },
                { "test2", "test2" },
                { "test3", "test3" }
            };

            System.Console.WriteLine(test1.Equals(test2));
        }
    }
}

// TODO: Look for features at Fiddler Everywhere, Fiddler Classic, Wireshark, Http ToolKit, Postman and other tools like that.
// TODO: Look for other client. functions.
// TODO: Add python scripting.
// TODO: add mock action that changes body or headers or both of the mocked/filtered request.
// TODO: add option to export and load requests and responses from files.
// TODO: add extra mock features based on what titanium web proxy gives.