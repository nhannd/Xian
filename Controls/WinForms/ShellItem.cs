using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ClearCanvas.Controls.WinForms
{
	// ReSharper disable RedundantAssignment
	// (it only appears redundant to Resharper, but we're dealing with P/Invoke here)
	internal class ShellItem : IDisposable, ICloneable
	{
#if DEBUG
		private static int _instanceCount = 0;

		internal static int InstanceCount
		{
			get { return _instanceCount; }
		}
#endif

		private ShellItem _rootItem = null;
		private Native.IShellFolder _shellFolder = null;
		private Pidl _pidl = null;
		private string _displayName = string.Empty;
		private string _typeName = string.Empty;
		private bool _hasSubFolders = false;
		private bool _isVirtual = false;
		private bool _isFolder = false;
		private int _iconIndex = -1;

		public ShellItem()
		{
			_rootItem = this;
			try
			{
				// create a PIDL for the Desktop shell item.
				_pidl = new Pidl(Environment.SpecialFolder.Desktop);

				const Native.SHGFI FLAGS = Native.SHGFI.SHGFI_PIDL | Native.SHGFI.SHGFI_DISPLAYNAME | Native.SHGFI.SHGFI_SYSICONINDEX;
				Native.SHFILEINFO shInfo = new Native.SHFILEINFO();
				Native.Shell32.SHGetFileInfo((IntPtr) _pidl, 0, out shInfo, (uint) Marshal.SizeOf(shInfo), FLAGS);

				// get the root IShellFolder interface
				int hResult = Native.Shell32.SHGetDesktopFolder(ref _shellFolder);
				if (hResult != 0)
					Marshal.ThrowExceptionForHR(hResult);

				_displayName = shInfo.szDisplayName;
				_typeName = string.Empty;
				_iconIndex = shInfo.iIcon;
				_isFolder = true;
				_isVirtual = true;
				_hasSubFolders = true;
			}
			catch (Exception ex)
			{
				// if an exception happens during construction, we must release the PIDL now (remember, it's a pointer!)
				if (_pidl != null)
					_pidl.Dispose();
				throw new Exception("Creation of the root namespace shell item failed.", ex);
			}

#if DEBUG
			_instanceCount++;
#endif
		}

		public ShellItem(Pidl pidl, ShellItem parentShellItem) : this(pidl, parentShellItem, true) {}

		public ShellItem(Pidl pidl, ShellItem parentShellItem, bool relativePidl)
		{
			int hResult;

			_rootItem = parentShellItem._rootItem;
			try
			{
				IntPtr tempPidl;
				if (relativePidl)
				{
					_pidl = new Pidl(parentShellItem.Pidl, pidl);
					tempPidl = (IntPtr) pidl; // use the relative one from parameters
				}
				else
				{
					_pidl = pidl.Clone();
					tempPidl = (IntPtr) _pidl; // use the absolute one that we constructed just now
				}

				const Native.SHGFI FLAGS = Native.SHGFI.SHGFI_PIDL // indicates that we're specifying the item by PIDL
				                           | Native.SHGFI.SHGFI_DISPLAYNAME // indicates that we want the item's display name
				                           | Native.SHGFI.SHGFI_SYSICONINDEX // indicates that we want the item's icon's index in the system image list
				                           | Native.SHGFI.SHGFI_ATTRIBUTES // indicates that we want the item's attributes
				                           | Native.SHGFI.SHGFI_TYPENAME; // indicates that we want the item's type name
				const Native.SFGAO REQUEST_ATTRIBUTES = Native.SFGAO.SFGAO_FOLDER | Native.SFGAO.SFGAO_HASSUBFOLDER | Native.SFGAO.SFGAO_FILESYSTEM;
				Native.SHFILEINFO shInfo = new Native.SHFILEINFO();
				Native.Shell32.SHGetFileInfo((IntPtr) _pidl, (uint) REQUEST_ATTRIBUTES, out shInfo, (uint) Marshal.SizeOf(shInfo), FLAGS);

				// read item attributes
				Native.SFGAO attributeFlags = (Native.SFGAO) shInfo.dwAttributes;

				// create the item's IShellFolder interface
				if ((attributeFlags & Native.SFGAO.SFGAO_FOLDER) != 0)
				{
					Guid iidIShellFolder = Native.IID_IShellFolder;
					if (_pidl == _rootItem._pidl)
					{
						// if the requested PIDL is the root namespace (the desktop) we can't use the the BindToObject method, so get it directly
						hResult = Native.Shell32.SHGetDesktopFolder(ref _shellFolder);
					}
					else
					{
						if (relativePidl)
							hResult = (int) parentShellItem._shellFolder.BindToObject(tempPidl, IntPtr.Zero, ref iidIShellFolder, out _shellFolder);
						else
							hResult = (int) _rootItem._shellFolder.BindToObject(tempPidl, IntPtr.Zero, ref iidIShellFolder, out _shellFolder);
					}

					if (hResult != 0)
					{
						// some objects are marked as folders, but really aren't and thus cannot be bound to an IShellFolder
						// log these events for future study, but it's not exactly something to be concerned about in isolated cases.
						// Marshal.ThrowExceptionForHR(hResult);
						if ((attributeFlags & Native.SFGAO.SFGAO_HASSUBFOLDER) == 0)
							attributeFlags = attributeFlags & ~Native.SFGAO.SFGAO_FOLDER;
					}
				}

				_displayName = shInfo.szDisplayName;
				_typeName = shInfo.szTypeName;
				_iconIndex = shInfo.iIcon;
				_isFolder = (attributeFlags & Native.SFGAO.SFGAO_FOLDER) != 0;
				_isVirtual = (attributeFlags & Native.SFGAO.SFGAO_FILESYSTEM) == 0;
				_hasSubFolders = (attributeFlags & Native.SFGAO.SFGAO_HASSUBFOLDER) != 0;
			}
			catch (Exception ex)
			{
				// if an exception happens during construction, we must release the PIDL now (remember, it's a pointer!)
				if (_pidl != null)
					_pidl.Dispose();
				throw new Exception("Creation of the specified shell item failed.", ex);
			}

#if DEBUG
			_instanceCount++;
#endif
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
#if DEBUG
				--_instanceCount;
#endif

				if (_shellFolder != null)
				{
					// release the IShellFolder interface of this shell item
					Marshal.ReleaseComObject(_shellFolder);
					_shellFolder = null;
				}

				if (_pidl != null)
				{
					_pidl.Dispose();
					_pidl = null;
				}

				if (_rootItem != null)
				{
					// do not dispose this - we don't own it!
					_rootItem = null;
				}
			}
		}

		public ShellItem Clone()
		{
			return new ShellItem(_pidl, _rootItem, false);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public IEnumerable<ShellItem> EnumerateSubfolders()
		{
			return this.EnumerateSubfolders(true);
		}

		public IEnumerable<ShellItem> EnumerateSubfolders(bool includeHiddenItems)
		{
			Native.SHCONTF flags = Native.SHCONTF.SHCONTF_FOLDERS;
			if (includeHiddenItems)
				flags |= Native.SHCONTF.SHCONTF_INCLUDEHIDDEN;
			return this.EnumerateChildren(flags);
		}

		public IEnumerable<ShellItem> EnumerateFiles()
		{
			return this.EnumerateFiles(true);
		}

		public IEnumerable<ShellItem> EnumerateFiles(bool includeHiddenItems)
		{
			Native.SHCONTF flags = Native.SHCONTF.SHCONTF_NONFOLDERS;
			if (includeHiddenItems)
				flags |= Native.SHCONTF.SHCONTF_INCLUDEHIDDEN;
			return this.EnumerateChildren(flags);
		}

		public IEnumerable<ShellItem> EnumerateChildren()
		{
			return this.EnumerateChildren(true);
		}

		public IEnumerable<ShellItem> EnumerateChildren(bool includeHiddenItems)
		{
			Native.SHCONTF flags = Native.SHCONTF.SHCONTF_NONFOLDERS | Native.SHCONTF.SHCONTF_FOLDERS;
			if (includeHiddenItems)
				flags |= Native.SHCONTF.SHCONTF_INCLUDEHIDDEN;
			return this.EnumerateChildren(flags);
		}

		private IEnumerable<ShellItem> EnumerateChildren(Native.SHCONTF flags)
		{
			if (!_isFolder)
				throw new InvalidOperationException("Children can only be enumerated on a folder-type item.");
			if (_shellFolder == null)
				return new ShellItem[0];

			// Get the IEnumIDList interface pointer.
			Native.IEnumIDList pEnum = null;
			uint hRes = _shellFolder.EnumObjects(IntPtr.Zero, flags, out pEnum);
			if (hRes != 0)
				throw new Exception("IShellFolder::EnumObjects failed to enumerate child objects.", Marshal.GetExceptionForHR((int) hRes));

			List<ShellItem> children = new List<ShellItem>();

			try
			{
				IntPtr pidl = IntPtr.Zero;
				int count = 0;

				// Grab the first enumeration.
				pEnum.Next(1, out pidl, out count);

				// Then continue with all the rest.
				while (!IntPtr.Zero.Equals(pidl) && count == 1)
				{
					// Create the new ShellItem object.
					using (Pidl safePidl = new Pidl(pidl))
					{
						children.Add(new ShellItem(safePidl, this));
					}

					// Free the PIDL and reset counters.
					Marshal.FreeCoTaskMem(pidl);
					pidl = IntPtr.Zero;
					count = 0;

					// Grab the next item.
					pEnum.Next(1, out pidl, out count);
				}
			}
			finally
			{
				// Free the interface pointer.
				Marshal.ReleaseComObject(pEnum);
			}

			return children;
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public string TypeName
		{
			get { return _typeName; }
		}

		public Pidl Pidl
		{
			get { return _pidl; }
		}

		public int IconIndex
		{
			get { return _iconIndex; }
		}

		public bool IsFolder
		{
			get { return _isFolder; }
		}

		public bool IsVirtual
		{
			get { return _isVirtual; }
		}

		public bool HasSubFolders
		{
			get { return _hasSubFolders; }
		}

		public string Path
		{
			get { return _pidl.Path; }
		}

		public ShellItem Root
		{
			get { return _rootItem; }
		}
	}

	// ReSharper restore RedundantAssignment
}