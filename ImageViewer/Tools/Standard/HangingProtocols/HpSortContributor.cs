#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layout;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard.HangingProtocols
{
    [ExtensionOf(typeof(HpImageBoxDefinitionContributorExtensionPoint), Enabled = true)]
    internal class HpSortContributor : IHpImageBoxDefinitionContributor
    {
        [HpDataContract("{D9CDB500-314B-488b-B335-3A2ACB484E9B}")]
        private class Data
        {
            public string Name { get; set; }
            public bool Reverse { get; set; }
        }

        private ImageComparerList.Item _selected;
        
        #region IHpImageBoxDefinitionContributor Members

        public void Capture(IHpImageBoxDefinitionContext context)
        {
            if (context.ImageBox != null && context.ImageBox.DisplaySet != null)
                _selected = CollectionUtils.SelectFirst(ImageComparerList.Items, item => item.Comparer.Equals(context.ImageBox.DisplaySet.PresentationImages.SortComparer));
            else
                _selected = null;
        }

        public void ApplyTo(IHpImageBoxDefinitionContext context)
        {
            if (_selected != null && context.ImageBox != null && context.ImageBox.DisplaySet != null)
            {
                context.ImageBox.DisplaySet.PresentationImages.Sort(_selected.Comparer);
                context.ImageBox.TopLeftPresentationImageIndex = 0;
                context.ImageBox.SelectDefaultTile();
            }
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
                           new HpProperty<ImageComparerList.Item>(SR.PropertyNameSortBy, SR.PropertyDescriptionSortBy, 
                               () => _selected, sortItem => _selected = sortItem)
                       };
        }

        #endregion

        #region IHpSerializableElement Members

        public object GetState()
        {
            return (_selected == null) ? null : new Data {Name = _selected.Name, Reverse = _selected.IsReverse };
        }

        public void SetState(object state)
        {
            var data = state as Data;
            _selected = data == null ? null : CollectionUtils.SelectFirst(ImageComparerList.Items, item => item.Name == data.Name && item.IsReverse == data.Reverse);
        }

        #endregion
    }
}