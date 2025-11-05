using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppleShop.Utils
{
    public static class SessionExtensions
    {
        // Dùng chung một bộ options "dễ tính" để hạn chế lỗi chuyển kiểu
        private static readonly JsonSerializerOptions SafeOptions = new JsonSerializerOptions
        {
            // không phân biệt HOA/thường tên thuộc tính (phù hợp khi model đổi casing)
            PropertyNameCaseInsensitive = true,

            // cho phép số dưới dạng chuỗi (tránh lỗi khi decimal/int được lưu thành string)
            NumberHandling = JsonNumberHandling.AllowReadingFromString,

            // bỏ qua field/properties lạ (tránh lỗi khi thêm/bớt cột trong VM)
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,

            // bỏ qua null khi ghi; khi đọc thì null -> giá trị mặc định của kiểu
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

            // Giữ comment nếu lỡ có dữ liệu kèm comment
            ReadCommentHandling = JsonCommentHandling.Skip,
            // Cho phép JSON không chuẩn có dấu phẩy thừa (nếu từng lưu tay)
            AllowTrailingCommas = true,
        };

        /// <summary>
        /// Lưu object vào Session dưới dạng JSON.
        /// </summary>
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            // Nếu value null thì xóa khỏi session để tránh lưu "null" (gây lỗi khi đọc)
            if (value == null)
            {
                session.Remove(key);
                return;
            }

            var json = JsonSerializer.Serialize(value, SafeOptions);
            session.SetString(key, json);
        }

        /// <summary>
        /// Đọc object từ Session (an toàn). Nếu lỗi parse hoặc không có dữ liệu -> trả về default(T).
        /// </summary>
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
                // Dữ liệu cũ bị "mồ côi" / khác kiểu -> đừng quăng exception, trả về default
                // Bạn có thể log lại nếu muốn:
                // Console.WriteLine($"[Session] Cannot deserialize key '{key}' to {typeof(T).Name}");
                return default;
            }
        }

        /// <summary>
        /// Cố gắng lấy object; nếu thất bại trả về fallback (ví dụ: new List&lt;CartItemVM&gt;()).
        /// </summary>
        public static T GetObjectOr<T>(this ISession session, string key, T fallbackIfFail)
        {
            var obj = session.GetObject<T>(key);
            if (obj == null)
                return fallbackIfFail;
            return obj;
        }

        /// <summary>
        /// Xóa khóa trong Session một cách an toàn.
        /// </summary>
        public static void RemoveObject(this ISession session, string key)
        {
            session.Remove(key);
        }
    }
}
