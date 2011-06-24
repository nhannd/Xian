#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layout;

namespace ClearCanvas.ImageViewer.Tools.Standard.HangingProtocols
{
    internal interface IHpSortComparer
    {
        string Name { get; }
        string Description { get; }
        //bool Reverse { get; }

        IComparer<IPresentationImage> GetComparer();
    }

    [ExtensionOf(typeof(HpImageBoxDefinitionContributorExtensionPoint), Enabled = true)]
    internal class HpSortContributor : IHpImageBoxDefinitionContributor
    {
        [HpDataContract("{D9CDB500-314B-488b-B335-3A2ACB484E9B}")]
        private class Data
        {
            public string Name { get; set; }
            //bool Reverse { get; set; }
        }

        private static HpSortComparer _selected;
        
        #region IHpImageBoxDefinitionContributor Members

        public void Capture(IHpImageBoxDefinitionContext context)
        {
            if (context.ImageBox != null && context.ImageBox.DisplaySet != null)
                _selected = HpSortComparer.All.Find(item => item.GetComparer().Equals(context.ImageBox.DisplaySet.PresentationImages.SortComparer));
        }

        public void ApplyTo(IHpImageBoxDefinitionContext context)
        {
            if (context.ImageBox != null && context.ImageBox.DisplaySet != null && _selected != null)
                context.ImageBox.DisplaySet.PresentationImages.Sort(_selected.GetComparer());
        }

        #endregion

        #region IHpContributor Members

        public string ContributorId
        {
            get { return "{A0A8E53C-C628-4734-AFED-7336068BE652}"; }
        }

        public IHpProperty[] GetProperties()
        {
            return new IHpProperty[]
                       {
                           new HpProperty<HpSortComparer>(SR.PropertyNameSortBy, SR.PropertyDescriptionSortBy, 
                               () => _selected, sortItem => _selected = sortItem)
                       };
        }

        #endregion

        #region IHpSerializableElement Members

        public object GetState()
        {
            return (_selected == null) ? null : new Data {Name = _selected.Name};
        }

        public void SetState(object state)
        {
            var data = state as Data;
            if (data != null)
                _selected = HpSortComparer.All.Find(item => item.Name == data.Name);
        }

        #endregion
    }
}