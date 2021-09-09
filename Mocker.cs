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

        // Checkers based on the matcher.
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
    }
}