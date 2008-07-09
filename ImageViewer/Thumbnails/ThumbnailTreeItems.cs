#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	public partial class ThumbnailComponent
	{
		private interface IImageSetTreeItem
		{
			string Description { get; }
		}

		private class ImageSetTreeItemBinding : TreeItemBindingBase
		{
			public override string GetNodeText(object item)
			{
				return ((IImageSetTreeItem) item).Description;
			}

			public override string GetTooltipText(object item)
			{
				return GetNodeText(item);
			}

			public override bool GetExpanded(object item)
			{
				if (item is PatientTreeItem)
					return ((PatientTreeItem) item).IsIntiallyExpanded;

				return false;
			}

			public override bool CanHaveSubTree(object item)
			{
				return item is PatientTreeItem;
			}

			public override ITree GetSubTree(object item)
			{
				if (item is PatientTreeItem)
					return ((PatientTreeItem) item).ImageSetSubTree;

				return null;
			}
		}

		private class PatientTreeItem : IImageSetTreeItem
		{
			private readonly string _patientInfo;
			private readonly Tree<IImageSetTreeItem> _imageSetSubTree;
			private bool _isInitiallyExpanded;

			public PatientTreeItem(string patientInfo)
			{
				_patientInfo = patientInfo;
				_imageSetSubTree = new Tree<IImageSetTreeItem>(new ImageSetTreeItemBinding());
				_isInitiallyExpanded = false;
			}

			public bool IsIntiallyExpanded
			{
				get { return _isInitiallyExpanded; }
				set { _isInitiallyExpanded = value; }
			}

			public string PatientInfo
			{
				get { return _patientInfo; }	
			}

			public Tree<IImageSetTreeItem> ImageSetSubTree
			{
				get { return _imageSetSubTree; }
			}

			#region IImageSetTreeItem Members

			public string Description
			{
				get { return _patientInfo; }
			}

			#endregion
		}

		private class ImageSetTreeItem : IImageSetTreeItem
		{
			private readonly IImageSet _imageSet;

			public ImageSetTreeItem(IImageSet imageSet)
			{
				_imageSet = imageSet;
			}

			public IImageSet ImageSet
			{
				get { return _imageSet; }
			}

			#region IImageSetTreeItem Members

			public string Description
			{
				get { return _imageSet.Name; }
			}

			#endregion
		}
	}
}
