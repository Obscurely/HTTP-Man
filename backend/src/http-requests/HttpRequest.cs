using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;

namespace HTTPMan.Http
{
    /// <summary>
    /// Object that represents an http request.
    /// </summary>
    public class HttpRequest
    {
        // Fields
        private readonly HttpMethod _method;
        private readonly string _url;
        private readonly Dictionary<string, string> _headers;
        private readonly string _bodyString;
        private readonly HttpContentType _bodyType;
        private readonly bool _hasBody;
        private readonly bool _keepBody;
        private readonly Version _httpMethodVersion;
        
        // Properties
        public HttpMethod Method { get { return _method; } }
        public string Url { get { return _url; } }
        public Dictionary<string, string> Headers { get { return _headers; } }
        public string BodyString { get { return _bodyString; } }
        public HttpContentType BodyType { get { return _bodyType; } }
        public bool HasBody { get { return _hasBody; } }
        public bool KeepBody { get { return _keepBody; } }
        public Version HttpMethodVersion { get { return _httpMethodVersion; } }

        // Constructors
#nullable enable
        /// <summary>
        /// Creates a request that works with the mocker, requester etc.
        /// </summary>
        /// <param name="method">The method of the request.</param>
        /// <param name="url">The url the request is sent to.</param>
        /// <param name="headers">The headers of the request.</param>
        /// <param name="body">The http content of the request.</param>
        /// <param name="bodyType">The http content type of the request.</param>
        /// <param name="keepBody">Wether to the body of the request should be kept in cache or not.</param>
        /// <param name="httpMethodVersion">The http version used by the request.</param>
        public HttpRequest(HttpMethod method, string url, Dictionary<string, string>? headers = null, string body = "", HttpContentType bodyType = HttpContentType.TextPlain, bool keepBody = false, 
            double httpMethodVersion = 1.1)
        {
            _method = method;
            _url = url;

            // Setting headers if any.
            if (headers != null)
            {
                _headers = headers;
            }  
            else
            {
                _headers = new Dictionary<string, string>();
            }

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
            {
                _httpMethodVersion = HttpVersion.Version10;
            }
            else if (httpMethodVersion == 1.1)
            {
                _httpMethodVersion = HttpVersion.Version11;
            }
            else if (httpMethodVersion == 2.0)
            {
                _httpMethodVersion = HttpVersion.Version20;
            }
            else
            {
                _httpMethodVersion = HttpVersion.Version11;
            }  
        }
#nullable disable
    }
}