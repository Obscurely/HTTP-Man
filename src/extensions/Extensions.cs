using System;
using System.Net.Http;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HTTPMan
{
    /// <summary>
    /// The class where object extension methods go in.
    /// </summary>
    public static class Extensions
    {
        // *************************
        // * Dictionary Extensions *
        // *************************

        /// <summary>
        /// Extension method that converts a Dictionary<string, string> to it's json string representation.
        /// </summary>
        /// <returns>String containing the given dict as a json string.</returns>
        public static string ToJsonString(this Dictionary<string, string> dict)
        {
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.WriteIndented = true;
            return JsonSerializer.Serialize(dict, typeof(Dictionary<string, string>), jsonOptions);
        }

        /// <summary>
        /// Extension method that converts a string to a Dictionary<string, string>.
        /// </summary>
        /// <returns>Object of type Dictionary<string, string> from the string.</returns>
        public static Dictionary<string, string> ToDict(this string str)
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(str);
        }

        /// <summary>
        /// Extension method that converts a Dictionary<string, object> to it's json string representation.
        /// </summary>
        /// <returns>String containing the given dict as a json string.</returns>
        public static string ToJsonString(this Dictionary<string, object> dict)
        {
            string json = "{\n";
            for (int i = 0; i < dict.Count; i++)
            {
                json += "\t";

                bool boolResult;
                int intResult;
                double doubleResult;
                if (bool.TryParse(dict.Values.ElementAt(i).ToString(), out boolResult))
                {
                    json += "\"" + dict.Keys.ElementAt(i) + "\": " + boolResult.ToString().ToLower() + ",\n";
                }
                else if (int.TryParse(dict.Values.ElementAt(i).ToString(), out intResult))
                {
                    json += "\"" + dict.Keys.ElementAt(i) + "\": " + intResult + ",\n";
                }
                else if (double.TryParse(dict.Values.ElementAt(i).ToString(), out doubleResult)) 
                {
                    json += "\"" + dict.Keys.ElementAt(i) + "\": " + doubleResult + ",\n";
                }
                else if (dict.Values.ElementAt(i).ToString().ToLower().Equals("null"))
                {
                    json += "\"" + dict.Keys.ElementAt(i) + "\": " + "null" + ",\n";
                }
                else
                {
                    json += "\"" + dict.Keys.ElementAt(i) + "\": \"" + dict.Values.ElementAt(i).ToString() + "\",\n";
                }
            }
            json += "}";
            return json;
        }

        /// <summary>
        /// Checks if keys and values of 2 Dictionary<string, string> are the same.
        /// </summary>
        /// <param name="dict2">The dictionary to compare to.</param>
        /// <returns>True if the keys and values of the 2 dictionaries match otherwise false.</returns>
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


        // *********************
        // * String Extensions *
        // *********************

        /// <summary>
        /// Only replaces the first occurrence of the substring.
        /// </summary>
        /// <param name="search">Old substring.</param>
        /// <param name="replace">New substring.</param>
        /// <returns>A string with the first occurrence of the old substring replaced.</returns>
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }


        // ******************************
        // * Mocking Objects Extensions *
        // ******************************

        /// <summary>
        /// Extension method that converts a MockHttpMethod to it's string in upper case representation.
        /// </summary>
        /// <returns></returns>
        public static string GetString(this MockHttpMethod method)
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

        /// <summary>
        /// Extension method that gets the option's string key for the current matcher.
        /// </summary>
        /// <returns>The option's string key for this matcher.</returns>
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

        /// <summary>
        /// Extension method that gets the option's string key for the current action.
        /// </summary>
        /// <returns>The option's string key for this action.</returns>
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
                    return "transformer";
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