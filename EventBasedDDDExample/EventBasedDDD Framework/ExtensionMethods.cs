using System;
using System.Collections.Generic;

namespace EventBasedDDD
{
    public static class BasicTypeExtensions
    {
        public static bool HasValue(this string source)
        {
            return !string.IsNullOrEmpty(source);
        }

        public static bool HasValue(this bool? source)
        {
            return source.HasValue;
        }

        public static bool HasValue(this DateTime? source)
        {
            return source.HasValue;
        }

        public static bool HasValue(this int? source)
        {
            return source.HasValue;
        }

        public static bool HasValue(this double? source)
        {
            return source.HasValue;
        }

        public static bool HasValue(this Guid source)
        {
            return source != Guid.Empty;
        }
    }

    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                action(item);
            }
        }
    }

    public static class MiscExtensions
    {
        #region Business Error Mapping

        public static List<ErrorItem> ToErrorItemList(this IEnumerable<ValidationErrorItem> validationErrorItems)
        {
            List<ErrorItem> items = new List<ErrorItem>();
            validationErrorItems.ForEach(validationErrorItem => items.Add(new ErrorItem { Key = validationErrorItem.ErrorKey, Parameters = validationErrorItem.Parameters }));
            return items;
        }

        #endregion
    }
}
