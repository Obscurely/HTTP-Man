using System;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HTTPMan
{
    public class HttpImporter
    {
        /// <summary>
        /// Coverts a hreq file containing a request to a HttpRequest object.
        /// </summary>
        /// <param name="fileLocation">the location of the file</param>
        /// <returns>An HttpRequest object which cand either be fully intialized or null in case the input file was not valid.</returns>
#nullable enable
        public static async Task<HttpRequest?> ImportRequestFromHREQ(string fileLocation)
        {
            string hreqFile = await File.ReadAllTextAsync(fileLocation).ConfigureAwait(false);
            Dictionary<string, object>? requestDict = HttpImporter.RequestToDict(hreqFile);
            if (requestDict == null)
                return null;

            HttpRequest request = new HttpRequest((HttpMethod)requestDict["http_method"], (string)requestDict["url"], (Dictionary<string, string>)requestDict["headers"],
                (string)requestDict["body_string"], (HttpContentType)requestDict["body_type"], (bool)requestDict["keep_body"], (double)requestDict["http_version"]);

            return request;
        }
#nullable disable

        /// <summary>
        /// Coverts a hres file containing a response to a HttpResponse object.
        /// </summary>
        /// <param name="fileLocation">the location of the file</param>
        /// <returns>An HttpResponse object which cand either be fully intialized or null in case the input file was not valid.</returns>
#nullable enable
        public static async Task<HttpResponse?> ImportResponseFromHRES(string fileLocation)
        {
            string hresFile = await File.ReadAllTextAsync(fileLocation).ConfigureAwait(false);
            Dictionary<string, object>? responseDict = HttpImporter.ResponseToDict(hresFile);
            if (responseDict == null)
                return null;

            HttpResponse response = new HttpResponse((int)responseDict["status_code"], (Dictionary<string, string>)responseDict["headers"],
                (string)responseDict["body_string"], (HttpContentType)responseDict["body_type"], (bool)responseDict["keep_body"], (double)responseDict["http_version"]);

            return response;
        }
#nullable disable

        /// <summary>
        /// Checks, validates and converts a .hreq file into a usable Dictionary<string, object> for code and if file is invalid returns null.
        /// </summary>
        /// <param name="requestJson">The file to import from as a string.</param>
        /// <returns>Request as a Dictionary<string, object> if file is valid and null if file is invalid.</returns>
#nullable enable
        public static Dictionary<string, object>? RequestToDict(string requestJson)
        {
            Dictionary<string, object> request = new();

            // Validates and adds http_version to the dict.
            string patternHttpVersion = "(?<=\"http_version\": )[0-9]*\\.[0-9]+(?=,\n)";
            double httpVersion;
            double[] availableHttpVersion = new double[] { 1.0, 1.1, 2.0 };
            if (!double.TryParse(Regex.Match(requestJson, patternHttpVersion).ToString(), out httpVersion))
                return null;
            else if (Array.IndexOf(availableHttpVersion, httpVersion) == -1)
                return null;
            request.Add("http_version", httpVersion);

            // Validates and adds http_method to the dict.
            string patternHttpMethod = "(?<=\"http_method\": \")[a-zA-Z]+(?=\",\n)";
            string httpMethodString = Regex.Match(requestJson, patternHttpMethod).ToString();
            string[] availableHttpMethods = new string[] { "GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "TRACE", "OPTIONS" };
            if (Array.IndexOf(availableHttpMethods, httpMethodString.ToUpper()) == -1)
                return null;
            request.Add("http_method", new HttpMethod(httpMethodString.ToUpper()));

            // Validates and adds url to the dict.
            string patternUrl = "(?<=\"url\": \")[a-zA-Z]+?://.*?\\.*?\\..*?(?=\",\n)";
            string url = Regex.Match(requestJson, patternUrl).ToString();
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute) || url.Equals(string.Empty))
                return null;
            request.Add("url", url);

            // Validates and adds headers to the dict.
            string patternHeaders = "(?<=\"headers\": \"){\n((.|\n)*)}(?=\",\n\t\"has_body\")";
            string headersJsonString = Regex.Match(requestJson, patternHeaders).ToString();
            Dictionary<string, string> headers = headersJsonString.ToDict();
            request.Add("headers", headers);

            // Validates and adds has_body to the dict.
            string patternHasBody = "(?<=\"has_body\": ).+?(?=,\n)";
            string hasBodyString = Regex.Match(requestJson, patternHasBody).ToString();
            bool hasBody;
            if (!bool.TryParse(hasBodyString, out hasBody))
                return null;
            request.Add("has_body", hasBody);

            // Validates and adds body_type to the dict.
            string patternBodyType = "(?<=\"body_type\": \").+?(?=\",\n)";
            string bodyTypeString = Regex.Match(requestJson, patternBodyType).ToString();
            HttpContentType? bodyType = bodyTypeString.ToHttpContentType();
            if (bodyType == null)
                return null;
            request.Add("body_type", (HttpContentType)bodyType);

            // Validates and adds body_string to the dict.
            string patternBodyString = "(?<=\"body_string\": \")((.|\n)*)(?=\",\n\t\"keep_body\": )";
            string bodyString = Regex.Match(requestJson, patternBodyString).ToString();
            request.Add("body_string", bodyString);

            // Validates and adds keep_body to the dict.
            string patternKeepBody = "(?<=\"keep_body\": ).+?(?=,\n})";
            string keepBodyString = Regex.Match(requestJson, patternKeepBody).ToString();
            bool keepBody;
            if (!bool.TryParse(keepBodyString, out keepBody))
                return null;
            request.Add("keep_body", keepBody);

            // Finally return back the checked and validated dictionary object containing the request.
            return request;
        }
#nullable disable

        /// <summary>
        /// Checks, validates and converts a .hres file into a usable dictionary<string, object> for code and if file is invalid returns null.
        /// </summary>
        /// <param name="responseJson">The file to import from as a string.</param>
        /// <returns>Response as a Dictionary<string, object> if file is valid and null if file is invalid.</returns>
#nullable enable
        public static Dictionary<string, object>? ResponseToDict(string responseJson)
        {
            Dictionary<string, object> response = new();

            // Validates and adds http_version to the dict.
            string patternHttpVersion = "(?<=\"http_version\": )[0-9]*\\.[0-9]+(?=,\n)";
            double httpVersion;
            double[] availableHttpVersion = new double[] { 1.0, 1.1, 2.0 };
            if (!double.TryParse(Regex.Match(responseJson, patternHttpVersion).ToString(), out httpVersion))
                return null;
            else if (Array.IndexOf(availableHttpVersion, httpVersion) == -1)
                return null;
            response.Add("http_version", httpVersion);

            // Validates and adds status_code to the dict.
            string patternStatusCode = "(?<=\"status_code\": )[0-9]*?(?=,\n)";
            int statusCode;
            if (!int.TryParse(Regex.Match(responseJson, patternStatusCode).ToString(), out statusCode))
                return null;
            response.Add("status_code", statusCode);

            // Validates and adds headers to the dict.
            string patternHeaders = "(?<=\"headers\": \"){\n((.|\n)*)}(?=\",\n\t\"has_body\")";
            string headersJsonString = Regex.Match(responseJson, patternHeaders).ToString();
            Dictionary<string, string> headers = headersJsonString.ToDict();
            response.Add("headers", headers);

            // Validates and adds has_body to the dict.
            string patternHasBody = "(?<=\"has_body\": ).+?(?=,\n)";
            string hasBodyString = Regex.Match(responseJson, patternHasBody).ToString();
            bool hasBody;
            if (!bool.TryParse(hasBodyString, out hasBody))
                return null;
            response.Add("has_body", hasBody);

            // Validates and adds body_type to the dict.
            string patternBodyType = "(?<=\"body_type\": \").+?(?=\",\n)";
            string bodyTypeString = Regex.Match(responseJson, patternBodyType).ToString();
            HttpContentType? bodyType = bodyTypeString.ToHttpContentType();
            if (bodyType == null)
                return null;
            response.Add("body_type", (HttpContentType)bodyType);

            // Validates and adds body_string to the dict.
            string patternBodyString = "(?<=\"body_string\": \")((.|\n)*)(?=\",\n\t\"keep_body\": )";
            string bodyString = Regex.Match(responseJson, patternBodyString).ToString();
            response.Add("body_string", bodyString);

            // Validates and adds keep_body to the dict.
            string patternKeepBody = "(?<=\"keep_body\": ).+?(?=,\n})";
            string keepBodyString = Regex.Match(responseJson, patternKeepBody).ToString();
            bool keepBody;
            if (!bool.TryParse(keepBodyString, out keepBody))
                return null;
            response.Add("keep_body", keepBody);

            // Finally return back the checked and validated dictionary object containing the response.
            return response;
        }
#nullable disable
    }
}