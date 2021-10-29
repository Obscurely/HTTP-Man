using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;

namespace HTTPMan
{
#nullable enable
    /// <summary>
    /// Object for the auto transform request action in the mocker.
    /// </summary>
    public class MockTransformer
    {
        // Fields
        // For Requests
        private readonly HttpMethod? _requestMethod = null;
        private readonly Dictionary<string, string>? _requestHeaders;
        private readonly string? _requestBodyString;
        private readonly HttpContentType? _requestBodyType = null;
        private readonly bool? _requestKeepBody = null;
        private readonly string? _requestHost;
        private readonly string? _requestUrl;
        private readonly Version? _requestHttpMethodVersion;
        // For Responses
        private readonly int? _responseStatusCode = null;
        private readonly Dictionary<string, string>? _responseHeaders;
        private readonly string? _responseBodyString;
        private readonly HttpContentType? _responseBodyType;
        private readonly bool? _responseKeepBody = null;
        private readonly Version? _responseHttpMethodVersion;

        // Properties
        // For Requests
        public HttpMethod? RequestMethod { get { return _requestMethod; } }
        public Dictionary<string, string>? RequestHeaders { get { return _requestHeaders; } }
        public string? RequestBodyString { get { return _requestBodyString; } }
        public HttpContentType? RequestBodyType { get { return _requestBodyType; } }
        public bool? RequestKeepBody { get { return _requestKeepBody; } }
        public string? RequestHost { get { return _requestHost; } }
        public string? RequestUrl { get { return _requestUrl; } }
        public Version? RequestHttpMethodVersion { get { return _requestHttpMethodVersion; } }
        // For Responses
        public int? ResponseStatusCode { get { return _responseStatusCode; } }
        public Dictionary<string, string>? ResponseHeaders { get { return _responseHeaders; } }
        public string? ResponseBodyString { get { return _responseBodyString; } }
        public HttpContentType? ResponseBodyType { get { return _responseBodyType; } }
        public bool? ResponseKeepBody { get { return _responseKeepBody; } }
        public Version? ResponseHttpMethodVersion { get { return _responseHttpMethodVersion; } }

        // Constructors
        /// <summary>
        /// Creates a mock transformer object to be used in the rule for the auto transform request action.
        /// </summary>
        /// <param name="requestMethod">The method used for the request.</param>
        /// <param name="requestHeaders">The request's headers.</param>
        /// <param name="requestBodyString">The request's body as a string.</param>
        /// <param name="requestBodyType">The request's body type.</param>
        /// <param name="requestKeepBody">Whether the request's body should be cached or not.</param>
        /// <param name="requestHost">The request's host.</param>
        /// <param name="requestUrl">The request's url.</param>
        /// <param name="requestHttpMethodVersion">The request's http version.</param>
        /// <param name="responseStatusCode">The response's status code.</param>
        /// <param name="responseHeaders">The response's headers.</param>
        /// <param name="responseBodyString">The response's body as a string.</param>
        /// <param name="responseBodyType">The response's body type.</param>
        /// <param name="responseKeepBody">Whether the response's body should be cached or not.</param>
        /// <param name="responseHttpMethodVersion">The response's http version.</param>
        public MockTransformer(HttpMethod? requestMethod = null, Dictionary<string, string>? requestHeaders = null, string? requestBodyString = null, HttpContentType? requestBodyType = null,
            bool? requestKeepBody = null, string? requestHost = null, string? requestUrl = null, double? requestHttpMethodVersion = null, int? responseStatusCode = null,
            Dictionary<string, string>? responseHeaders = null, string? responseBodyString = null, HttpContentType? responseBodyType = null, bool? responseKeepBody = null,
            double? responseHttpMethodVersion = null)
        {
            // Request
            _requestMethod = requestMethod;
            _requestHeaders = requestHeaders;
            _requestBodyString = requestBodyString;
            _requestBodyType = requestBodyType;
            _requestKeepBody = requestKeepBody;
            _requestHost = requestHost;
            _requestUrl = requestUrl;

            // Setting http version for request.
            if (requestHttpMethodVersion == 1.0)
                _requestHttpMethodVersion = HttpVersion.Version10;
            else if (requestHttpMethodVersion == 1.1)
                _requestHttpMethodVersion = HttpVersion.Version11;
            else if (requestHttpMethodVersion == 2.0)
                _requestHttpMethodVersion = HttpVersion.Version20;
            else
                _requestHttpMethodVersion = null;

            // Response
            _responseStatusCode = responseStatusCode;
            _responseHeaders = responseHeaders;
            _responseBodyString = responseBodyString;
            _responseBodyType = responseBodyType;
            _responseKeepBody = responseKeepBody;

            // Setting http version for response.
            if (responseHttpMethodVersion == 1.0)
                _responseHttpMethodVersion = HttpVersion.Version10;
            else if (responseHttpMethodVersion == 1.1)
                _responseHttpMethodVersion = HttpVersion.Version11;
            else if (responseHttpMethodVersion == 2.0)
                _responseHttpMethodVersion = HttpVersion.Version20;
            else
                _responseHttpMethodVersion = null;
        }
    }
#nullable disable
}