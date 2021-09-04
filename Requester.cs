using System.Net.Http.Headers;
using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HTTPMan
{
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

        private string PickProxy(List<string> proxies)
        {
            Random random = new(Guid.NewGuid().GetHashCode());
            string proxy = proxies[random.Next(proxies.Count)];
            return proxy;
        }

#nullable enable
        public async Task<HttpResponseMessage> GetRequest(string url, double? httpVersion = null, HttpVersionPolicy? versionPolicy = null, Dictionary<string, string>? headers = null,
            Dictionary<string, string>? parameters = null, string? content = null, HttpContentType? contentType = null, TimeSpan? timeout = null, long? maxResponseContentBufferSize = null,
            bool? acceptCache = null, bool? dnt = null, bool anonymizeRequest = false, bool useProxies = false)
        {
            // Setting proxy if user chose to use one.
            if (useProxies)
            {
                if (Proxies.Count() == 0)
                {
                    Proxies = await GetProxies();
                }
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
                            Content = new StringContent(content, Encoding.UTF8, HttpContentTypeString.Get((int)contentType))
                        };
                    }
                    else
                    {
                        request = new()
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri(url),
                            Content = new StringContent(content, Encoding.UTF8, HttpContentTypeString.Get((int)HttpContentType.TextPlain))
                        };
                    }
                }

                // Setting timeout if any.
                if (timeout != null)
                    Client.Timeout = (TimeSpan)timeout;

                // Setting max response content buffer size if specified.
                if (maxResponseContentBufferSize != null)
                {
                    Client.MaxResponseContentBufferSize = (long)maxResponseContentBufferSize;
                }

                // Setting if request accepts cache or not.
                if (acceptCache != null && Client.DefaultRequestHeaders.CacheControl != null)
                {
                    Client.DefaultRequestHeaders.CacheControl.NoCache = (bool)acceptCache;
                }

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
                        response = await Client.GetAsync(url);
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
            }

            return response;
        }
#nullable disable

#nullable enable
        public async Task<HttpResponseMessage> PostRequest(string url, double? httpVersion = null, HttpVersionPolicy? versionPolicy = null, Dictionary<string, string>? headers = null,
            Dictionary<string, string>? parameters = null, string? content = null, HttpContentType? contentType = null, TimeSpan? timeout = null, long? maxResponseContentBufferSize = null,
            bool? acceptCache = null, bool? dnt = null, bool anonymizeRequest = false, bool useProxies = false)
        {
            // Setting proxy if user chose to use one.
            if (useProxies)
            {
                if (Proxies.Count() == 0)
                {
                    Proxies = await GetProxies();
                }
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
                            Content = new StringContent(content, Encoding.UTF8, HttpContentTypeString.Get((int)contentType))
                        };
                    }
                    else
                    {
                        request = new()
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri(url),
                            Content = new StringContent(content, Encoding.UTF8, HttpContentTypeString.Get((int)HttpContentType.TextPlain))
                        };
                    }
                }

                // Setting timeout if any.
                if (timeout != null)
                    Client.Timeout = (TimeSpan)timeout;

                // Setting max response content buffer size if specified.
                if (maxResponseContentBufferSize != null)
                {
                    Client.MaxResponseContentBufferSize = (long)maxResponseContentBufferSize;
                }

                // Setting if request accepts cache or not.
                if (acceptCache != null && Client.DefaultRequestHeaders.CacheControl != null)
                {
                    Client.DefaultRequestHeaders.CacheControl.NoCache = (bool)acceptCache;
                }

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
                        response = await Client.GetAsync(url);
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
            }

            return response;
        }
#nullable disable

        /*
        public async Task<HttpResponseMessage> PostRequest(string url, Dictionary<string, string> body)
        {
            // Setting body.
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.WriteIndented = true;
            StringContent requestBody = new(JsonSerializer.Serialize(body, typeof(Dictionary<string, string>), jsonOptions), Encoding.UTF8, "application/json");
        }
        */
    }
}