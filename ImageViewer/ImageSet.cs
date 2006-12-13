using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class ImageSet : IImageSet
	{
		#region Private fields

		private DisplaySetCollection _displaySets = new DisplaySetCollection();
		private List<IDisplaySet> _linkedDisplaySets = new List<IDisplaySet>();
		private LogicalWorkspace _parentLogicalWorkspace;
		private IImageViewer _imageViewer;
		private string _name;
		private object _tag;

		#endregion

		public ImageSet()
		{
			_displaySets.ItemAdded += new EventHandler<DisplaySetEventArgs>(OnDisplaySetAdded);
			_displaySets.ItemRemoved += new EventHandler<DisplaySetEventArgs>(OnDisplaySetRemoved);
		}

		#region IImageSet Members

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

		public ILogicalWorkspace ParentLogicalWorkspace
		{
			get { return _parentLogicalWorkspace as ILogicalWorkspace; }
			internal set { _parentLogicalWorkspace = value as LogicalWorkspace; }
		}

		public DisplaySetCollection DisplaySets
		{
			get { return _displaySets; }
		}

		public ReadOnlyCollection<IDisplaySet> LinkedDisplaySets
		{
			get { return _linkedDisplaySets.AsReadOnly(); }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		public void Draw()
		{
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
	}
}
