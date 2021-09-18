using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Titanium.Web.Proxy.EventArguments;

namespace HTTPMan
{
    /// <summary>
    /// Handles the event arguments given by the proxy and mocker rule. Checks if the request is matched and it is manipulates it as the rule says.
    /// </summary>
    public class Mocker
    {
        // Methods
        /// <summary>
        /// Handles the event arguments given by the proxy and mocker rule. Checks if the request is matched and it is manipulates it as the rule says.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event arguments given by the proxy.</param>
        public void Mock(MockerRule rule, SessionEventArgs e)
        {
            if (rule.Matcher == MockMatcher.ForHost)
            {
                if (Mocker.IsSameHost(rule, e))
                {
                    // . . .
                }
            }
            else if (rule.Matcher == MockMatcher.ForUrl)
            {
                if (Mocker.IsSameUrl(rule, e))
                {
                    // . . .
                }
            }
            else if (rule.Matcher == MockMatcher.ForUrlsMatchingRegex)
            {
                if (Mocker.IsUrlMatchingRegex(rule, e))
                {
                    // . . .
                }
            }
            else if (rule.Matcher == MockMatcher.ExactQueryString)
            {
                if (Mocker.IsSameQueryString(rule, e))
                {
                    // . . .
                }
            }
            else if (rule.Matcher == MockMatcher.IncludingHeaders)
            {
                if (Mocker.IsIncludingHeaders(rule, e))
                {
                    // . . .
                }
            }
            else if (rule.Matcher == MockMatcher.ExactBody)
            {
                if (Mocker.IsSameBody(rule, e))
                {
                    // . . .
                }
            }
            else if (rule.Matcher == MockMatcher.BodyIncluding)
            {
                if (Mocker.IsBodyIncluding(rule, e))
                {
                    // . . .
                }
            }
            else if (rule.Matcher == MockMatcher.ExactJsonBody)
            {
                if (Mocker.IsSameJsonBody(rule, e))
                {
                    // . . .
                }
            }
            else if (rule.Matcher == MockMatcher.JsonBodyIncluding)
            {
                if (Mocker.IsJsonBodyIncluding(rule, e))
                {
                    // . . .
                }
            }
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
                // AutoTransformRequestOrResponse(rule, e);
            }
            else if (rule.MockingAction == MockAction.TimeoutWithNoResponse)
            {
                // TimeoutWithNoResponse(rule, e);
            }
            else if (rule.MockingAction == MockAction.CloseConnectionImmediately)
            {
                // CloseConnectionImmediately(rule, e);
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
        /// <param name="e">Event arguments given by the proxy.</param>
        /// <returns>Returns true if the rule and the event arguments match otherwise false.</returns>
        private static bool IsSameHost(MockerRule rule, SessionEventArgs e)
        {
            if (rule.MatcherOptions[rule.Matcher.GetOptionsKey()].Equals(e.HttpClient.Request.Host))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the request has the same url as in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event arguments given by the proxy.</param>
        /// <returns>Returns true if the rule and the event arguments match otherwise false.</returns>
        private static bool IsSameUrl(MockerRule rule, SessionEventArgs e)
        {
            if (rule.MatcherOptions[rule.Matcher.GetOptionsKey()].Equals(e.HttpClient.Request.Url))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the regex pattern in the rule mathes the whole request url.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event arguments given by the proxy.</param>
        /// <returns>Returns true if the rule and the event arguments match otherwise false.</returns>
        private static bool IsUrlMatchingRegex(MockerRule rule, SessionEventArgs e)
        {
            string regexPattern = rule.MatcherOptions[rule.Matcher.GetOptionsKey()];
            Regex regexUrl = new(regexPattern);
            string match = regexUrl.Match(e.HttpClient.Request.Url).ToString();

            if (match.Equals(e.HttpClient.Request.Url))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the request has the same query string as in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event arguments given by the proxy.</param>
        /// <returns>Returns true if the rule and the event arguments match otherwise false.</returns>
        private static bool IsSameQueryString(MockerRule rule, SessionEventArgs e)
        {
            string[] urlSplit = e.HttpClient.Request.Url.Split("?");
            if (urlSplit.Length != 2)
                return false;

            string queryString = urlSplit[1];
            if (rule.MatcherOptions[rule.Matcher.GetOptionsKey()].Equals(queryString))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the request includes the headers peresent in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event arguments given by the proxy.</param>
        /// <returns>Returns true if the rule and the event arguments match otherwise false.</returns>
        private static bool IsIncludingHeaders(MockerRule rule, SessionEventArgs e)
        {
            Dictionary<string, string> headers = rule.MatcherOptions;

            for (int i = 0; i < headers.Count; i++)
            {
                if (!e.HttpClient.Request.Headers.HeaderExists(headers.Keys.ElementAt(i)))
                {
                    return false;
                }
                else
                {
                    if (!e.HttpClient.Request.Headers.Headers[headers.Keys.ElementAt(i)].Value.Equals(headers.Values.ElementAt(i)))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the request has the same body as in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event arguments given by the proxy.</param>
        /// <returns>Returns true if the rule and the event arguments match otherwise false.</returns>
        private static bool IsSameBody(MockerRule rule, SessionEventArgs e)
        {
            string body = rule.MatcherOptions[rule.Matcher.GetOptionsKey()];
            string requestBody = e.HttpClient.Request.BodyString;

            if (body.Equals(requestBody))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the request's body includes the body given in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event arguments given by the proxy.</param>
        /// <returns>Returns true if the rule and the event arguments match otherwise false.</returns>
        private static bool IsBodyIncluding(MockerRule rule, SessionEventArgs e)
        {
            string body = rule.MatcherOptions[rule.Matcher.GetOptionsKey()];
            string requestBody = e.HttpClient.Request.BodyString;

            if (requestBody.Split(body).Length >= 2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the request has the same json body as in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event arguments given by the proxy.</param>
        /// <returns>Returns true if the rule and the event arguments match otherwise false.</returns>
        private static bool IsSameJsonBody(MockerRule rule, SessionEventArgs e)
        {
            string bodyString = rule.MatcherOptions[rule.Matcher.GetOptionsKey()];
            string requestBodyString = e.HttpClient.Request.BodyString;

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
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the request's json body includes the json body given in the rule.
        /// </summary>
        /// <param name="rule">The mocker rule.</param>
        /// <param name="e">Event arguments given by the proxy.</param>
        /// <returns>Returns true if the rule and the event arguments match otherwise false.</returns>
        private static bool IsJsonBodyIncluding(MockerRule rule, SessionEventArgs e)
        {
            string bodyString = rule.MatcherOptions[rule.Matcher.GetOptionsKey()];
            string requestBodyString = e.HttpClient.Request.BodyString;

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
                    return false;

                if (!requestBody[body.Keys.ElementAt(i)].Equals(body.Values.ElementAt(i)))
                    return false;
            }

            return true;
        }


        // *****************************
        // * Request/Response Changers *
        // *****************************

        private static SessionEventArgs ReturnFixedResponse(MockerRule rule, SessionEventArgs e)
        {
            HttpResponse response = (HttpResponse)(rule.MockingActionOptions[rule.MockingAction.GetOptionsKey()]);

            e.HttpClient.Response.StatusCode = response.StatusCode;

            // Setting headers.
            e.HttpClient.Response.Headers.Clear();
            if (response.Headers.Count != 0)
                e.HttpClient.Response.Headers.AddHeaders(response.Headers);

            e.SetResponseBodyString(response.BodyString);
            e.HttpClient.Request.ContentType = response.BodyType.GetString();
            e.HttpClient.Response.KeepBody = response.KeepBody;
            e.HttpClient.Response.HttpVersion = response.HttpMethodVersion;

            return e;
        }

        private static SessionEventArgs ForwardRequestToDifferentHost(MockerRule rule, SessionEventArgs e)
        {
            string originalHost = e.HttpClient.Request.RequestUri.Host;
            string newHost = (string)(rule.MockingActionOptions[rule.MockingAction.GetOptionsKey()]);

            e.HttpClient.Request.RequestUriString = e.HttpClient.Request.RequestUriString.ReplaceFirst(originalHost, newHost);
            e.HttpClient.Request.Host = newHost;

            return e;
        }

        public static SessionEventArgs AutoTransformRequestOrResponse(MockerRule rule, SessionEventArgs e, bool isRequest)
        {
            MockTransformer transformer = (MockTransformer)(rule.MockingActionOptions[rule.MockingAction.GetOptionsKey()]);

            if (isRequest)
            {
                // Setting http method if any.
                if (transformer.RequestMethod != null)
                    e.HttpClient.Request.Method = transformer.RequestMethod.ToString();

                // Setting headers if any.
                if (transformer.RequestHeaders != null)
                {
                    e.HttpClient.Request.Headers.Clear();
                    if (transformer.RequestHeaders.Count != 0)
                        e.HttpClient.Request.Headers.AddHeaders(transformer.RequestHeaders);
                }

                // Setting body type if any.
                if (transformer.RequestBodyType != null)
                    e.HttpClient.Request.ContentType = transformer.RequestBodyType.GetString();

                // Setting request body if any.
                if (transformer.RequestBodyString != null)
                    e.SetRequestBodyString(transformer.RequestBodyString);

                // Setting request keep body if any.
                if (transformer.RequestKeepBody != null)
                    e.HttpClient.Request.KeepBody = (bool)transformer.RequestKeepBody;

                // Changing host if any given.
                if (transformer.RequestHost != null)
                {
                    string originalHost = e.HttpClient.Request.RequestUri.Host;

                    e.HttpClient.Request.RequestUriString = e.HttpClient.Request.RequestUriString.ReplaceFirst(originalHost, transformer.RequestHost);
                    e.HttpClient.Request.Host = transformer.RequestHost;
                }

                // Chaning url if any given.
                if (transformer.RequestUrl != null)
                {
                    string oldRequestUrl = e.HttpClient.Request.RequestUriString;
                    string oldRequestHost = e.HttpClient.Request.Host;

                    try
                    {
                        Uri newUri = new(transformer.RequestUrl);

                        e.HttpClient.Request.RequestUri = newUri;
                        e.HttpClient.Request.Host = newUri.Host;
                    }
                    catch (Exception)
                    {
                        e.HttpClient.Request.RequestUriString = oldRequestUrl;
                        e.HttpClient.Request.Host = oldRequestHost;
                    }
                }

                // Setting http version if any.
                if (transformer.RequestHttpMethodVersion != null)
                    e.HttpClient.Request.HttpVersion = transformer.RequestHttpMethodVersion;

            }
            else if (!isRequest)
            {
                // Setting response status code if any given.
                if (transformer.ResponseStatusCode != null)
                    e.HttpClient.Response.StatusCode = (int)transformer.ResponseStatusCode;

                // Setting headers if any.
                if (transformer.ResponseHeaders != null)
                {
                    e.HttpClient.Request.Headers.Clear();
                    if (transformer.RequestHeaders.Count != 0)
                        e.HttpClient.Request.Headers.AddHeaders(transformer.RequestHeaders);
                }

                // Setting body type if any.
                if (transformer.RequestBodyType != null)
                    e.HttpClient.Request.ContentType = transformer.RequestBodyType.GetString();

                // Setting request body if any.
                if (transformer.RequestBodyString != null)
                    e.SetRequestBodyString(transformer.RequestBodyString);

                // Setting request keep body if any.
                if (transformer.RequestKeepBody != null)
                    e.HttpClient.Request.KeepBody = (bool)transformer.RequestKeepBody;

                // Changing host if any given.
                if (transformer.RequestHost != null)
                {
                    string originalHost = e.HttpClient.Request.RequestUri.Host;

                    e.HttpClient.Request.RequestUriString = e.HttpClient.Request.RequestUriString.ReplaceFirst(originalHost, transformer.RequestHost);
                    e.HttpClient.Request.Host = transformer.RequestHost;
                }

                // Chaning url if any given.
                if (transformer.RequestUrl != null)
                {
                    string oldRequestUrl = e.HttpClient.Request.RequestUriString;
                    string oldRequestHost = e.HttpClient.Request.Host;

                    try
                    {
                        Uri newUri = new(transformer.RequestUrl);

                        e.HttpClient.Request.RequestUri = newUri;
                        e.HttpClient.Request.Host = newUri.Host;
                    }
                    catch (Exception)
                    {
                        e.HttpClient.Request.RequestUriString = oldRequestUrl;
                        e.HttpClient.Request.Host = oldRequestHost;
                    }
                }

                // Setting http version if any.
                if (transformer.RequestHttpMethodVersion != null)
                    e.HttpClient.Request.HttpVersion = transformer.RequestHttpMethodVersion;
            }

            return e;
        }
    }
}