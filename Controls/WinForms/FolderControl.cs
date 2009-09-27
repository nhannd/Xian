using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ClearCanvas.Controls.WinForms
{
	public abstract class FolderControl : Control, IFolderCoordinatee, INotifyPropertyChanged
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private event EventHandler _pidlChanged;
		private FolderCoordinator _folderCoordinator = null;

		protected FolderControl() {}

		#region Designer Properties

		#region FolderCoordinator

		public FolderCoordinator FolderCoordinator
		{
			get { return _folderCoordinator; }
			set
			{
				if (_folderCoordinator != value)
				{
					if (_folderCoordinator != null)
						_folderCoordinator.UnregisterCoordinatee(this);

					_folderCoordinator = value;

					if (_folderCoordinator != null)
						_folderCoordinator.RegisterCoordinatee(this);

					this.OnPropertyChanged(new PropertyChangedEventArgs("FolderCoordinator"));
				}
			}
		}

		private void ResetFolderCoordinator()
		{
			this.FolderCoordinator = null;
		}

		private bool ShouldSerializeFolderCoordinator()
		{
			return this.FolderCoordinator != null;
		}

		#endregion

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (_propertyChanged != null)
				_propertyChanged.Invoke(this, e);
		}

		protected virtual void OnCurrentPidlChanged(EventArgs e) {}

		#endregion

		#region IFolderCoordinatee Members

		Pidl IFolderCoordinatee.Pidl
		{
			get { return this.CurrentPidlCore; }
		}

		event EventHandler IFolderCoordinatee.PidlChanged
		{
			add { _pidlChanged += value; }
			remove { _pidlChanged -= value; }
		}

		void IFolderCoordinatee.BrowseTo(Pidl pidl)
		{
			if (this.CurrentPidlCore != pidl)
			{
				this.BrowseToCore(pidl);
				this.OnCurrentPidlChanged(EventArgs.Empty);
			}
		}

		#endregion

		protected Pidl CurrentPidl
		{
			get { return this.CurrentPidlCore; }
			set
			{
				if (this.CurrentPidlCore != value)
				{
					this.BrowseToCore(value);
					this.OnCurrentPidlChanged(EventArgs.Empty);
					this.NotifyCoordinatorPidlChanged();
				}
			}
		}

		protected abstract Pidl CurrentPidlCore { get; }

		protected abstract void BrowseToCore(Pidl pidl);

		public abstract void Reload();

		protected void NotifyCoordinatorPidlChanged()
		{
			if (_pidlChanged != null)
				_pidlChanged.Invoke(this, EventArgs.Empty);
		}

		protected virtual void HandleBrowseException(Exception exception)
		{
			Form parentForm = this.FindForm();
			if (parentForm != null)
				MessageBox.Show(parentForm, exception.Message, parentForm.Text);
			else
				MessageBox.Show(exception.Message, this.GetType().Name);
		}

		protected virtual void HandleInitializationException(Exception exception) {}

		/// <summary>
		/// Calls <see cref="IDisposable.Dispose"/> on each item separately.
		/// </summary>
		/// <param name="enumerable">An <see cref="IEnumerable"/> of <see cref="IDisposable"/> elements.</param>
		internal static void DisposeEach(IEnumerable enumerable)
		{
			foreach (IDisposable item in enumerable)
				item.Dispose();
		}
	}
}