using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    internal static class MinMaxHelper
    {
        internal static TValue MinValue<TItem, TValue>(IEnumerable<TItem> items, Predicate<TItem> itemFilter, Converter<TItem, TValue> valueGetter, TValue nullValue)
        {
            return CollectionUtils.Min(GetSourceValues(items, itemFilter, valueGetter, nullValue), nullValue);
        }

        internal static TValue MaxValue<TItem, TValue>(IEnumerable<TItem> items, Predicate<TItem> itemFilter, Converter<TItem, TValue> valueGetter, TValue nullValue)
        {
            return CollectionUtils.Max(GetSourceValues(items, itemFilter, valueGetter, nullValue), nullValue);
        }


        private static IEnumerable<TValue> GetSourceValues<TItem, TValue>(IEnumerable<TItem> items, Predicate<TItem> itemFilter, Converter<TItem, TValue> valueGetter, TValue nullValue)
        {
            List<TItem> sources = CollectionUtils.Select(items, itemFilter);

            return CollectionUtils.Select(
                CollectionUtils.Map(sources, valueGetter),
                    delegate(TValue value) { return !Equals(value, nullValue); });
        }
    }
}
