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
using System.Collections;

namespace ClearCanvas.ImageViewer.Comparers
{
    public sealed class ReverseComparer<T> : IComparer<T>, IEquatable<ReverseComparer<T>>
    {
        private readonly Comparison<T> _realComparer;

        public ReverseComparer(IComparer<T> realComparer)
            : this(realComparer.Compare)
        {
        }

        public ReverseComparer(Comparison<T> realComparer)
        {
            _realComparer = realComparer;
        }

        public int Compare(T x, T y)
        {
            return -_realComparer(x, y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ReverseComparer<T>)
                return Equals((ReverseComparer<T>) obj);
            return false;
        }

        #region IEquatable<ReverseComparer<T>> Members

        public bool Equals(ReverseComparer<T> other)
        {
            return other != null &&  _realComparer.Target.Equals(other._realComparer.Target);
        }

        #endregion
    }

    public sealed class ReverseComparer : IComparer, IEquatable<ReverseComparer>
    {
        private readonly Comparison<object> _realComparer;

        public ReverseComparer(IComparer realComparer)
            : this(realComparer.Compare)
        {
        }

        public ReverseComparer(Comparison<object> realComparer)
        {
            _realComparer = realComparer;
        }

        public int Compare(object x, object y)
        {
            return -_realComparer(x, y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ReverseComparer)
                return Equals((ReverseComparer) obj);
            return false;
        }

        #region IEquatable<ReverseComparer Members

        public bool Equals(ReverseComparer other)
        {
            return other != null &&  _realComparer.Target.Equals(other._realComparer.Target);
        }

        #endregion
    }
}
