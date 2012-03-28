#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Comparers;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [ExtensionPoint]
    public sealed class ImageComparerFactoryExtensionPoint : ExtensionPoint<IImageComparerFactory>
    {
    }

    public class ImageComparer
    {
        private readonly IComparer<IPresentationImage> _comparer;

        public ImageComparer(string name, string description, IComparer<IPresentationImage> comparer)
        {
            Name = name;
            Description = description;
            _comparer = comparer;
        }

        #region IImageComparer Members

        public string Name { get; private set; }
        public string Description { get; private set; }
        public IComparer<IPresentationImage> GetComparer(bool reverse)
        {
            return !reverse ? _comparer : new ReverseComparer<IPresentationImage>(_comparer);
        }

        #endregion

        public static List<ImageComparer> CreateAll()
        {
            List<ImageComparer> comparers = CreateStockComparers();

            try
            {
                foreach (IImageComparerFactory factory in new ImageComparerFactoryExtensionPoint().CreateExtensions())
                    comparers.AddRange(factory.CreateComparers());
            }
            catch (NotSupportedException)
            {
            }

            comparers.Sort((c1, c2) => String.Compare(c1.Description, c2.Description));
            return comparers;
        }

        private static List<ImageComparer> CreateStockComparers()
        {
            return new List<ImageComparer>
                       {
                           new ImageComparer("Instance Number", "SortByImageNumberDescription", new InstanceAndFrameNumberComparer()),
                           new ImageComparer("Acquisition Time", "SortByAcquisitionTimeDescription", new AcquisitionTimeComparer()),
                           new ImageComparer("Slice Location", "SortBySliceLocationDescription", new SliceLocationComparer())
                       };
        }
    }

    public interface IImageComparerFactory
    {
        List<ImageComparer> CreateComparers();
    }
}