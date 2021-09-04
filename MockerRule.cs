using System;
using System.Net.Http;
using System.Collections.Generic;

namespace HTTPMan
{
    public enum MockMatcher
    {
        ForHost = 0,
        ForUrl = 1,
        ForUrlsMatchingRegex = 2,
        ExactQueryString = 3,
        IncludingHeaders = 4,
        ExactBody = 5,
        BodyIncluding = 6,
        ExactJsonBody = 7,
        JsonBodyIncluding = 8
    }

    public enum MockAction
    {
        PassRequestToDestination = 0,
        PauseRequestToManuallyEdit = 1,
        PauseResponseToManuallyEdit = 2,
        PauseRequestAndResponseToManuallyEdit = 3,
        ReturnFixedResponse = 4,
        ForwardRequestToDifferentHost = 5,
        AutoTransformRequestOrResponse = 6,
        TimeoutWithNoResponse = 7,
        CloseConnectionImmediately = 8
    }

    public class MockerRule
    {
        private readonly HttpMethod _method;
        private readonly MockMatcher _matcher;
        private readonly Dictionary<string, string> _matcherOptions;
        private readonly MockAction _mockingAction;
        private readonly Dictionary<string, object> _mockingActionOptions;

        public HttpMethod Method { get { return _method; } }
        public MockMatcher Matcher { get { return _matcher; } }
        public Dictionary<string, string> MatcherOptions { get { return _matcherOptions; } }
        public MockAction MockingAction { get { return _mockingAction; } }
        public Dictionary<string, object> MockingActionOptions { get { return _mockingActionOptions; } }

        public MockerRule(HttpMethod method, MockMatcher matcher, Dictionary<string, string> matcherOptions, MockAction mockingAction, Dictionary<string, object> mockingActionOptions)
        {
            _method = method;
            _matcher = matcher;
            _matcherOptions = matcherOptions;
            _mockingAction = mockingAction;
            _mockingActionOptions = mockingActionOptions;
        }
    }
}