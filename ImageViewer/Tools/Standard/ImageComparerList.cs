#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    public class ImageComparerList
    {
        private class ItemConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                var itemDescription = value as string;
                return CollectionUtils.SelectFirst(Items, item => item.Description == itemDescription);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
            {
                if (destinationType != typeof(string) || value == null)
                    return null;

                return ((Item)value).Description;
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new List<Item>(EnumerateStandardValues()));
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return false;
            }

            private static IEnumerable<Item> EnumerateStandardValues()
            {
                //yield return null;
                foreach (var item in Items)
                    yield return item;
            }
        }

        [TypeConverter(typeof(ItemConverter))]
        public class Item
        {
            private readonly IResourceResolver _resourceResolver;
            private readonly string _description;

            internal Item(ImageComparer imageComparer, bool reverse)
            {
                _resourceResolver = new ResourceResolver(imageComparer.GetType(), false);
                _description = imageComparer.Description;
                Name = imageComparer.Name;
                IsReverse = reverse;
                Comparer = imageComparer.GetComparer(reverse);
            }

            public string Name { get; private set; }
            public bool IsReverse { get; private set; }
            public IComparer<IPresentationImage> Comparer { get; private set; }

            public string Description
            {
                get { return !IsReverse ? _resourceResolver.LocalizeString(_description) : string.Format(SR.FormatSortByReverse, _resourceResolver.LocalizeString(_description)); }
            }
        }

        public static ReadOnlyCollection<Item> Items { get; private set; }

        static ImageComparerList()
        {
            var items = new List<Item>();

            foreach (var comparer in ImageComparer.CreateAll())
            {
                items.Add(new Item(comparer, false));
                items.Add(new Item(comparer, true));
            }

            Items = items.AsReadOnly();
        }
    }
}
