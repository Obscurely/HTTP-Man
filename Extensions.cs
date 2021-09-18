using System.Linq;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

namespace HTTPMan
{
    /// <summary>
    /// The class where object extension methods go in.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Extension method that converts a Dictionary<string, string> to it's json string representation.
        /// </summary>
        /// <param name="dict">The objects its self.</param>
        /// <returns></returns>
        public static string ToJsonString(this Dictionary<string, string> dict)
        {
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.WriteIndented = true;
            return JsonSerializer.Serialize(dict, typeof(Dictionary<string, string>), jsonOptions);
        }

        /// <summary>
        /// Extension method that converts a Dictionary<string, object> to it's json string representation.
        /// </summary>
        /// <param name="dict">The object its self.</param>
        /// <returns></returns>
        public static string ToJsonString(this Dictionary<string, object> dict)
        {
            string json = "{\n";
            for (int i = 0; i < dict.Count; i++)
            {
                json += "\t";

                bool boolResult;
                int intResult;
                if (bool.TryParse(dict.Values.ElementAt(i).ToString(), out boolResult))
                {
                    json += "\"" + dict.Keys.ElementAt(i) + "\": " + boolResult.ToString().ToLower() + "\n";
                }
                else if (int.TryParse(dict.Values.ElementAt(i).ToString(), out intResult))
                {
                    json += "\"" + dict.Keys.ElementAt(i) + "\": " + intResult.ToString() + "\n";
                }
                else if (dict.Values.ElementAt(i).ToString().ToLower().Equals("null"))
                {
                    json += "\"" + dict.Keys.ElementAt(i) + "\": " + "null" + "\n";
                }
                else
                {
                    json += "\"" + dict.Keys.ElementAt(i) + "\": \"" + dict.Values.ElementAt(i).ToString() + "\"\n";
                }
            }
            json += "}";
            return json;
        }

        /// <summary>
        /// Only replaces the first occurrence of the substring.
        /// </summary>
        /// <param name="text">the actual string object to replace.</param>
        /// <param name="search">Old substring.</param>
        /// <param name="replace">New substring.</param>
        /// <returns>Returns a string with the first occurrences of the old substring replaced.</returns>
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// Checks if keys and values of 2 Dictionary<string, string> are the same.
        /// </summary>
        /// <param name="dict1">The first dictionary to compare.</param>
        /// <param name="dict2">The second dictionary to compare to.</param>
        /// <returns>Returns true if the keys and values of the 2 dictionaries match otherwise false.</returns>
        public static bool ContentEquals(this Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            if (dict1.Count != dict2.Count)
                return false;

            for (int i = 0; i < dict1.Count; i++)
            {
                if (dict1.Keys.ElementAt(i).Equals(dict2.Keys.ElementAt(i)) && dict1.Values.ElementAt(i).Equals(dict2.Values.ElementAt(i)))
                    continue;
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Extension method that converts a MockHttpMethod to it's string in upper case representation.
        /// </summary>
        /// <param name="MockHttpMethod">The object its self</param>
        /// <param name="isNotDefaultMethod">Can be set to whatever is just so it will be differentiated from the original one.</param>
        /// <returns></returns>
        public static string ToString(this MockHttpMethod method, bool isNotDefaultMethod)
        {
            switch ((int)method)
            {
                case 0:
                    return "GET";
                case 1:
                    return "POST";
                case 2:
                    return "PUT";
                case 3:
                    return "PATCH";
                case 4:
                    return "DELETE";
                case 5:
                    return "HEAD";
                case 6:
                    return "OPTIONS";
                case 7:
                    return "TRACE";
                case 8:
                    return "ANY";

                default:
                    return "ANY";
            }
        }

        public static string GetOptionsKey(this MockMatcher matcher)
        {
            switch ((int)matcher)
            {
                case 0:
                    return "host";
                case 1:
                    return "url";
                case 2:
                    return "regexPattern";
                case 3:
                    return "query";
                case 4:
                    return "";
                case 5:
                    return "exactBody";
                case 6:
                    return "partBody";
                case 7:
                    return "exactJsonBody";
                case 8:
                    return "partJsonBody";

                default:
                    return "";
            }
        }

        public static string GetOptionsKey(this MockAction action)
        {
            switch ((int)action)
            {
                case 0:
                    return ""; // Not needed.
                case 1:
                    return ""; // Not needed.
                case 2:
                    return ""; // Not needed.
                case 3:
                    return ""; // Not needed.
                case 4:
                    return "response";
                case 5:
                    return "host";
                case 6:
                    return "transformer"; // TODO: make object for this one (values for responses and requests.)
                case 7:
                    return ""; // Not needed.
                case 8:
                    return ""; // Not needed.

                default:
                    return "";
            }
        }
    }
}