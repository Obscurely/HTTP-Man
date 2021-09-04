using System.Linq;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

namespace HTTPMan
{
    public static class Extensions
    {
        public static string ToJsonString(this Dictionary<string, string> dict)
        {
            JsonSerializerOptions jsonOptions = new();
            jsonOptions.WriteIndented = true;
            return JsonSerializer.Serialize(dict, typeof(Dictionary<string, string>), jsonOptions);
        }

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
    }
}