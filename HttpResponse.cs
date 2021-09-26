using System;
using System.Net;
using System.Collections.Generic;

namespace HTTPMan
{
    public class HttpResponse
    {
        // Fields
        private readonly int _statusCode;
        private readonly Dictionary<string, string> _headers;
        private readonly string _bodyString;
        private readonly HttpContentType _bodyType;
        private readonly bool _hasBody = false;
        private readonly bool _keepBody;
        private readonly Version _httpMethodVersion;

        // Properties
        public int StatusCode { get { return _statusCode; } }
        public Dictionary<string, string> Headers { get { return _headers; } }
        public string BodyString { get { return _bodyString; } }
        public HttpContentType BodyType { get { return _bodyType; } }
        public bool HasBody { get { return _hasBody; } }
        public bool KeepBody { get { return _keepBody; } }
        public Version HttpMethodVersion { get { return _httpMethodVersion; } }

        // Constructors
#nullable enable
        /// <summary>
        /// Creates a response that works with the mocker.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="headers">The headers of the response.</param>
        /// <param name="body">The http content of the response.</param>
        /// <param name="bodyType">The http content type of the response.</param>
        /// <param name="keepBody">Wether to the body of the response should be kept in cache or not.</param>
        /// <param name="httpMethodVersion">The http version used by the request/response.</param>
        public HttpResponse(int statusCode = 404, Dictionary<string, string>? headers = null, string body = "", HttpContentType bodyType = HttpContentType.TextPlain, bool keepBody = false,
            double httpMethodVersion = 1.1)
        {
            _statusCode = statusCode;

            // Setting headers if any.
            if (headers != null)
                _headers = headers;
            else
                _headers = new Dictionary<string, string>() { };

            // Setting body if any.
            if (!body.Equals(""))
            {
                _bodyString = body;
                _hasBody = true;
            }
            else
            {
                _bodyString = "";
                _hasBody = false;
            }

            _bodyType = bodyType;
            _keepBody = keepBody;

            // Setting http version.
            if (httpMethodVersion == 1.0)
                _httpMethodVersion = HttpVersion.Version10;
            else if (httpMethodVersion == 1.1)
                _httpMethodVersion = HttpVersion.Version11;
            else if (httpMethodVersion == 2.0)
                _httpMethodVersion = HttpVersion.Version20;
            else
                _httpMethodVersion = HttpVersion.Version11;
        }
#nullable disable
    }
}