using System.Collections.Generic;
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
