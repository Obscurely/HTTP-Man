using System;
using System.Net.Http;
using System.Collections.Generic;

namespace HTTPMan
{
    /// <summary>
    /// Enum that contains the http methods supported by the mocker as well as an ANY option.
    /// </summary>
    public enum MockHttpMethod
    {
        Get = 0,
        Post = 1,
        Put = 2,
        Patch = 3,
        Delete = 4,
        Head = 5,
        Options = 6,
        Trace = 7,
        Any = 8
    }

    /// <summary>
    /// Enum that contains the mather options supported by the mocker.
    /// </summary>
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

    /// <summary>
    /// Enum that contains the actions supported by the mocker when a request is matched.
    /// </summary>
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

    /// <summary>
    /// Class that creates a mocker rule object that can be added to the server's list and be used on incoming requests.
    /// </summary>
    public class MockerRule
    {
        private readonly MockHttpMethod _method;
        private readonly MockMatcher _matcher;
        private readonly Dictionary<string, string> _matcherOptions;
        private readonly MockAction _mockingAction;
        private readonly Dictionary<string, object> _mockingActionOptions;
        private readonly bool _isForRequest = false;
        private readonly bool _isForResponse = false;
        private readonly bool _isForTunnelConnect = false;

        public MockHttpMethod Method { get { return _method; } }
        public MockMatcher Matcher { get { return _matcher; } }
        public Dictionary<string, string> MatcherOptions { get { return _matcherOptions; } }
        public MockAction MockingAction { get { return _mockingAction; } }
        public Dictionary<string, object> MockingActionOptions { get { return _mockingActionOptions; } }
        public bool IsForRequest { get { return _isForRequest; } }
        public bool IsForResponse { get { return _isForResponse; } }
        public bool IsForTunnelConnect { get { return _isForTunnelConnect; } }

        /// <summary>
        /// Creates a rule with specified information.
        /// </summary>
        /// <param name="method">Http Method the mocker should match for (or ANY).</param>
        /// <param name="matcher">Matching method to match only specific request for modifying.</param>
        /// /// <param name="matcherOptions">Information for the matching method in order for the program to know how to use it.</param>
        /// <param name="mockingAction">What method should the server use if the specific request is matched.</param>
        /// <param name="mockingActionOptions">Information for mocking action to know what to do.</param>
        public MockerRule(MockHttpMethod method, MockMatcher matcher, Dictionary<string, string> matcherOptions, MockAction mockingAction, Dictionary<string, object> mockingActionOptions)
        {
            _method = method;
            _matcher = matcher;
            _matcherOptions = matcherOptions;
            _mockingAction = mockingAction;
            _mockingActionOptions = mockingActionOptions;

            if (mockingAction == MockAction.PassRequestToDestination)
            {
                _isForRequest = true;
            }
            else if (mockingAction == MockAction.PauseRequestToManuallyEdit)
            {
                _isForRequest = true;
            }
            else if (mockingAction == MockAction.PauseResponseToManuallyEdit)
            {
                _isForResponse = true;
            }
            else if (mockingAction == MockAction.PauseRequestAndResponseToManuallyEdit)
            {
                _isForRequest = true;
                _isForResponse = true;
            }
            else if (mockingAction == MockAction.ReturnFixedResponse)
            {
                _isForResponse = true;
            }
            else if (mockingAction == MockAction.ForwardRequestToDifferentHost)
            {
                _isForRequest = true;
            }
            else if (mockingAction == MockAction.AutoTransformRequestOrResponse)
            {
                _isForRequest = true;
                _isForResponse = true;
            }
            else if (mockingAction == MockAction.TimeoutWithNoResponse)
            {
                _isForRequest = true;
                _isForResponse = true;
            }
            else if (mockingAction == MockAction.CloseConnectionImmediately)
            {
                _isForTunnelConnect = true;
                _isForRequest = true;
                _isForResponse = true;
            }
        }
    }
}