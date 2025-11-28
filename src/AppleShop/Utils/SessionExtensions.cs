using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppleShop.Utils
{
    public static class SessionExtensions
    {
      
        private static readonly JsonSerializerOptions SafeOptions = new JsonSerializerOptions
        {
    
            PropertyNameCaseInsensitive = true,
    
            NumberHandling = JsonNumberHandling.AllowReadingFromString,

            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,

            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

            ReadCommentHandling = JsonCommentHandling.Skip,
         
            AllowTrailingCommas = true,
        };

      
        public static void SetObject<T>(this ISession session, string key, T value)
        {
      
            if (value == null)
            {
                session.Remove(key);
                return;
            }

            var json = JsonSerializer.Serialize(value, SafeOptions);
            session.SetString(key, json);
        }

      
        public static T? GetObject<T>(this ISession session, string key)
        {
            var str = session.GetString(key);
            if (string.IsNullOrWhiteSpace(str))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(str, SafeOptions);
            }
            catch
            {
              
                return default;
            }
        }

        public static T GetObjectOr<T>(this ISession session, string key, T fallbackIfFail)
        {
            var obj = session.GetObject<T>(key);
            if (obj == null)
                return fallbackIfFail;
            return obj;
        }

     
        public static void RemoveObject(this ISession session, string key)
        {
            session.Remove(key);
        }
    }
}
