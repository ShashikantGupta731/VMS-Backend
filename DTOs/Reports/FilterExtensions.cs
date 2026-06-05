using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

namespace backend.DTOs.Reports
{
    public static class FilterExtensions
    {
        public static int? GetInt(this Dictionary<string, object> filters, string key)
        {
            if (filters == null) return null;
            
            var match = filters.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (match == null) return null;
            
            var val = filters[match];
            if (val == null) return null;

            if (val is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.Number && jsonElement.TryGetInt32(out var intVal))
                    return intVal;
                if (jsonElement.ValueKind == JsonValueKind.String && int.TryParse(jsonElement.GetString(), out var strIntVal))
                    return strIntVal;
            }
            else if (int.TryParse(val.ToString(), out var parsedVal))
            {
                return parsedVal;
            }
            
            return null;
        }

        public static string GetString(this Dictionary<string, object> filters, string key)
        {
            if (filters == null) return null;

            var match = filters.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (match == null) return null;
            
            var val = filters[match];
            if (val == null) return null;

            if (val is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String)
                return jsonElement.GetString();
                
            return val.ToString();
        }
    }
}
