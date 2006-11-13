using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer
{
	public class DisplaySet : IDisplaySet
	{
		#region Private Fields

		private PresentationImageCollection _presentationImages;
		private IImageViewer _imageViewer;
		private LogicalWorkspace _parentLogicalWorkspace;
		private ImageBox _imageBox;
		private List<IPresentationImage> _linkedPresentationImages = new List<IPresentationImage>();

		private bool _selected = false;
		private bool _linked = false;
		private string _name;

		#endregion

		public DisplaySet()
		{
			this.PresentationImages.ItemAdded += new EventHandler<PresentationImageEventArgs>(OnPresentationImageAdded);
			this.PresentationImages.ItemRemoved += new EventHandler<PresentationImageEventArgs>(OnPresentationImageRemoved);
		}

		#region Properties
		
		/// <summary>
		/// Gets a collection of presentation images.
		/// </summary>
		public PresentationImageCollection PresentationImages
		{
			get 
			{
				if (_presentationImages == null)
					_presentationImages = new PresentationImageCollection();

				return _presentationImages; 
			}
		}

		/// <summary>
		/// Gets a collection of linked <see cref="PresentationImages"/>
		/// </summary>
		/// <value>A collection of linked <see cref="PresentationImages"/></value>
		public ReadOnlyCollection<IPresentationImage> LinkedPresentationImages
		{
			get { return _linkedPresentationImages.AsReadOnly(); }
		}

		/// <summary>
		/// Gets the parent <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>Can be <b>null</b> if the <see cref="DisplaySet"/> has not
		/// been added to a <see cref="LogicalWorkspace"/> yet.</value>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			internal set 
			{
				_imageViewer = value;

				if (_imageViewer != null)
				{
					foreach (PresentationImage image in this.PresentationImages)
						image.ImageViewer = value;
				}
			}
		}

		/// <summary>
		/// Gets the parent <see cref="LogicalWorkspace"/>
		/// </summary>
		/// <value>Can be <b>null</b> if the <see cref="DisplaySet"/> has not
		/// been added to a <see cref="LogicalWorkspace"/> yet.</value>
		public ILogicalWorkspace ParentLogicalWorkspace
		{
			get { return _parentLogicalWorkspace as ILogicalWorkspace; }
			internal set { _parentLogicalWorkspace = value as LogicalWorkspace; }
		}

		public IImageBox ImageBox
		{
			get { return _imageBox as IImageBox; }
			internal set { _imageBox = value as ImageBox; }
		}

		/// <summary>
		/// Gets or sets the name of the display set.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="DisplaySet"/> is visible.
		/// </summary>
		public bool Visible
		{
			get { return this.ImageBox != null; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="DisplaySet"/> is selected.
		/// </summary>
		public bool Selected
		{
			get { return _selected; }
			internal set
			{
				if (_selected != value)
				{
					_selected = value;

					if (_selected)
					{
						if (this.ImageViewer == null)
							return;

						this.ImageViewer.EventBroker.OnDisplaySetSelected(
							new DisplaySetSelectedEventArgs(this));
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ImageBox"/> is
		/// linked.
		/// </summary>
		/// <value><b>true</b> if linked; <b>false</b> otherwise.</value>
		/// <remarks>
		/// Multiple display sets may be linked, allowing tools that can operate on
		/// multiple display sets to operate on all linked display sets simultaneously.  
		/// Note that the concept of linkage is slightly different from selection:
		/// it is possible for an <see cref="DisplaySet"/> to be 1) selected but not linked
		/// 2) linked but not selected and 3) selected and linked.
		/// </remarks>
		public bool Linked
		{
			get { return _linked; }
			set
			{
				if (_linked != value)
				{
					_linked = value;

					if (_linked)
						_parentLogicalWorkspace.LinkDisplaySet(this);
					else
						_parentLogicalWorkspace.UnlinkDisplaySet(this);
				}
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposePresentationImages();
			}
		}

		private void DisposePresentationImages()
		{
			if (this.PresentationImages == null)
				return;

			foreach (PresentationImage image in this.PresentationImages)
				image.Dispose();

			_presentationImages.ItemAdded -= new EventHandler<PresentationImageEventArgs>(OnPresentationImageAdded);
			_presentationImages.ItemRemoved -= new EventHandler<PresentationImageEventArgs>(OnPresentationImageAdded);
			_presentationImages = null;
		}

		public IDisplaySet Clone()
		{
			IDisplaySet displaySet = new DisplaySet();

			foreach (IPresentationImage image in this.PresentationImages)
				displaySet.PresentationImages.Add(image.Clone());

			return displaySet;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public void Draw()
		{
			if (this.Visible)
			{
				foreach (PresentationImage image in this.PresentationImages)
					image.Draw();
			}
		}

		internal void LinkPresentationImage(PresentationImage image)
		{
			_linkedPresentationImages.Add(image);
		}

		internal void UnlinkPresentation(PresentationImage image)
		{
			_linkedPresentationImages.Remove(image);
		}

		private void OnPresentationImageAdded(object sender, PresentationImageEventArgs e)
		{
			PresentationImage image = e.PresentationImage as PresentationImage;
			image.ParentDisplaySet = this;
			image.ImageViewer = this.ImageViewer;

			if (e.PresentationImage.Linked)
				_linkedPresentationImages.Add(e.PresentationImage);
		}

		private void OnPresentationImageRemoved(object sender, PresentationImageEventArgs e)
		{
			if (e.PresentationImage.Linked)
				_linkedPresentationImages.Remove(e.PresentationImage);
		}
	}
}
