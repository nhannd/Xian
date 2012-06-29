#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System.Collections.Generic;

namespace ClearCanvas.Common.Specifications
{
    public abstract class CompositeSpecification : Specification
    {
        private List<ISpecification> _childSpecs = new List<ISpecification>();

        public CompositeSpecification()
        {
        }

        public void Add(ISpecification spec)
        {
            _childSpecs.Add(spec);
        }

        public void AddRange(IEnumerable<ISpecification> specs)
        {
            _childSpecs.AddRange(specs);
        }

        public bool IsEmpty
        {
            get { return _childSpecs.Count == 0; }
        }

        public IEnumerable<ISpecification> Elements
        {
            get { return _childSpecs; }
        }
    }
}
