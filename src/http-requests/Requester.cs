using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HTTPMan
{
    /// <summary>
    /// Class for doing complex http requests (basically everything that is supported by the built-in http client class and the specific request type can be changed.)
    /// </summary>
    public class Requester
    {
        // Fields
        private HttpClient _client;
        private const string _proxiesUrl = "https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout=5000&country=all&ssl=all&anonymity=all&simplified=true";
        private List<string> _proxies = new();

        // Properties
        private HttpClient Client { get { return _client; } set { _client = value; } }
        private string ProxiesUrl { get { return _proxiesUrl; } }
        private List<string> Proxies { get { return _proxies; } set { _proxies = value; } }

        // Methods
        /// <summary>
        /// Gets a list of proxies from proxyscrape.com
        /// </summary>
        /// <returns>Return a list with the proxy downloaded from proxyscrape.com</returns>
        private async Task<List<string>> GetProxies()
        {
            List<string> proxies = new();
            using (HttpClient client = new())
            {
                HttpResponseMessage response = await client.GetAsync(ProxiesUrl);
                string proxiesString = await response.Content.ReadAsStringAsync();
                string[] proxiesArray = proxiesString.Split("\n");
                for (int i = 0; i < proxiesArray.Length; i++)
                {
                    if (!proxiesArray[i].Equals(string.Empty))
                        proxies.Add(proxiesArray[i]);
                }
            }

            return proxies;
        }

        /// <summary>
        /// Simple function that basically picks a random item from a List not just a proxy, but because that's the only use here is way is named like this.
        /// </summary>
        /// <param name="proxies">list with proxies to pick from.</param>
        /// <returns>A random selected proxy from the given list.</returns>
        private string PickProxy(List<string> proxies)
        {
            Random random = new(Guid.NewGuid().GetHashCode());
            string proxy = proxies[random.Next(proxies.Count)];
            return proxy;
        }

        /// <summary>
        /// Complex HTTP Get Request function.
        /// </summary>
        /// <param name="url">The url of the request. Mandatory!</param>
        /// <param name="httpVersion">The HTTP version of the request. Most common is 1.1. Available options are 1.0, 1.1 and 2.0. If value passed is not valid it will default to 1.1.</param>
        /// <param name="versionPolicy">The HTTP version policy tells the request how to handle the version. Available options are exact version, exact version or lower and exact version or higher.</param>
        /// <param name="headers">Dictionary containing the header keys with their values that are gonna be added to the request.</param>
        /// <param name="parameters">Dictionary containing the param keys with their values that are gonna be added to the request url.</param>
        /// <param name="content">HTTP Get Request aren't supposed to send a body, but it may be useful in some cases. The string with the content has to be in the format specified in ContentType.</param>
        /// <param name="contentType">The type of the content you are sending, set with the enum HttpContentType, if not set it will default text/plain.</param>
        /// <param name="timeout">Timeout for the request before it stops waiting for a response from the server.</param>
        /// <param name="maxResponseContentBufferSize">If you want to limit the content size of the response you can set it here in BYTES.</param>
        /// <param name="acceptCache">Whether the request should or not be cached. The default is usually not to cache.</param>
        /// <param name="dnt">DNT is a header value that tells the server the message "DO NOT TRACK" which basically means you don't want to be tracked, but the server doesn't have to care about that.</param>
        /// <param name="anonymizeRequest">Anonymize request is a proccess that adds/changes some headers of the request making it send less information about you. Doesn't do much, but is still usefull
        ///                                when doing data mining type of actions. Works best with DNT set to true.</param>
        /// <param name="useProxies">If setting to true the request will be sent through a free proxy with a default, unchangeable timeout of 10 second. If the request fails to complete in that time
        ///                          or another error occurs because of the proxy it will return back and empty response with only one header called error specifying what happened. Usually you have
        ///                          to try and send the request multiple times. Please note proxies don't secure your data, only hide your identity and even at that they can fail. Use them carefully.</param>
        /// <returns>The response as a HttpResponseMessage which contains all the data about the response and the request the was used to send it. If an error ocurred is gonna send an empty
        ///          response with only one header called error with a value containing information about the error.</returns>
#nullable enable
        public async Task<HttpResponseMessage> GetRequest(string url, double? httpVersion = null, HttpVersionPolicy? versionPolicy = null, Dictionary<string, string>? headers = null,
            Dictionary<string, string>? parameters = null, string? content = null, HttpContentType? contentType = null, TimeSpan? timeout = null, long? maxResponseContentBufferSize = null,
            bool? acceptCache = null, bool? dnt = null, bool anonymizeRequest = false, bool useProxies = false)
        {
            // Setting proxy if user chose to use one.
            if (useProxies)
            {
                if (Proxies.Count() == 0)
                    Proxies = await GetProxies().ConfigureAwait(false);
                string proxy = PickProxy(Proxies);
                HttpClientHandler clientHandler = new()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    Proxy = new WebProxy(new Uri($"http://{proxy}"))
                };

                Client = new(clientHandler); // creates/recreates the client instance, in order to be fresh, using the selected proxy.
                timeout = TimeSpan.FromSeconds(10); // setting the timeout to 10 in order to give slower proxies time to finish the request.
            }
            else
            {
                Client = new(); // creates/recreates the client instance in order to be fresh.
            }

            HttpResponseMessage response;
            using (Client)
            {
                // Setting http version. Defaults to 1.1 if not specified or specified a invalid value.
                if (httpVersion == null)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 1.0)
                    Client.DefaultRequestVersion = HttpVersion.Version10;
                else if (httpVersion == 1.1)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 2.0)
                    Client.DefaultRequestVersion = HttpVersion.Version20;
                else
                    Client.DefaultRequestVersion = HttpVersion.Version11;

                // Setting version policy. Defaults to request's version or higher if not specified.
                if (versionPolicy == null)
                    Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                else
                    Client.DefaultVersionPolicy = (HttpVersionPolicy)versionPolicy;

                // Setting headers if any.
                if (headers != null)
                {
                    Client.DefaultRequestHeaders.Clear();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        Client.DefaultRequestHeaders.Add(headers.Keys.ElementAt(i), headers.Values.ElementAt(i));
                    }
                }

                // Setting params if any.
                if (parameters != null)
                {
                    url += "?"; // symbol at the end which specifies there would be upcoming params.

                    // Manually setting the first param in order to not get a "&" at the end.
                    url += parameters.Keys.ElementAt(0) + "=" + parameters.Values.ElementAt(0);

                    // Automatically setting the rest of params if any.
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        url += "&" + parameters.Keys.ElementAt(i) + "=" + parameters.Values.ElementAt(i);
                    }
                }

                // Setting the content if any.
                HttpRequestMessage? request = null;
                if (content != null)
                {
                    if (contentType != null)
                    {
                        request = new()
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri(url),
                            Content = new StringContent(content, Encoding.UTF8, contentType.GetString())
                        };
                    }
                    else
                    {
                        request = new()
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri(url),
                            Content = new StringContent(content, Encoding.UTF8, HttpContentType.TextPlain.GetString())
                        };
                    }
                }

                // Setting timeout if any.
                if (timeout != null)
                    Client.Timeout = (TimeSpan)timeout;

                // Setting max response content buffer size if specified.
                if (maxResponseContentBufferSize != null)
                    Client.MaxResponseContentBufferSize = (long)maxResponseContentBufferSize;

                // Setting if request accepts cache or not.
                if (acceptCache != null && Client.DefaultRequestHeaders.CacheControl != null)
                    Client.DefaultRequestHeaders.CacheControl.NoCache = (bool)acceptCache;

                // Setting the chosen dnt header value if chose.
                if (dnt != null)
                {
                    if ((bool)dnt)
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    }
                    else
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 0.ToString());
                    }
                }

                // Anonymize request as much request as much as possible if requested.
                if (anonymizeRequest == true)
                {
                    // Setting some headers that provide more anonymity. Not much but it's something.
                    Random random = new();
                    Client.DefaultRequestHeaders.Remove("x-forwarded-for");
                    Client.DefaultRequestHeaders.Add("x-forwarded-for", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("x-forwarded-host");
                    Client.DefaultRequestHeaders.Add("x-forwarded-host", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("forwarded");
                    Client.DefaultRequestHeaders.Add("forwarded", "by=" + random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("user-agent");
                    Client.DefaultRequestHeaders.Add("user-agent", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("referer");
                    Client.DefaultRequestHeaders.Add("referer", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("accept-language");
                    Client.DefaultRequestHeaders.Add("accept-language", "en-US");
                    Client.DefaultRequestHeaders.Remove("dnt");
                    Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    Client.DefaultRequestHeaders.Remove("cache-control");
                    Client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    Client.DefaultRequestHeaders.Add("pragma", "no-cache");
                }

                // Doing the actual request and saving the response.
                if (request == null)
                {
                    try
                    {
                        response = await Client.GetAsync(url); // if the request does not have a body.
                    }
                    catch (TaskCanceledException)
                    {
                        response = new();
                        if (timeout != null)
                            response.Headers.Add("error", "The request could not be completed in the given timeout of " + timeout.Value.TotalSeconds + " seconds");
                        else
                            response.Headers.Add("error", "The request could not be completed in the given timeout");
                    }
                    catch (Exception e)
                    {
                        response = new();
                        response.Headers.Add("error", e.Message);
                    }
                }
                else
                {
                    try
                    {
                        response = await Client.SendAsync(request); // if the get request has a body will have to send it this way because of the way HttpClient works.
                    }
                    catch (TaskCanceledException)
                    {
                        response = new();
                        if (timeout != null)
                            response.Headers.Add("error", "The request could not be completed in the given timeout of " + timeout.Value.TotalSeconds + " seconds");
                        else
                            response.Headers.Add("error", "The request could not be completed in the given timeout");
                    }
                    catch (Exception e)
                    {
                        response = new();
                        response.Headers.Add("error", e.Message);
                    }
                }
            }

            return response;
        }
#nullable disable

        /// <summary>
        /// Complex HTTP Post Request function.
        /// </summary>
        /// <param name="url">The url of the request. Mandatory!</param>
        /// <param name="httpVersion">The HTTP version of the request. Most common is 1.1. Available options are 1.0, 1.1 and 2.0. If value passed is not valid it will default to 1.1.</param>
        /// <param name="versionPolicy">The HTTP version policy tells the request how to handle the version. Available options are exact version, exact version or lower and exact version or higher.</param>
        /// <param name="headers">Dictionary containing the header keys with their values that are gonna be added to the request.</param>
        /// <param name="parameters">Dictionary containing the param keys with their values that are gonna be added to the request url.</param>
        /// <param name="content">The HTTP Post Request content to be sent. The string with the content has to be in the format specified in ContentType.</param>
        /// <param name="contentType">The type of the content you are sending, set with the enum HttpContentType, if not set it will default text/plain.</param>
        /// <param name="timeout">Timeout for the request before it stops waiting for a response from the server.</param>
        /// <param name="maxResponseContentBufferSize">If you want to limit the content size of the response you can set it here in BYTES.</param>
        /// <param name="acceptCache">Whether the request should or not be cached. The default is usually not to cache. HTTP Post Requests are only cacheable if freshness information is included.</param>
        /// <param name="dnt">DNT is a header value that tells the server the message "DO NOT TRACK" which basically means you don't want to be tracked, but the server doesn't have to care about that.</param>
        /// <param name="anonymizeRequest">Anonymize request is a proccess that adds/changes some headers of the request making it send less information about you. Doesn't do much, but is still usefull
        ///                                when doing data mining type of actions. Works best with DNT set to true.</param>
        /// <param name="useProxies">If setting to true the request will be sent through a free proxy with a default, unchangeable timeout of 10 second. If the request fails to complete in that time
        ///                          or another error occurs because of the proxy it will return back and empty response with only one header called error specifying what happened. Usually you have
        ///                          to try and send the request multiple times. Please note proxies don't secure your data, only hide your identity and even at that they can fail. Use them carefully.</param>
        /// <returns>The response as a HttpResponseMessage which contains all the data about the response and the request the was used to send it. If an error ocurred is gonna send an empty
        ///          response with only one header called error with a value containing information about the error.</returns>
#nullable enable
        public async Task<HttpResponseMessage> PostRequest(string url, double? httpVersion = null, HttpVersionPolicy? versionPolicy = null, Dictionary<string, string>? headers = null,
            Dictionary<string, string>? parameters = null, string? content = null, HttpContentType? contentType = null, TimeSpan? timeout = null, long? maxResponseContentBufferSize = null,
            bool? acceptCache = null, bool? dnt = null, bool anonymizeRequest = false, bool useProxies = false)
        {
            // Setting proxy if user chose to use one.
            if (useProxies)
            {
                if (Proxies.Count() == 0)
                    Proxies = await GetProxies().ConfigureAwait(false);
                string proxy = PickProxy(Proxies);
                HttpClientHandler clientHandler = new()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    Proxy = new WebProxy(new Uri($"http://{proxy}"))
                };

                Client = new(clientHandler); // creates/recreates the client instance, in order to be fresh, using the selected proxy.
                timeout = TimeSpan.FromSeconds(10); // setting the timeout to 10 in order to give slower proxies time to finish the request.
            }
            else
            {
                Client = new(); // creates/recreates the client instance in order to be fresh.
            }

            // Null variable which will change withing the code if the user passed a content.
            // At the end when the request is made the function checks if the requestBody is not null and if it's not it will include it in the request.
            // Otherwise it will make a post request without a body.
            StringContent? requestBody = null;

            HttpResponseMessage response;
            using (Client)
            {
                // Setting http version. Defaults to 1.1 if not specified or specified a invalid value.
                if (httpVersion == null)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 1.0)
                    Client.DefaultRequestVersion = HttpVersion.Version10;
                else if (httpVersion == 1.1)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 2.0)
                    Client.DefaultRequestVersion = HttpVersion.Version20;
                else
                    Client.DefaultRequestVersion = HttpVersion.Version11;

                // Setting version policy. Defaults to request's version or higher if not specified.
                if (versionPolicy == null)
                    Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                else
                    Client.DefaultVersionPolicy = (HttpVersionPolicy)versionPolicy;

                // Setting headers if any.
                if (headers != null)
                {
                    Client.DefaultRequestHeaders.Clear();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        Client.DefaultRequestHeaders.Add(headers.Keys.ElementAt(i), headers.Values.ElementAt(i));
                    }
                }

                // Setting params if any.
                if (parameters != null)
                {
                    url += "?"; // symbol at the end which specifies there would be upcoming params.

                    // Manually setting the first param in order to not get a "&" at the end.
                    url += parameters.Keys.ElementAt(0) + "=" + parameters.Values.ElementAt(0);

                    // Automatically setting the rest of params if any.
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        url += "&" + parameters.Keys.ElementAt(i) + "=" + parameters.Values.ElementAt(i);
                    }
                }

                // Converting the content if any.
                if (content != null)
                {
                    if (contentType != null)
                        requestBody = new(content, Encoding.UTF8, contentType.GetString());
                    else
                        requestBody = new(content, Encoding.UTF8, HttpContentType.TextPlain.GetString());
                }

                // Setting timeout if any.
                if (timeout != null)
                    Client.Timeout = (TimeSpan)timeout;

                // Setting max response content buffer size if specified.
                if (maxResponseContentBufferSize != null)
                    Client.MaxResponseContentBufferSize = (long)maxResponseContentBufferSize;

                // Setting if request accepts cache or not.
                if (acceptCache != null && Client.DefaultRequestHeaders.CacheControl != null)
                    Client.DefaultRequestHeaders.CacheControl.NoCache = (bool)acceptCache;

                // Setting the chosen dnt header value if chose.
                if (dnt != null)
                {
                    if ((bool)dnt)
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    }
                    else
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 0.ToString());
                    }
                }

                // Anonymize request as much request as much as possible if requested.
                if (anonymizeRequest == true)
                {
                    // Setting some headers that provide more anonymity. Not much but it's something.
                    Random random = new();
                    Client.DefaultRequestHeaders.Remove("x-forwarded-for");
                    Client.DefaultRequestHeaders.Add("x-forwarded-for", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("x-forwarded-host");
                    Client.DefaultRequestHeaders.Add("x-forwarded-host", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("forwarded");
                    Client.DefaultRequestHeaders.Add("forwarded", "by=" + random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("user-agent");
                    Client.DefaultRequestHeaders.Add("user-agent", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("referer");
                    Client.DefaultRequestHeaders.Add("referer", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("accept-language");
                    Client.DefaultRequestHeaders.Add("accept-language", "en-US");
                    Client.DefaultRequestHeaders.Remove("dnt");
                    Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    Client.DefaultRequestHeaders.Remove("cache-control");
                    Client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    Client.DefaultRequestHeaders.Add("pragma", "no-cache");
                }

                // Doing the actual request and saving the response.
                try
                {
                    if (requestBody != null)
                        response = await Client.PostAsync(url, requestBody);
                    else
                        response = await Client.PostAsync(url, null!);
                }
                catch (TaskCanceledException)
                {
                    response = new();
                    if (timeout != null)
                        response.Headers.Add("error", "The request could not be completed in the given timeout of " + timeout.Value.TotalSeconds + " seconds");
                    else
                        response.Headers.Add("error", "The request could not be completed in the given timeout");
                }
                catch (Exception e)
                {
                    response = new();
                    response.Headers.Add("error", e.Message);
                }
            }

            return response;
        }
#nullable disable

        /// <summary>
        /// Complex HTTP Put Request function.
        /// </summary>
        /// <param name="url">The url of the request. Mandatory!</param>
        /// <param name="httpVersion">The HTTP version of the request. Most common is 1.1. Available options are 1.0, 1.1 and 2.0. If value passed is not valid it will default to 1.1.</param>
        /// <param name="versionPolicy">The HTTP version policy tells the request how to handle the version. Available options are exact version, exact version or lower and exact version or higher.</param>
        /// <param name="headers">Dictionary containing the header keys with their values that are gonna be added to the request.</param>
        /// <param name="parameters">Dictionary containing the param keys with their values that are gonna be added to the request url.</param>
        /// <param name="content">The HTTP Put Request content to be sent. The string with the content has to be in the format specified in ContentType.</param>
        /// <param name="contentType">The type of the content you are sending, set with the enum HttpContentType, if not set it will default text/plain.</param>
        /// <param name="timeout">Timeout for the request before it stops waiting for a response from the server.</param>
        /// <param name="dnt">DNT is a header value that tells the server the message "DO NOT TRACK" which basically means you don't want to be tracked, but the server doesn't have to care about that.</param>
        /// <param name="anonymizeRequest">Anonymize request is a proccess that adds/changes some headers of the request making it send less information about you. Doesn't do much, but is still usefull
        ///                                when doing data mining type of actions. Works best with DNT set to true.</param>
        /// <param name="useProxies">If setting to true the request will be sent through a free proxy with a default, unchangeable timeout of 10 second. If the request fails to complete in that time
        ///                          or another error occurs because of the proxy it will return back and empty response with only one header called error specifying what happened. Usually you have
        ///                          to try and send the request multiple times. Please note proxies don't secure your data, only hide your identity and even at that they can fail. Use them carefully.</param>
        /// <returns>The response as a HttpResponseMessage which contains all the data about the response and the request the was used to send it. If an error ocurred is gonna send an empty
        ///          response with only one header called error with a value containing information about the error. Note Put Request's responses don't have a body.</returns>
#nullable enable
        public async Task<HttpResponseMessage> PutRequest(string url, double? httpVersion = null, HttpVersionPolicy? versionPolicy = null, Dictionary<string, string>? headers = null,
            Dictionary<string, string>? parameters = null, string? content = null, HttpContentType? contentType = null, TimeSpan? timeout = null, bool? dnt = null, bool anonymizeRequest = false,
            bool useProxies = false)
        {
            // Setting proxy if user chose to use one.
            if (useProxies)
            {
                if (Proxies.Count() == 0)
                    Proxies = await GetProxies().ConfigureAwait(false);
                string proxy = PickProxy(Proxies);
                HttpClientHandler clientHandler = new()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    Proxy = new WebProxy(new Uri($"http://{proxy}"))
                };

                Client = new(clientHandler); // creates/recreates the client instance, in order to be fresh, using the selected proxy.
                timeout = TimeSpan.FromSeconds(10); // setting the timeout to 10 in order to give slower proxies time to finish the request.
            }
            else
            {
                Client = new(); // creates/recreates the client instance in order to be fresh.
            }

            // Null variable which will change withing the code if the user passed a content.
            // At the end when the request is made the function checks if the requestBody is not null and if it's not it will include it in the request.
            // Otherwise it will make a post request without a body.
            StringContent? requestBody = null;

            HttpResponseMessage response;
            using (Client)
            {
                // Setting http version. Defaults to 1.1 if not specified or specified a invalid value.
                if (httpVersion == null)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 1.0)
                    Client.DefaultRequestVersion = HttpVersion.Version10;
                else if (httpVersion == 1.1)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 2.0)
                    Client.DefaultRequestVersion = HttpVersion.Version20;
                else
                    Client.DefaultRequestVersion = HttpVersion.Version11;

                // Setting version policy. Defaults to request's version or higher if not specified.
                if (versionPolicy == null)
                    Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                else
                    Client.DefaultVersionPolicy = (HttpVersionPolicy)versionPolicy;

                // Setting headers if any.
                if (headers != null)
                {
                    Client.DefaultRequestHeaders.Clear();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        Client.DefaultRequestHeaders.Add(headers.Keys.ElementAt(i), headers.Values.ElementAt(i));
                    }
                }

                // Setting params if any.
                if (parameters != null)
                {
                    url += "?"; // symbol at the end which specifies there would be upcoming params.

                    // Manually setting the first param in order to not get a "&" at the end.
                    url += parameters.Keys.ElementAt(0) + "=" + parameters.Values.ElementAt(0);

                    // Automatically setting the rest of params if any.
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        url += "&" + parameters.Keys.ElementAt(i) + "=" + parameters.Values.ElementAt(i);
                    }
                }

                // Converting the content if any.
                if (content != null)
                {
                    if (contentType != null)
                        requestBody = new(content, Encoding.UTF8, contentType.GetString());
                    else
                        requestBody = new(content, Encoding.UTF8, HttpContentType.TextPlain.GetString());
                }

                // Setting timeout if any.
                if (timeout != null)
                    Client.Timeout = (TimeSpan)timeout;

                // Setting the chosen dnt header value if chose.
                if (dnt != null)
                {
                    if ((bool)dnt)
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    }
                    else
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 0.ToString());
                    }
                }

                // Anonymize request as much request as much as possible if requested.
                if (anonymizeRequest == true)
                {
                    // Setting some headers that provide more anonymity. Not much but it's something.
                    Random random = new();
                    Client.DefaultRequestHeaders.Remove("x-forwarded-for");
                    Client.DefaultRequestHeaders.Add("x-forwarded-for", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("x-forwarded-host");
                    Client.DefaultRequestHeaders.Add("x-forwarded-host", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("forwarded");
                    Client.DefaultRequestHeaders.Add("forwarded", "by=" + random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("user-agent");
                    Client.DefaultRequestHeaders.Add("user-agent", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("referer");
                    Client.DefaultRequestHeaders.Add("referer", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("accept-language");
                    Client.DefaultRequestHeaders.Add("accept-language", "en-US");
                    Client.DefaultRequestHeaders.Remove("dnt");
                    Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    Client.DefaultRequestHeaders.Remove("cache-control");
                    Client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    Client.DefaultRequestHeaders.Add("pragma", "no-cache");
                }

                // Doing the actual request and saving the response.
                try
                {
                    if (requestBody != null)
                        response = await Client.PutAsync(url, requestBody);
                    else
                        response = await Client.PutAsync(url, null!);
                }
                catch (TaskCanceledException)
                {
                    response = new();
                    if (timeout != null)
                        response.Headers.Add("error", "The request could not be completed in the given timeout of " + timeout.Value.TotalSeconds + " seconds");
                    else
                        response.Headers.Add("error", "The request could not be completed in the given timeout");
                }
                catch (Exception e)
                {
                    response = new();
                    response.Headers.Add("error", e.Message);
                }
            }

            return response;
        }
#nullable disable

        /// <summary>
        /// Complex HTTP Patch Request function. HTTP Patch request applies partial modifications to a resource.
        /// </summary>
        /// <param name="url">The url of the request. Mandatory!</param>
        /// <param name="httpVersion">The HTTP version of the request. Most common is 1.1. Available options are 1.0, 1.1 and 2.0. If value passed is not valid it will default to 1.1.</param>
        /// <param name="versionPolicy">The HTTP version policy tells the request how to handle the version. Available options are exact version, exact version or lower and exact version or higher.</param>
        /// <param name="headers">Dictionary containing the header keys with their values that are gonna be added to the request.</param>
        /// <param name="parameters">Dictionary containing the param keys with their values that are gonna be added to the request url.</param>
        /// <param name="content">The HTTP Patch Request content to be sent. The string with the content has to be in the format specified in ContentType.</param>
        /// <param name="contentType">The type of the content you are sending, set with the enum HttpContentType, if not set it will default text/plain.</param>
        /// <param name="timeout">Timeout for the request before it stops waiting for a response from the server.</param>
        /// <param name="maxResponseContentBufferSize">If you want to limit the content size of the response you can set it here in BYTES.</param>
        /// <param name="dnt">DNT is a header value that tells the server the message "DO NOT TRACK" which basically means you don't want to be tracked, but the server doesn't have to care about that.</param>
        /// <param name="anonymizeRequest">Anonymize request is a proccess that adds/changes some headers of the request making it send less information about you. Doesn't do much, but is still usefull
        ///                                when doing data mining type of actions. Works best with DNT set to true.</param>
        /// <param name="useProxies">If setting to true the request will be sent through a free proxy with a default, unchangeable timeout of 10 second. If the request fails to complete in that time
        ///                          or another error occurs because of the proxy it will return back and empty response with only one header called error specifying what happened. Usually you have
        ///                          to try and send the request multiple times. Please note proxies don't secure your data, only hide your identity and even at that they can fail. Use them carefully.</param>
        /// <returns>The response as a HttpResponseMessage which contains all the data about the response and the request the was used to send it. If an error ocurred is gonna send an empty
        ///          response with only one header called error with a value containing information about the error. Note Put Request's responses don't have a body.</returns>
#nullable enable
        public async Task<HttpResponseMessage> PatchRequest(string url, double? httpVersion = null, HttpVersionPolicy? versionPolicy = null, Dictionary<string, string>? headers = null,
            Dictionary<string, string>? parameters = null, string? content = null, HttpContentType? contentType = null, TimeSpan? timeout = null, long? maxResponseContentBufferSize = null,
            bool? dnt = null, bool anonymizeRequest = false, bool useProxies = false)
        {
            // Setting proxy if user chose to use one.
            if (useProxies)
            {
                if (Proxies.Count() == 0)
                    Proxies = await GetProxies().ConfigureAwait(false);
                string proxy = PickProxy(Proxies);
                HttpClientHandler clientHandler = new()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    Proxy = new WebProxy(new Uri($"http://{proxy}"))
                };

                Client = new(clientHandler); // creates/recreates the client instance, in order to be fresh, using the selected proxy.
                timeout = TimeSpan.FromSeconds(10); // setting the timeout to 10 in order to give slower proxies time to finish the request.
            }
            else
            {
                Client = new(); // creates/recreates the client instance in order to be fresh.
            }

            // Null variable which will change withing the code if the user passed a content.
            // At the end when the request is made the function checks if the requestBody is not null and if it's not it will include it in the request.
            // Otherwise it will make a post request without a body.
            StringContent? requestBody = null;

            HttpResponseMessage response;
            using (Client)
            {
                // Setting http version. Defaults to 1.1 if not specified or specified a invalid value.
                if (httpVersion == null)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 1.0)
                    Client.DefaultRequestVersion = HttpVersion.Version10;
                else if (httpVersion == 1.1)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 2.0)
                    Client.DefaultRequestVersion = HttpVersion.Version20;
                else
                    Client.DefaultRequestVersion = HttpVersion.Version11;

                // Setting version policy. Defaults to request's version or higher if not specified.
                if (versionPolicy == null)
                    Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                else
                    Client.DefaultVersionPolicy = (HttpVersionPolicy)versionPolicy;

                // Setting headers if any.
                if (headers != null)
                {
                    Client.DefaultRequestHeaders.Clear();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        Client.DefaultRequestHeaders.Add(headers.Keys.ElementAt(i), headers.Values.ElementAt(i));
                    }
                }

                // Setting params if any.
                if (parameters != null)
                {
                    url += "?"; // symbol at the end which specifies there would be upcoming params.

                    // Manually setting the first param in order to not get a "&" at the end.
                    url += parameters.Keys.ElementAt(0) + "=" + parameters.Values.ElementAt(0);

                    // Automatically setting the rest of params if any.
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        url += "&" + parameters.Keys.ElementAt(i) + "=" + parameters.Values.ElementAt(i);
                    }
                }

                // Converting the content if any.
                if (content != null)
                {
                    if (contentType != null)
                        requestBody = new(content, Encoding.UTF8, contentType.GetString());
                    else
                        requestBody = new(content, Encoding.UTF8, HttpContentType.TextPlain.GetString());
                }

                // Setting timeout if any.
                if (timeout != null)
                    Client.Timeout = (TimeSpan)timeout;

                // Setting max response content buffer size if specified.
                if (maxResponseContentBufferSize != null)
                    Client.MaxResponseContentBufferSize = (long)maxResponseContentBufferSize;

                // Setting the chosen dnt header value if chose.
                if (dnt != null)
                {
                    if ((bool)dnt)
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    }
                    else
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 0.ToString());
                    }
                }

                // Anonymize request as much request as much as possible if requested.
                if (anonymizeRequest == true)
                {
                    // Setting some headers that provide more anonymity. Not much but it's something.
                    Random random = new();
                    Client.DefaultRequestHeaders.Remove("x-forwarded-for");
                    Client.DefaultRequestHeaders.Add("x-forwarded-for", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("x-forwarded-host");
                    Client.DefaultRequestHeaders.Add("x-forwarded-host", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("forwarded");
                    Client.DefaultRequestHeaders.Add("forwarded", "by=" + random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("user-agent");
                    Client.DefaultRequestHeaders.Add("user-agent", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("referer");
                    Client.DefaultRequestHeaders.Add("referer", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("accept-language");
                    Client.DefaultRequestHeaders.Add("accept-language", "en-US");
                    Client.DefaultRequestHeaders.Remove("dnt");
                    Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    Client.DefaultRequestHeaders.Remove("cache-control");
                    Client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    Client.DefaultRequestHeaders.Add("pragma", "no-cache");
                }

                // Doing the actual request and saving the response.
                try
                {
                    if (requestBody != null)
                        response = await Client.PatchAsync(url, requestBody);
                    else
                        response = await Client.PatchAsync(url, null!);
                }
                catch (TaskCanceledException)
                {
                    response = new();
                    if (timeout != null)
                        response.Headers.Add("error", "The request could not be completed in the given timeout of " + timeout.Value.TotalSeconds + " seconds");
                    else
                        response.Headers.Add("error", "The request could not be completed in the given timeout");
                }
                catch (Exception e)
                {
                    response = new();
                    response.Headers.Add("error", e.Message);
                }
            }

            return response;
        }
#nullable disable

        /// <summary>
        /// Complex HTTP Delete Request function.
        /// </summary>
        /// <param name="url">The url of the request. Mandatory!</param>
        /// <param name="httpVersion">The HTTP version of the request. Most common is 1.1. Available options are 1.0, 1.1 and 2.0. If value passed is not valid it will default to 1.1.</param>
        /// <param name="versionPolicy">The HTTP version policy tells the request how to handle the version. Available options are exact version, exact version or lower and exact version or higher.</param>
        /// <param name="headers">Dictionary containing the header keys with their values that are gonna be added to the request.</param>
        /// <param name="parameters">Dictionary containing the param keys with their values that are gonna be added to the request url.</param>
        /// <param name="content">The HTTP Delete Request content to be sent. The string with the content has to be in the format specified in ContentType.</param>
        /// <param name="contentType">The type of the content you are sending, set with the enum HttpContentType, if not set it will default text/plain.</param>
        /// <param name="timeout">Timeout for the request before it stops waiting for a response from the server.</param>
        /// <param name="maxResponseContentBufferSize">If you want to limit the content size of the response you can set it here in BYTES.</param>
        /// <param name="acceptCache">Whether the request should or not be cached. The default is usually not to cache. HTTP Post Requests are only cacheable if freshness information is included.</param>
        /// <param name="dnt">DNT is a header value that tells the server the message "DO NOT TRACK" which basically means you don't want to be tracked, but the server doesn't have to care about that.</param>
        /// <param name="anonymizeRequest">Anonymize request is a proccess that adds/changes some headers of the request making it send less information about you. Doesn't do much, but is still usefull
        ///                                when doing data mining type of actions. Works best with DNT set to true.</param>
        /// <param name="useProxies">If setting to true the request will be sent through a free proxy with a default, unchangeable timeout of 10 second. If the request fails to complete in that time
        ///                          or another error occurs because of the proxy it will return back and empty response with only one header called error specifying what happened. Usually you have
        ///                          to try and send the request multiple times. Please note proxies don't secure your data, only hide your identity and even at that they can fail. Use them carefully.</param>
        /// <returns>The response as a HttpResponseMessage which contains all the data about the response and the request the was used to send it. If an error ocurred is gonna send an empty
        ///          response with only one header called error with a value containing information about the error.</returns>
#nullable enable
        public async Task<HttpResponseMessage> DeleteRequest(string url, double? httpVersion = null, HttpVersionPolicy? versionPolicy = null, Dictionary<string, string>? headers = null,
            Dictionary<string, string>? parameters = null, string? content = null, HttpContentType? contentType = null, TimeSpan? timeout = null, long? maxResponseContentBufferSize = null,
            bool? dnt = null, bool anonymizeRequest = false, bool useProxies = false)
        {
            // Setting proxy if user chose to use one.
            if (useProxies)
            {
                if (Proxies.Count() == 0)
                    Proxies = await GetProxies().ConfigureAwait(false);
                string proxy = PickProxy(Proxies);
                HttpClientHandler clientHandler = new()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    Proxy = new WebProxy(new Uri($"http://{proxy}"))
                };

                Client = new(clientHandler); // creates/recreates the client instance, in order to be fresh, using the selected proxy.
                timeout = TimeSpan.FromSeconds(10); // setting the timeout to 10 in order to give slower proxies time to finish the request.
            }
            else
            {
                Client = new(); // creates/recreates the client instance in order to be fresh.
            }

            // Null variable which will change withing the code if the user passed a content.
            // At the end when the request is made the function checks if the requestBody is not null and if it's not it will include it in the request.
            // Otherwise it will make a post request without a body.

            HttpResponseMessage response;
            using (Client)
            {
                // Setting http version. Defaults to 1.1 if not specified or specified a invalid value.
                if (httpVersion == null)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 1.0)
                    Client.DefaultRequestVersion = HttpVersion.Version10;
                else if (httpVersion == 1.1)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 2.0)
                    Client.DefaultRequestVersion = HttpVersion.Version20;
                else
                    Client.DefaultRequestVersion = HttpVersion.Version11;

                // Setting version policy. Defaults to request's version or higher if not specified.
                if (versionPolicy == null)
                    Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                else
                    Client.DefaultVersionPolicy = (HttpVersionPolicy)versionPolicy;

                // Setting headers if any.
                if (headers != null)
                {
                    Client.DefaultRequestHeaders.Clear();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        Client.DefaultRequestHeaders.Add(headers.Keys.ElementAt(i), headers.Values.ElementAt(i));
                    }
                }

                // Setting params if any.
                if (parameters != null)
                {
                    url += "?"; // symbol at the end which specifies there would be upcoming params.

                    // Manually setting the first param in order to not get a "&" at the end.
                    url += parameters.Keys.ElementAt(0) + "=" + parameters.Values.ElementAt(0);

                    // Automatically setting the rest of params if any.
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        url += "&" + parameters.Keys.ElementAt(i) + "=" + parameters.Values.ElementAt(i);
                    }
                }

                // Setting the content if any.
                HttpRequestMessage? request = null;
                if (content != null)
                {
                    if (contentType != null)
                    {
                        request = new()
                        {
                            Method = HttpMethod.Delete,
                            RequestUri = new Uri(url),
                            Content = new StringContent(content, Encoding.UTF8, contentType.GetString())
                        };
                    }
                    else
                    {
                        request = new()
                        {
                            Method = HttpMethod.Delete,
                            RequestUri = new Uri(url),
                            Content = new StringContent(content, Encoding.UTF8, HttpContentType.TextPlain.GetString())
                        };
                    }
                }

                // Setting timeout if any.
                if (timeout != null)
                    Client.Timeout = (TimeSpan)timeout;

                // Setting max response content buffer size if specified.
                if (maxResponseContentBufferSize != null)
                    Client.MaxResponseContentBufferSize = (long)maxResponseContentBufferSize;

                // Setting the chosen dnt header value if chose.
                if (dnt != null)
                {
                    if ((bool)dnt)
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    }
                    else
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 0.ToString());
                    }
                }

                // Anonymize request as much request as much as possible if requested.
                if (anonymizeRequest == true)
                {
                    // Setting some headers that provide more anonymity. Not much but it's something.
                    Random random = new();
                    Client.DefaultRequestHeaders.Remove("x-forwarded-for");
                    Client.DefaultRequestHeaders.Add("x-forwarded-for", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("x-forwarded-host");
                    Client.DefaultRequestHeaders.Add("x-forwarded-host", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("forwarded");
                    Client.DefaultRequestHeaders.Add("forwarded", "by=" + random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("user-agent");
                    Client.DefaultRequestHeaders.Add("user-agent", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("referer");
                    Client.DefaultRequestHeaders.Add("referer", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("accept-language");
                    Client.DefaultRequestHeaders.Add("accept-language", "en-US");
                    Client.DefaultRequestHeaders.Remove("dnt");
                    Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    Client.DefaultRequestHeaders.Remove("cache-control");
                    Client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    Client.DefaultRequestHeaders.Add("pragma", "no-cache");
                }

                // Doing the actual request and saving the response.
                if (request == null)
                {
                    try
                    {
                        response = await Client.DeleteAsync(url); // if the request does not have a body
                    }
                    catch (TaskCanceledException)
                    {
                        response = new();
                        if (timeout != null)
                            response.Headers.Add("error", "The request could not be completed in the given timeout of " + timeout.Value.TotalSeconds + " seconds");
                        else
                            response.Headers.Add("error", "The request could not be completed in the given timeout");
                    }
                    catch (Exception e)
                    {
                        response = new();
                        response.Headers.Add("error", e.Message);
                    }
                }
                else
                {
                    try
                    {
                        response = await Client.SendAsync(request); // if the delete request has a body will have to send it this way because of the way HttpClient works.
                    }
                    catch (TaskCanceledException)
                    {
                        response = new();
                        if (timeout != null)
                            response.Headers.Add("error", "The request could not be completed in the given timeout of " + timeout.Value.TotalSeconds + " seconds");
                        else
                            response.Headers.Add("error", "The request could not be completed in the given timeout");
                    }
                    catch (Exception e)
                    {
                        response = new();
                        response.Headers.Add("error", e.Message);
                    }
                }
            }

            return response;
        }
#nullable disable

        /// <summary>
        /// Complex HTTP Head Request function. A Head Request gets the headers of a request's response without downloading any other data.
        /// </summary>
        /// <param name="url">The url of the request. Mandatory!</param>
        /// <param name="httpVersion">The HTTP version of the request. Most common is 1.1. Available options are 1.0, 1.1 and 2.0. If value passed is not valid it will default to 1.1.</param>
        /// <param name="versionPolicy">The HTTP version policy tells the request how to handle the version. Available options are exact version, exact version or lower and exact version or higher.</param>
        /// <param name="headers">Dictionary containing the header keys with their values that are gonna be added to the request.</param>
        /// <param name="parameters">Dictionary containing the param keys with their values that are gonna be added to the request url.</param>
        /// <param name="timeout">Timeout for the request before it stops waiting for a response from the server.</param>
        /// <param name="acceptCache">Whether the request should or not be cached. The default is usually not to cache.</param>
        /// <param name="dnt">DNT is a header value that tells the server the message "DO NOT TRACK" which basically means you don't want to be tracked, but the server doesn't have to care about that.</param>
        /// <param name="anonymizeRequest">Anonymize request is a proccess that adds/changes some headers of the request making it send less information about you. Doesn't do much, but is still usefull
        ///                                when doing data mining type of actions. Works best with DNT set to true.</param>
        /// <param name="useProxies">If setting to true the request will be sent through a free proxy with a default, unchangeable timeout of 10 second. If the request fails to complete in that time
        ///                          or another error occurs because of the proxy it will return back and empty response with only one header called error specifying what happened. Usually you have
        ///                          to try and send the request multiple times. Please note proxies don't secure your data, only hide your identity and even at that they can fail. Use them carefully.</param>
        /// <returns>The response as a HttpResponseMessage which contains all the data about the response and the request the was used to send it. If an error ocurred is gonna send an empty
        ///          response with only one header called error with a value containing information about the error.</returns>
#nullable enable
        public async Task<HttpResponseMessage> HeadRequest(string url, double? httpVersion = null, HttpVersionPolicy? versionPolicy = null, Dictionary<string, string>? headers = null,
            Dictionary<string, string>? parameters = null, TimeSpan? timeout = null, bool? acceptCache = null, bool? dnt = null, bool anonymizeRequest = false, bool useProxies = false)
        {
            // Setting proxy if user chose to use one.
            if (useProxies)
            {
                if (Proxies.Count() == 0)
                    Proxies = await GetProxies().ConfigureAwait(false);
                string proxy = PickProxy(Proxies);
                HttpClientHandler clientHandler = new()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    Proxy = new WebProxy(new Uri($"http://{proxy}"))
                };

                Client = new(clientHandler); // creates/recreates the client instance, in order to be fresh, using the selected proxy.
                timeout = TimeSpan.FromSeconds(10); // setting the timeout to 10 in order to give slower proxies time to finish the request.
            }
            else
            {
                Client = new(); // creates/recreates the client instance in order to be fresh.
            }

            HttpResponseMessage response;
            using (Client)
            {
                // Setting http version. Defaults to 1.1 if not specified or specified a invalid value.
                if (httpVersion == null)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 1.0)
                    Client.DefaultRequestVersion = HttpVersion.Version10;
                else if (httpVersion == 1.1)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 2.0)
                    Client.DefaultRequestVersion = HttpVersion.Version20;
                else
                    Client.DefaultRequestVersion = HttpVersion.Version11;

                // Setting version policy. Defaults to request's version or higher if not specified.
                if (versionPolicy == null)
                    Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                else
                    Client.DefaultVersionPolicy = (HttpVersionPolicy)versionPolicy;

                // Setting headers if any.
                if (headers != null)
                {
                    Client.DefaultRequestHeaders.Clear();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        Client.DefaultRequestHeaders.Add(headers.Keys.ElementAt(i), headers.Values.ElementAt(i));
                    }
                }

                // Setting params if any.
                if (parameters != null)
                {
                    url += "?"; // symbol at the end which specifies there would be upcoming params.

                    // Manually setting the first param in order to not get a "&" at the end.
                    url += parameters.Keys.ElementAt(0) + "=" + parameters.Values.ElementAt(0);

                    // Automatically setting the rest of params if any.
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        url += "&" + parameters.Keys.ElementAt(i) + "=" + parameters.Values.ElementAt(i);
                    }
                }

                // Setting timeout if any.
                if (timeout != null)
                    Client.Timeout = (TimeSpan)timeout;

                // Setting if request accepts cache or not.
                if (acceptCache != null && Client.DefaultRequestHeaders.CacheControl != null)
                    Client.DefaultRequestHeaders.CacheControl.NoCache = (bool)acceptCache;

                // Setting the chosen dnt header value if chose.
                if (dnt != null)
                {
                    if ((bool)dnt)
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    }
                    else
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 0.ToString());
                    }
                }

                // Anonymize request as much request as much as possible if requested.
                if (anonymizeRequest == true)
                {
                    // Setting some headers that provide more anonymity. Not much but it's something.
                    Random random = new();
                    Client.DefaultRequestHeaders.Remove("x-forwarded-for");
                    Client.DefaultRequestHeaders.Add("x-forwarded-for", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("x-forwarded-host");
                    Client.DefaultRequestHeaders.Add("x-forwarded-host", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("forwarded");
                    Client.DefaultRequestHeaders.Add("forwarded", "by=" + random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("user-agent");
                    Client.DefaultRequestHeaders.Add("user-agent", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("referer");
                    Client.DefaultRequestHeaders.Add("referer", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("accept-language");
                    Client.DefaultRequestHeaders.Add("accept-language", "en-US");
                    Client.DefaultRequestHeaders.Remove("dnt");
                    Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    Client.DefaultRequestHeaders.Remove("cache-control");
                    Client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    Client.DefaultRequestHeaders.Add("pragma", "no-cache");
                }

                // Setting up the request
                HttpRequestMessage? request = new()
                {
                    Method = HttpMethod.Head,
                    RequestUri = new Uri(url)
                };

                // Doing the actual request and saving the response.
                try
                {
                    response = await Client.SendAsync(request);
                }
                catch (TaskCanceledException)
                {
                    response = new();
                    if (timeout != null)
                        response.Headers.Add("error", "The request could not be completed in the given timeout of " + timeout.Value.TotalSeconds + " seconds");
                    else
                        response.Headers.Add("error", "The request could not be completed in the given timeout");
                }
                catch (Exception e)
                {
                    response = new();
                    response.Headers.Add("error", e.Message);
                }
            }

            return response;
        }
#nullable disable

        /// <summary>
        /// Complex HTTP Trace Request function. A Trace Request performs a message loop-back test along the path to the target resource, providing a useful debugging mechanism.
        /// </summary>
        /// <param name="url">The url of the request. Mandatory!</param>
        /// <param name="httpVersion">The HTTP version of the request. Most common is 1.1. Available options are 1.0, 1.1 and 2.0. If value passed is not valid it will default to 1.1.</param>
        /// <param name="versionPolicy">The HTTP version policy tells the request how to handle the version. Available options are exact version, exact version or lower and exact version or higher.</param>
        /// <param name="headers">Dictionary containing the header keys with their values that are gonna be added to the request.</param>
        /// <param name="parameters">Dictionary containing the param keys with their values that are gonna be added to the request url.</param>
        /// <param name="timeout">Timeout for the request before it stops waiting for a response from the server.</param>
        /// <param name="dnt">DNT is a header value that tells the server the message "DO NOT TRACK" which basically means you don't want to be tracked, but the server doesn't have to care about that.</param>
        /// <param name="anonymizeRequest">Anonymize request is a proccess that adds/changes some headers of the request making it send less information about you. Doesn't do much, but is still usefull
        ///                                when doing data mining type of actions. Works best with DNT set to true.</param>
        /// <param name="useProxies">If setting to true the request will be sent through a free proxy with a default, unchangeable timeout of 10 second. If the request fails to complete in that time
        ///                          or another error occurs because of the proxy it will return back and empty response with only one header called error specifying what happened. Usually you have
        ///                          to try and send the request multiple times. Please note proxies don't secure your data, only hide your identity and even at that they can fail. Use them carefully.</param>
        /// <returns>The response as a HttpResponseMessage which contains all the data about the response and the request the was used to send it. If an error ocurred is gonna send an empty
        ///          response with only one header called error with a value containing information about the error.</returns>
#nullable enable
        public async Task<HttpResponseMessage> TraceRequest(string url, double? httpVersion = null, HttpVersionPolicy? versionPolicy = null, Dictionary<string, string>? headers = null,
            Dictionary<string, string>? parameters = null, TimeSpan? timeout = null, bool? dnt = null, bool anonymizeRequest = false, bool useProxies = false)
        {
            // Setting proxy if user chose to use one.
            if (useProxies)
            {
                if (Proxies.Count() == 0)
                    Proxies = await GetProxies().ConfigureAwait(false);
                string proxy = PickProxy(Proxies);
                HttpClientHandler clientHandler = new()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    Proxy = new WebProxy(new Uri($"http://{proxy}"))
                };

                Client = new(clientHandler); // creates/recreates the client instance, in order to be fresh, using the selected proxy.
                timeout = TimeSpan.FromSeconds(10); // setting the timeout to 10 in order to give slower proxies time to finish the request.
            }
            else
            {
                Client = new(); // creates/recreates the client instance in order to be fresh.
            }

            HttpResponseMessage response;
            using (Client)
            {
                // Setting http version. Defaults to 1.1 if not specified or specified a invalid value.
                if (httpVersion == null)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 1.0)
                    Client.DefaultRequestVersion = HttpVersion.Version10;
                else if (httpVersion == 1.1)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 2.0)
                    Client.DefaultRequestVersion = HttpVersion.Version20;
                else
                    Client.DefaultRequestVersion = HttpVersion.Version11;

                // Setting version policy. Defaults to request's version or higher if not specified.
                if (versionPolicy == null)
                    Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                else
                    Client.DefaultVersionPolicy = (HttpVersionPolicy)versionPolicy;

                // Setting headers if any.
                if (headers != null)
                {
                    Client.DefaultRequestHeaders.Clear();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        Client.DefaultRequestHeaders.Add(headers.Keys.ElementAt(i), headers.Values.ElementAt(i));
                    }
                }

                // Setting params if any.
                if (parameters != null)
                {
                    url += "?"; // symbol at the end which specifies there would be upcoming params.

                    // Manually setting the first param in order to not get a "&" at the end.
                    url += parameters.Keys.ElementAt(0) + "=" + parameters.Values.ElementAt(0);

                    // Automatically setting the rest of params if any.
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        url += "&" + parameters.Keys.ElementAt(i) + "=" + parameters.Values.ElementAt(i);
                    }
                }

                // Setting timeout if any.
                if (timeout != null)
                    Client.Timeout = (TimeSpan)timeout;

                // Setting the chosen dnt header value if chose.
                if (dnt != null)
                {
                    if ((bool)dnt)
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    }
                    else
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 0.ToString());
                    }
                }

                // Anonymize request as much request as much as possible if requested.
                if (anonymizeRequest == true)
                {
                    // Setting some headers that provide more anonymity. Not much but it's something.
                    Random random = new();
                    Client.DefaultRequestHeaders.Remove("x-forwarded-for");
                    Client.DefaultRequestHeaders.Add("x-forwarded-for", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("x-forwarded-host");
                    Client.DefaultRequestHeaders.Add("x-forwarded-host", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("forwarded");
                    Client.DefaultRequestHeaders.Add("forwarded", "by=" + random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("user-agent");
                    Client.DefaultRequestHeaders.Add("user-agent", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("referer");
                    Client.DefaultRequestHeaders.Add("referer", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("accept-language");
                    Client.DefaultRequestHeaders.Add("accept-language", "en-US");
                    Client.DefaultRequestHeaders.Remove("dnt");
                    Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    Client.DefaultRequestHeaders.Remove("cache-control");
                    Client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    Client.DefaultRequestHeaders.Add("pragma", "no-cache");
                }

                // Setting up the request
                HttpRequestMessage? request = new()
                {
                    Method = HttpMethod.Trace,
                    RequestUri = new Uri(url)
                };

                // Doing the actual request and saving the response.
                try
                {
                    response = await Client.SendAsync(request);
                }
                catch (TaskCanceledException)
                {
                    response = new();
                    if (timeout != null)
                        response.Headers.Add("error", "The request could not be completed in the given timeout of " + timeout.Value.TotalSeconds + " seconds");
                    else
                        response.Headers.Add("error", "The request could not be completed in the given timeout");
                }
                catch (Exception e)
                {
                    response = new();
                    response.Headers.Add("error", e.Message);
                }
            }

            return response;
        }
#nullable disable

        /// <summary>
        /// Complex HTTP Options Request function.
        /// </summary>
        /// <param name="url">The url of the request. Mandatory!</param>
        /// <param name="httpVersion">The HTTP version of the request. Most common is 1.1. Available options are 1.0, 1.1 and 2.0. If value passed is not valid it will default to 1.1.</param>
        /// <param name="versionPolicy">The HTTP version policy tells the request how to handle the version. Available options are exact version, exact version or lower and exact version or higher.</param>
        /// <param name="headers">Dictionary containing the header keys with their values that are gonna be added to the request.</param>
        /// <param name="parameters">Dictionary containing the param keys with their values that are gonna be added to the request url.</param>
        /// <param name="timeout">Timeout for the request before it stops waiting for a response from the server.</param>
        /// <param name="maxResponseContentBufferSize">If you want to limit the content size of the response you can set it here in BYTES.</param>
        /// <param name="dnt">DNT is a header value that tells the server the message "DO NOT TRACK" which basically means you don't want to be tracked, but the server doesn't have to care about that.</param>
        /// <param name="anonymizeRequest">Anonymize request is a proccess that adds/changes some headers of the request making it send less information about you. Doesn't do much, but is still usefull
        ///                                when doing data mining type of actions. Works best with DNT set to true.</param>
        /// <param name="useProxies">If setting to true the request will be sent through a free proxy with a default, unchangeable timeout of 10 second. If the request fails to complete in that time
        ///                          or another error occurs because of the proxy it will return back and empty response with only one header called error specifying what happened. Usually you have
        ///                          to try and send the request multiple times. Please note proxies don't secure your data, only hide your identity and even at that they can fail. Use them carefully.</param>
        /// <returns>The response as a HttpResponseMessage which contains all the data about the response and the request the was used to send it. If an error ocurred is gonna send an empty
        ///          response with only one header called error with a value containing information about the error.</returns>
#nullable enable
        public async Task<HttpResponseMessage> OptionsRequest(string url, double? httpVersion = null, HttpVersionPolicy? versionPolicy = null, Dictionary<string, string>? headers = null,
            Dictionary<string, string>? parameters = null, TimeSpan? timeout = null, long? maxResponseContentBufferSize = null, bool? dnt = null, bool anonymizeRequest = false, bool useProxies = false)
        {
            // Setting proxy if user chose to use one.
            if (useProxies)
            {
                if (Proxies.Count() == 0)
                    Proxies = await GetProxies().ConfigureAwait(false);
                string proxy = PickProxy(Proxies);
                HttpClientHandler clientHandler = new()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    Proxy = new WebProxy(new Uri($"http://{proxy}"))
                };

                Client = new(clientHandler); // creates/recreates the client instance, in order to be fresh, using the selected proxy.
                timeout = TimeSpan.FromSeconds(10); // setting the timeout to 10 in order to give slower proxies time to finish the request.
            }
            else
            {
                Client = new(); // creates/recreates the client instance in order to be fresh.
            }

            HttpResponseMessage response;
            using (Client)
            {
                // Setting http version. Defaults to 1.1 if not specified or specified a invalid value.
                if (httpVersion == null)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 1.0)
                    Client.DefaultRequestVersion = HttpVersion.Version10;
                else if (httpVersion == 1.1)
                    Client.DefaultRequestVersion = HttpVersion.Version11;
                else if (httpVersion == 2.0)
                    Client.DefaultRequestVersion = HttpVersion.Version20;
                else
                    Client.DefaultRequestVersion = HttpVersion.Version11;

                // Setting version policy. Defaults to request's version or higher if not specified.
                if (versionPolicy == null)
                    Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                else
                    Client.DefaultVersionPolicy = (HttpVersionPolicy)versionPolicy;

                // Setting headers if any.
                if (headers != null)
                {
                    Client.DefaultRequestHeaders.Clear();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        Client.DefaultRequestHeaders.Add(headers.Keys.ElementAt(i), headers.Values.ElementAt(i));
                    }
                }

                // Setting params if any.
                if (parameters != null)
                {
                    url += "?"; // symbol at the end which specifies there would be upcoming params.

                    // Manually setting the first param in order to not get a "&" at the end.
                    url += parameters.Keys.ElementAt(0) + "=" + parameters.Values.ElementAt(0);

                    // Automatically setting the rest of params if any.
                    for (int i = 1; i < parameters.Count; i++)
                    {
                        url += "&" + parameters.Keys.ElementAt(i) + "=" + parameters.Values.ElementAt(i);
                    }
                }

                // Setting timeout if any.
                if (timeout != null)
                    Client.Timeout = (TimeSpan)timeout;

                // Setting max response content buffer size if specified.
                if (maxResponseContentBufferSize != null)
                    Client.MaxResponseContentBufferSize = (long)maxResponseContentBufferSize;

                // Setting the chosen dnt header value if chose.
                if (dnt != null)
                {
                    if ((bool)dnt)
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    }
                    else
                    {
                        Client.DefaultRequestHeaders.Remove("dnt");
                        Client.DefaultRequestHeaders.Add("dnt", 0.ToString());
                    }
                }

                // Anonymize request as much request as much as possible if requested.
                if (anonymizeRequest == true)
                {
                    // Setting some headers that provide more anonymity. Not much but it's something.
                    Random random = new();
                    Client.DefaultRequestHeaders.Remove("x-forwarded-for");
                    Client.DefaultRequestHeaders.Add("x-forwarded-for", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("x-forwarded-host");
                    Client.DefaultRequestHeaders.Add("x-forwarded-host", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("forwarded");
                    Client.DefaultRequestHeaders.Add("forwarded", "by=" + random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("user-agent");
                    Client.DefaultRequestHeaders.Add("user-agent", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("referer");
                    Client.DefaultRequestHeaders.Add("referer", random.Next().ToString());
                    Client.DefaultRequestHeaders.Remove("accept-language");
                    Client.DefaultRequestHeaders.Add("accept-language", "en-US");
                    Client.DefaultRequestHeaders.Remove("dnt");
                    Client.DefaultRequestHeaders.Add("dnt", 1.ToString());
                    Client.DefaultRequestHeaders.Remove("cache-control");
                    Client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    Client.DefaultRequestHeaders.Add("pragma", "no-cache");
                }

                // Setting up the request
                HttpRequestMessage? request = new()
                {
                    Method = HttpMethod.Options,
                    RequestUri = new Uri(url)
                };

                // Doing the actual request and saving the response.
                try
                {
                    response = await Client.SendAsync(request);
                }
                catch (TaskCanceledException)
                {
                    response = new();
                    if (timeout != null)
                        response.Headers.Add("error", "The request could not be completed in the given timeout of " + timeout.Value.TotalSeconds + " seconds");
                    else
                        response.Headers.Add("error", "The request could not be completed in the given timeout");
                }
                catch (Exception e)
                {
                    response = new();
                    response.Headers.Add("error", e.Message);
                }
            }

            return response;
        }
#nullable disable
    }
}