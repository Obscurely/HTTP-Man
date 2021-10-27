using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HTTPMan
{
    /// <summary>
    /// Contains all actions related to exporting http stuff to a file.
    /// </summary>
    public class HttpExporter
    {
        /// <summary>
        /// Function that exports an http request to a .hreq file which can be imported back to the program or used for anything else.
        /// </summary>
        /// <param name="request">The request object to export.</param>
        /// <param name="fileLocation">The location where to export the file.</param>
        /// <returns>True if the export was successfull and the task finished, if not it will give an error not false.</returns>
        public static async Task<bool> ExportRequestToHREQ(HttpRequest request, string fileLocation)
        {
            Dictionary<string, object> requestDict = new();

            requestDict.Add("http_version", request.HttpMethodVersion);
            requestDict.Add("http_method", request.Method);
            requestDict.Add("url", request.Url);
            requestDict.Add("headers", request.Headers.ToJsonString());
            requestDict.Add("has_body", request.HasBody);
            requestDict.Add("body_type", request.BodyType.GetString());
            requestDict.Add("body_string", request.BodyString);
            requestDict.Add("keep_body", request.KeepBody);

            await File.WriteAllTextAsync(fileLocation, requestDict.ToJsonString());

            return true;
        }

        /// <summary>
        /// Function that exports an http response to a .hres file which can be imported back to the program or used for anything else.
        /// </summary>
        /// <param name="response">The response object to export.</param>
        /// <param name="fileLocation">The location where to export the file.</param>
        /// <returns>True if the export was successfull and the task finished, if not it will give an error not false.</returns>
        public static async Task<bool> ExportResponseToHRES(HttpResponse response, string fileLocation)
        {
            Dictionary<string, object> responseDict = new();

            responseDict.Add("http_version", response.HttpMethodVersion);
            responseDict.Add("status_code", response.StatusCode);
            responseDict.Add("headers", response.Headers.ToJsonString());
            responseDict.Add("has_body", response.HasBody);
            responseDict.Add("body_type", response.BodyType.GetString());
            responseDict.Add("body_string", response.BodyString);
            responseDict.Add("keep_body", response.KeepBody);

            await File.WriteAllTextAsync(fileLocation, responseDict.ToJsonString());

            return true;
        }
    }
}