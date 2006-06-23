using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Describes a display set.
	/// </summary>
	public class DisplaySet : IDrawable
	{
		private PresentationImageCollection _presentationImages = new PresentationImageCollection();
		private LogicalWorkspace _parentLogicalWorkspace;
		private bool _visible = false;
		private bool _selected = false;
		private bool _linked = false;
		private List<PresentationImage> _linkedPresentationImages = new List<PresentationImage>();
		private event EventHandler<LinkageChangedEventArgs> _linkageChangedEvent;

		/// <summary>
		/// Initializes a new instance of the <see cref="DisplaySet"/> class.
		/// </summary>
		public DisplaySet()
		{
			_presentationImages.ItemAdded += new EventHandler<PresentationImageEventArgs>(OnPresentationImageAdded);
			_presentationImages.ItemRemoved += new EventHandler<PresentationImageEventArgs>(OnPresentationImageRemoved);
		}

		/// <summary>
		/// Gets a collection of presentation images.
		/// </summary>
		public PresentationImageCollection PresentationImages
		{
			get { return _presentationImages; }
		}

		/// <summary>
		/// Gets a collection of linked <see cref="PresentationImages"/>
		/// </summary>
		/// <value>A collection of linked <see cref="PresentationImages"/></value>
		public ReadOnlyCollection<PresentationImage> LinkedPresentationImages
		{
			get { return _linkedPresentationImages.AsReadOnly(); }
		}

		/// <summary>
		/// Gets the parent <see cref="ImageWorkspace"/>.
		/// </summary>
		/// <value>Can be <b>null</b> if the <see cref="DisplaySet"/> has not
		/// been added to a <see cref="LogicalWorkspace"/> yet.</value>
		public ImageWorkspace ParentWorkspace
		{
			get 
			{
				if (this.ParentLogicalWorkspace == null)
					return null;

				return this.ParentLogicalWorkspace.ParentWorkspace; 
			}
		}

		/// <summary>
		/// Gets the parent <see cref="LogicalWorkspace"/>
		/// </summary>
		/// <value>Can be <b>null</b> if the <see cref="DisplaySet"/> has not
		/// been added to a <see cref="LogicalWorkspace"/> yet.</value>
		public LogicalWorkspace ParentLogicalWorkspace
		{
			get
			{
				return _parentLogicalWorkspace;
			}
			set
			{
				Platform.CheckForNullReference(value, "ParentLogicalWorkspace");
				_parentLogicalWorkspace = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="DisplaySet"/> is visible.
		/// </summary>
		public bool Visible
		{
			get { return _visible;	}
			set	{ _visible = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="DisplaySet"/> is selected.
		/// </summary>
		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (_selected != value)
				{
					_selected = value;

					if (_selected)
					{
						this.ParentWorkspace.EventBroker.OnDisplaySetSelected(
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

					EventsHelper.Fire(_linkageChangedEvent, this, new LinkageChangedEventArgs(value));
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Linked"/> property in this <see cref="DisplaySet"/>
		/// has changed.
		/// </summary>
		/// <remarks>The event handler receives an argument of type <see cref="LinkageChangedEventArgs"/>.</remarks>
		public event EventHandler<LinkageChangedEventArgs> LinkageChanged
		{
			add { _linkageChangedEvent += value; }
			remove { _linkageChangedEvent -= value; }
		}

		#region IDrawable Members

		public void Draw(bool paintNow)
		{
			foreach (PresentationImage image in this.PresentationImages)
				image.Draw(paintNow);
		}

		#endregion

		private void OnPresentationImageAdded(object sender, PresentationImageEventArgs e)
		{
			e.PresentationImage.ParentDisplaySet = this;
			e.PresentationImage.LinkageChanged += new EventHandler<LinkageChangedEventArgs>(OnPresentationImageLinkageChanged);

			if (e.PresentationImage.Linked)
				_linkedPresentationImages.Add(e.PresentationImage);
		}

		private void OnPresentationImageRemoved(object sender, PresentationImageEventArgs e)
		{
			e.PresentationImage.LinkageChanged -= new EventHandler<LinkageChangedEventArgs>(OnPresentationImageLinkageChanged);

			if (e.PresentationImage.Linked)
				_linkedPresentationImages.Remove(e.PresentationImage);
		}

		private void OnPresentationImageLinkageChanged(object sender, LinkageChangedEventArgs e)
		{
			PresentationImage presentationImage = sender as PresentationImage;
			Platform.CheckForInvalidCast(presentationImage, "sender", "PresentationImage");

			if (e.IsLinked)
				_linkedPresentationImages.Add(presentationImage);
			else
				_linkedPresentationImages.Remove(presentationImage);
		}
	}
}
