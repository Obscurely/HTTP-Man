using System.Collections.Generic;

namespace HTTPMan
{
#nullable enable
    public class MockTransformer
    {
        // Fields
        private readonly MockHttpMethod? _requestMethod = null;
        private readonly Dictionary<string, string>? _requestHeaders = null;
        private readonly string? _requestBodyString = null;
        private readonly HttpContentType? _requestBodyType = null;
        private readonly MockHttpMethod? _responseMethod = null;
        private readonly Dictionary<string, string>? _responseHeaders = null;
        private readonly string? _responseBodyString = null;
        private readonly HttpContentType? _responseBodyType = null;

        // Properties
        public MockHttpMethod? RequestMethod { get { return _requestMethod; } }
        public Dictionary<string, string>? RequestHeaders { get { return _requestHeaders; } }
        public string? RequestBodyString { get { return _requestBodyString; } }
        public HttpContentType? RequestBodyType { get { return _requestBodyType; } }
        public MockHttpMethod? ResponseMethod { get { return _responseMethod; } }
        public Dictionary<string, string>? ResponseHeaders { get { return _responseHeaders; } }
        public string? ResponseBodyString { get { return _responseBodyString; } }
        public HttpContentType? ResponseBodyType { get { return _responseBodyType; } }

        // Constructors
        public MockTransformer(MockHttpMethod? requestMethod = null, Dictionary<string, string>? requestHeaders = null, string? requestBodyString = null, HttpContentType? requestBodyType = null,
            MockHttpMethod? responseMethod = null, Dictionary<string, string>? responseHeaders = null, string? responseBodyString = null, HttpContentType? responseBodyType = null)
        {
            _requestMethod = requestMethod;
            _requestHeaders = requestHeaders;
            _requestBodyString = requestBodyString;
            _requestBodyType = requestBodyType;
            _responseMethod = responseMethod;
            _responseHeaders = responseHeaders;
            _responseBodyString = responseBodyString;
            _responseBodyType = responseBodyType;
        }
    }
#nullable disable
}