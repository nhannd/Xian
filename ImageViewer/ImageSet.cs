using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A container for <see cref="IDisplaySet"/> objects.
	/// </summary>
	public class ImageSet : IImageSet
	{
		#region Private fields

		private DisplaySetCollection _displaySets = new DisplaySetCollection();
		private List<IDisplaySet> _linkedDisplaySets = new List<IDisplaySet>();
		private LogicalWorkspace _parentLogicalWorkspace;
		private IImageViewer _imageViewer;
		private string _name;
		private string _patientInfo;
		private string _uid;
		private event EventHandler _drawing;

		#endregion
		
		/// <summary>
		/// Initializes a new instance of <see cref="ImageSet"/>.
		/// </summary>
		public ImageSet()
		{
			_displaySets.ItemAdded += new EventHandler<DisplaySetEventArgs>(OnDisplaySetAdded);
			_displaySets.ItemRemoved += new EventHandler<DisplaySetEventArgs>(OnDisplaySetRemoved);
		}

		#region IImageSet Members

		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="ImageSet"/> is not part of the 
		/// logical workspace yet.</value>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			internal set
			{
				_imageViewer = value;

				if (_imageViewer != null)
				{
					foreach (DisplaySet displaySet in this.DisplaySets)
						displaySet.ImageViewer = value;
				}
			}
		}

		/// <summary>
		/// Gets the parent <see cref="LogicalWorkspace"/>
		/// </summary>
		/// <value>The parent <see cref="ILogicalWorkspace"/> or <b>null</b> if the 
		/// <see cref="ImageSet"/> has not been added to an 
		/// <see cref="ILogicalWorkspace"/> yet.</value>
		public ILogicalWorkspace ParentLogicalWorkspace
		{
			get { return _parentLogicalWorkspace as ILogicalWorkspace; }
			internal set { _parentLogicalWorkspace = value as LogicalWorkspace; }
		}

		/// <summary>
		/// Gets the collection of <see cref="IDisplaySet"/> objects that belong
		/// to this <see cref="ImageSet"/>.
		/// </summary>
		public DisplaySetCollection DisplaySets
		{
			get { return _displaySets; }
		}

		/// <summary>
		/// Gets a collection of linked <see cref="IDisplaySet"/> objects.
		/// </summary>
		public ReadOnlyCollection<IDisplaySet> LinkedDisplaySets
		{
			get { return _linkedDisplaySets.AsReadOnly(); }
		}

		/// <summary>
		/// Gets or sets the name of the image set.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string PatientInfo
		{
			get { return _patientInfo; }
			set { _patientInfo = value; }
		}


		/// <summary>
		/// Gets or sets unique identifier for this <see cref="IImageSet"/>.
		/// </summary>
		public string Uid
		{
			get { return _uid; }
			set { _uid = value; }
		}

		/// <summary>
		/// Fires just before the <see cref="ImageSet"/> is actually drawn/rendered.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawing += value; }
			remove { _drawing -= value; }
		}

		/// <summary>
		/// Draws the <see cref="ImageSet"/>.
		/// </summary>
		public void Draw()
		{
			OnDrawing();
			foreach (DisplaySet displaySet in this.DisplaySets)
				displaySet.Draw();
		}

		#endregion

		public override string ToString()
		{
			return this.Name;
		}

		#region Disposal

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
				Platform.Log(LogLevel.Error, e);
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
				DisposeDisplaySets();
			}
		}

		private void DisposeDisplaySets()
		{
			if (this.DisplaySets == null)
				return;

			foreach (DisplaySet displaySet in this.DisplaySets)
				displaySet.Dispose();

			_displaySets.ItemAdded -= new EventHandler<DisplaySetEventArgs>(OnDisplaySetAdded);
			_displaySets.ItemRemoved -= new EventHandler<DisplaySetEventArgs>(OnDisplaySetRemoved);
			_displaySets = null;
		}

		#endregion

		internal void LinkDisplaySet(DisplaySet displaySet)
		{
			_linkedDisplaySets.Add(displaySet);
		}

		internal void UnlinkDisplaySet(DisplaySet displaySet)
		{
			_linkedDisplaySets.Remove(displaySet);
		}

		private void OnDisplaySetAdded(object sender, DisplaySetEventArgs e)
		{
			DisplaySet displaySet = e.DisplaySet as DisplaySet;

			displaySet.ParentImageSet = this;
			displaySet.ImageViewer = this.ImageViewer;

			if (e.DisplaySet.Linked)
				_linkedDisplaySets.Add(e.DisplaySet);
		}

		private void OnDisplaySetRemoved(object sender, DisplaySetEventArgs e)
		{
			if (e.DisplaySet.Linked)
				_linkedDisplaySets.Remove(e.DisplaySet);
		}

		private void OnDrawing()
		{
			EventsHelper.Fire(_drawing, this, EventArgs.Empty);
		}
	}
}
