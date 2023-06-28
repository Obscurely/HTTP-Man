using System;
using System.Net;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Titanium.Web.Proxy.EventArguments;
using System.Threading.Tasks;

namespace HTTPMan
{
    /// <summary>
    /// Handles the event argument given by the proxy and mocker rule. Checks if the request is matched and it is manipulates it as the rule says.
    /// </summary>
    public class Mocker
    {
        // Methods
        /// <summary>
        /// Handles the event argument given by the proxy and mocker rule. Checks if the request is matched and it is manipulates it as the rule says.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        public async Task<SessionEventArgs> Mock(MockerRule rule, SessionEventArgs e, bool isRequest)
        {
            if (rule.Matcher == MockMatcher.ForHost && Mocker.IsSameHost(rule, e))
            {
                e = MockRequest(rule, e, isRequest);
            }
            else if (rule.Matcher == MockMatcher.ForUrl && Mocker.IsSameUrl(rule, e))
            {
                e = MockRequest(rule, e, isRequest);
            }
            else if (rule.Matcher == MockMatcher.ForUrlsMatchingRegex && Mocker.IsUrlMatchingRegex(rule, e))
            {
                e = MockRequest(rule, e, isRequest);
            }
            else if (rule.Matcher == MockMatcher.ExactQueryString && Mocker.IsExactQueryString(rule, e))
            {
                e = MockRequest(rule, e, isRequest);
            }   
            else if (rule.Matcher == MockMatcher.IncludingHeaders && Mocker.IsIncludingHeaders(rule, e))
            {
                e = MockRequest(rule, e, isRequest);
            }
            else if (rule.Matcher == MockMatcher.ExactBody && await Mocker.IsExactBody(rule, e).ConfigureAwait(false))
            {
                e = MockRequest(rule, e, isRequest);
            }
            else if (rule.Matcher == MockMatcher.BodyIncluding && await Mocker.IsBodyIncluding(rule, e).ConfigureAwait(false))
            {
                e = MockRequest(rule, e, isRequest);
            }
            else if (rule.Matcher == MockMatcher.ExactJsonBody && await Mocker.IsExactJsonBody(rule, e).ConfigureAwait(false))
            {
                e = MockRequest(rule, e, isRequest);
            }
            else if (rule.Matcher == MockMatcher.JsonBodyIncluding && await Mocker.IsJsonBodyIncluding(rule, e).ConfigureAwait(false))
            {
                e = MockRequest(rule, e, isRequest);
            }      

            return e;
        }

        private static SessionEventArgs MockRequest(MockerRule rule, SessionEventArgs e, bool isRequest)
        {
            if (rule.MockingAction == MockAction.PassRequestToDestination)
            {
                return e;
            }
            else if (rule.MockingAction == MockAction.PauseRequestToManuallyEdit)
            {
                // TODO: implement when making the UI.
            }
            else if (rule.MockingAction == MockAction.PauseResponseToManuallyEdit)
            {
                // TODO: implement when making the UI.
            }
            else if (rule.MockingAction == MockAction.PauseRequestAndResponseToManuallyEdit)
            {
                // TODO: implement when making the UI.
            }
            else if (rule.MockingAction == MockAction.ReturnFixedResponse)
            {
                e = ReturnFixedResponse(rule, e);
            }
            else if (rule.MockingAction == MockAction.ForwardRequestToDifferentHost)
            {
                e = ForwardRequestToDifferentHost(rule, e);
            }
            else if (rule.MockingAction == MockAction.AutoTransformRequestOrResponse)
            {
                e = AutoTransformRequestOrResponse(rule, e, isRequest);
            }
            else if (rule.MockingAction == MockAction.TimeoutWithNoResponse)
            {
                e = TimeOutWithNoResponse(e);
            }
            else if (rule.MockingAction == MockAction.CloseConnectionImmediately)
            {
                e = CloseConnectionImmediately(e);
            }

            return e;
        }

        // *********************************
        // * Checkers based on the matcher *
        // *********************************

        /// <summary>
        /// Checks if the request has the same host as in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>True if the rule and the event argument match otherwise false.</returns>
        private static bool IsSameHost(MockerRule rule, SessionEventArgs e)
        {
            if (rule.MatcherOptions[rule.Matcher.GetOptionsKey()].Equals(e.HttpClient.Request.Host))
            {
                return true;
            }   
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the request has the same url as in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>True if the rule and the event argument match otherwise false.</returns>
        private static bool IsSameUrl(MockerRule rule, SessionEventArgs e)
        {
            if (rule.MatcherOptions[rule.Matcher.GetOptionsKey()].Equals(e.HttpClient.Request.Url))
            {
                return true;
            } 
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the regex pattern in the rule mathes the whole request url.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>True if the rule and the event argument match otherwise false.</returns>
        private static bool IsUrlMatchingRegex(MockerRule rule, SessionEventArgs e)
        {
            string regexPattern = rule.MatcherOptions[rule.Matcher.GetOptionsKey()];
            Regex regexUrl = new(regexPattern);
            string match = regexUrl.Match(e.HttpClient.Request.Url).ToString();

            if (match.Equals(e.HttpClient.Request.Url))
            {
                return true;
            }
            else
            {
                return false;
            }       
        }

        /// <summary>
        /// Checks if the request has the same query string as in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>True if the rule and the event argument match otherwise false.</returns>
        private static bool IsExactQueryString(MockerRule rule, SessionEventArgs e)
        {
            string[] urlSplit = e.HttpClient.Request.Url.Split("?");
            if (urlSplit.Length != 2)
            {
                return false;
            }

            if (urlSplit[1].Split("q=").Length < 2)
            {
                return false;
            }
                
            string queryString = urlSplit[1].Split("q=")[1].Split("&")[0];
            if (rule.MatcherOptions[rule.Matcher.GetOptionsKey()].Equals(queryString))
            {
                return true;
            }
            else
            {
                return false;
            }    
        }

        /// <summary>
        /// Checks if the request includes the headers peresent in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>True if the rule and the event argument match otherwise false.</returns>
        private static bool IsIncludingHeaders(MockerRule rule, SessionEventArgs e)
        {
            Dictionary<string, string> headers = rule.MatcherOptions;

            for (int i = 0; i < headers.Count; i++)
            {
                if (!e.HttpClient.Request.Headers.HeaderExists(headers.Keys.ElementAt(i)))
                {
                    return false;
                }
                else if (!e.HttpClient.Request.Headers.Headers[headers.Keys.ElementAt(i)].Value.Equals(headers.Values.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the request has the same body as in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>True if the rule and the event argument match otherwise false.</returns>
        private static async Task<bool> IsExactBody(MockerRule rule, SessionEventArgs e)
        {
            string body = rule.MatcherOptions[rule.Matcher.GetOptionsKey()];
            string requestBody = await e.GetRequestBodyAsString();

            if (body.Equals(requestBody))
            {
                return true;
            }  
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the request's body includes the body given in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>True if the rule and the event argument match otherwise false.</returns>
        private static async Task<bool> IsBodyIncluding(MockerRule rule, SessionEventArgs e)
        {
            string body = rule.MatcherOptions[rule.Matcher.GetOptionsKey()];
            string requestBody = await e.GetRequestBodyAsString();

            if (requestBody.Split(body).Length >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the request has the same json body as in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>True if the rule and the event argument match otherwise false.</returns>
        private static async Task<bool> IsExactJsonBody(MockerRule rule, SessionEventArgs e)
        {
            string bodyString = rule.MatcherOptions[rule.Matcher.GetOptionsKey()];
            string requestBodyString = await e.GetRequestBodyAsString();

            Dictionary<string, string> body = new();
            Dictionary<string, string> requestBody = new();

            try
            {
                body = JsonSerializer.Deserialize<Dictionary<string, string>>(bodyString);
                requestBody = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBodyString);
            }
            catch (Exception)
            {
                return false;
            }

            if (body.ContentEquals(requestBody))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the request's json body includes the json body given in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>True if the rule and the event argument match otherwise false.</returns>
        private static async Task<bool> IsJsonBodyIncluding(MockerRule rule, SessionEventArgs e)
        {
            string bodyString = rule.MatcherOptions[rule.Matcher.GetOptionsKey()];
            string requestBodyString = await e.GetRequestBodyAsString();

            Dictionary<string, string> body = new();
            Dictionary<string, string> requestBody = new();

            try
            {
                body = JsonSerializer.Deserialize<Dictionary<string, string>>(bodyString);
                requestBody = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBodyString);
            }
            catch (Exception)
            {
                return false;
            }

            for (int i = 0; i < body.Count; i++)
            {
                if (!requestBody.ContainsKey(body.Keys.ElementAt(i)))
                {
                    return false;
                }

                if (!requestBody[body.Keys.ElementAt(i)].Equals(body.Values.ElementAt(i)))
                {
                    return false;
                }   
            }

            return true;
        }

        // *****************************
        // * Request/Response Changers *
        // *****************************

        /// <summary>
        /// Gives back for request a fixed response.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>The modified event argument.</returns>
        private static SessionEventArgs ReturnFixedResponse(MockerRule rule, SessionEventArgs e)
        {
            HttpResponse response = (HttpResponse)(rule.MockingActionOptions[rule.MockingAction.GetOptionsKey()]);

            e.HttpClient.Response.StatusCode = response.StatusCode;

            // Setting headers.
            e.HttpClient.Response.Headers.Clear();
            if (response.Headers.Count != 0)
            {
                e.HttpClient.Response.Headers.AddHeaders(response.Headers);
            }

            e.SetResponseBodyString(response.BodyString);
            e.HttpClient.Response.ContentType = response.BodyType.GetString();
            e.HttpClient.Response.KeepBody = response.KeepBody;
            e.HttpClient.Response.HttpVersion = response.HttpMethodVersion;

            return e;
        }

        /// <summary>
        /// Forwards the given request to a different host, for example if you have like www.google.com/test chaning the host will make it be like www.duckduckgo.com/test
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>The modified event argument.</returns>
        private static SessionEventArgs ForwardRequestToDifferentHost(MockerRule rule, SessionEventArgs e)
        {
            string originalHost = e.HttpClient.Request.RequestUri.Host;
            string newHost = (string)(rule.MockingActionOptions[rule.MockingAction.GetOptionsKey()]);

            e.HttpClient.Request.Url = e.HttpClient.Request.Url.ReplaceFirst(originalHost, newHost);
            e.HttpClient.Request.RequestUriString = e.HttpClient.Request.RequestUriString.ReplaceFirst(originalHost, newHost);
            e.HttpClient.Request.Host = newHost;
            e.HttpClient.Request.Headers.RemoveHeader("host");
            e.HttpClient.Request.Headers.AddHeader("host", newHost);

            return e;
        }

        /// <summary>
        /// Auto transforms the given request using the transformer object given with the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <param name="transformer">The transformer object used for transforming the request</param>
        /// <returns>The modified event argument.</returns>
        private static SessionEventArgs AutoTransformRequest(SessionEventArgs e, MockTransformer transformer)
        {
            // Setting http method if any.
            if (transformer.RequestMethod != null)
            {
                e.HttpClient.Request.Method = (string)(transformer.RequestMethod).ToString();
            }

            // Setting headers if any.
            if (transformer.RequestHeaders != null)
            {
                e.HttpClient.Request.Headers.Clear();
                if (transformer.RequestHeaders.Count != 0)
                {
                    e.HttpClient.Request.Headers.AddHeaders((Dictionary<string, string>)(transformer.RequestHeaders));
                } 
            }

            // Setting body type if any.
            if (transformer.RequestBodyType != null)
            {
                e.HttpClient.Request.ContentType = transformer.RequestBodyType.GetString();
            }

            // Setting request body if any.
            if (transformer.RequestBodyString != null)
            {
                e.SetRequestBodyString((string)(transformer.RequestBodyString));
            }

            // Setting request keep body if any.
            if (transformer.RequestKeepBody != null)
            {
                e.HttpClient.Request.KeepBody = (bool)transformer.RequestKeepBody;
            }

            // Changing host if any given.
            if (transformer.RequestHost != null)
            {
                string originalHost = e.HttpClient.Request.RequestUri.Host;
                string newHost = transformer.RequestHost;

                e.HttpClient.Request.Url = e.HttpClient.Request.Url.ReplaceFirst(originalHost, newHost);
                e.HttpClient.Request.RequestUriString = e.HttpClient.Request.RequestUriString.ReplaceFirst(originalHost, newHost);
                e.HttpClient.Request.Host = newHost;
                e.HttpClient.Request.Headers.RemoveHeader("host");
                e.HttpClient.Request.Headers.AddHeader("host", newHost);
            }

            // Changing url if any given.
            if (transformer.RequestUrl != null)
            {
                string oldRequestUrl = e.HttpClient.Request.Url;
                string oldRequestHost = e.HttpClient.Request.Host;

                try
                {
                    Uri newUri = new((string)(transformer.RequestUrl));

                    e.HttpClient.Request.RequestUri = newUri;
                    e.HttpClient.Request.Url = (string)(transformer.RequestUrl);
                    e.HttpClient.Request.Host = newUri.Host;
                    e.HttpClient.Request.Headers.RemoveHeader("host");
                    e.HttpClient.Request.Headers.AddHeader("host", newUri.Host);
                }
                catch (Exception)
                {
                    e.HttpClient.Request.RequestUriString = oldRequestUrl;
                    e.HttpClient.Request.Host = oldRequestHost;
                }
            }

            // Setting http version if any.
            if (transformer.RequestHttpMethodVersion != null)
            {
                e.HttpClient.Request.HttpVersion = transformer.RequestHttpMethodVersion;
            } 

            return e;
        }
        
        /// <summary>
        /// Auto transforms the given response using the transformer object given with the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <param name="transformer">The transformer object used for transforming the response</param>
        /// <returns>The modified event argument.</returns>
        private static SessionEventArgs AutoTransformResponse(SessionEventArgs e, MockTransformer transformer)
        {
            // Setting response status code if any given.
            if (transformer.ResponseStatusCode != null)
            {
                e.HttpClient.Response.StatusCode = (int)transformer.ResponseStatusCode;
            }

            // Setting headers if any.
            if (transformer.ResponseHeaders != null)
            {
                e.HttpClient.Response.Headers.Clear();
                if (transformer.ResponseHeaders.Count != 0)
                {
                    e.HttpClient.Response.Headers.AddHeaders(transformer.ResponseHeaders);
                }
            }

            // Setting body type if any.
            if (transformer.ResponseBodyType != null)
            {
                e.HttpClient.Response.ContentType = transformer.ResponseBodyType.GetString();
            }

            // Setting response body if any.
            if (transformer.ResponseBodyString != null)
            {
                e.SetResponseBodyString(transformer.ResponseBodyString);
            }

            // Setting response keep body if any.
            if (transformer.ResponseKeepBody != null)
            {
                e.HttpClient.Response.KeepBody = (bool)transformer.ResponseKeepBody;
            }

            // Setting http version if any.
            if (transformer.ResponseHttpMethodVersion != null)
            {
                e.HttpClient.Response.HttpVersion = (Version)(transformer.ResponseHttpMethodVersion);
            }

            return e;
        }

        /// <summary>
        /// Auto transforms the given request or response using the transformer object given with the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <param name="isRequest">Whether the current request changing is a request or a response.</param>
        /// <returns>The modified event argument.</returns>
        public static SessionEventArgs AutoTransformRequestOrResponse(MockerRule rule, SessionEventArgs e, bool isRequest)
        {
            MockTransformer transformer = (MockTransformer)(rule.MockingActionOptions[rule.MockingAction.GetOptionsKey()]);

            if (isRequest)
            {
                return AutoTransformRequest(e, transformer);
            }   
            else
            {
                return AutoTransformResponse(e, transformer);
            }
        }

        /// <summary>
        /// Returns back after request an empty response that has as the status code RequestTimeout.
        /// </summary>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>The modified event argument.</returns>
        public static SessionEventArgs TimeOutWithNoResponse(SessionEventArgs e)
        {
            Dictionary<string, Titanium.Web.Proxy.Models.HttpHeader> headers = new();
            e.GenericResponse("", HttpStatusCode.RequestTimeout, headers, false);

            return e;
        }

        /// <summary>
        /// Closes the connection immediately.
        /// </summary>
        /// <param name="e">Event argument given by the proxy.</param>
        /// <returns>The modified event argument.</returns>
        public static SessionEventArgs CloseConnectionImmediately(SessionEventArgs e)
        {
            // Terminating connection and session.
            e.TerminateServerConnection();
            e.TerminateSession();
            e.Dispose();

            return e;
        }
    }
}