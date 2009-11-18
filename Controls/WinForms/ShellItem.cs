#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
		private bool _disposed = false;

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

		~ShellItem()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
#if DEBUG
				--_instanceCount;
#endif

				if (disposing)
				{
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

				if (_shellFolder != null)
				{
					// release the IShellFolder interface of this shell item
					Marshal.ReleaseComObject(_shellFolder);
					_shellFolder = null;
				}

				_disposed = true;
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

		public IEnumerable<ShellItem> EnumerateChildren()
		{
			return this.EnumerateChildren(ChildType.Files | ChildType.Folders, true);
		}

		public IEnumerable<ShellItem> EnumerateChildren(bool includeHiddenItems)
		{
			return this.EnumerateChildren(ChildType.Files | ChildType.Folders, includeHiddenItems);
		}

		public IEnumerable<ShellItem> EnumerateChildren(ChildType types)
		{
			return this.EnumerateChildren(types, true);
		}

		public IEnumerable<ShellItem> EnumerateChildren(ChildType types, bool includeHiddenItems)
		{
			Native.SHCONTF flags = 0;
			flags |= ((types & ChildType.Files) == ChildType.Files) ? Native.SHCONTF.SHCONTF_NONFOLDERS : 0;
			flags |= ((types & ChildType.Folders) == ChildType.Folders) ? Native.SHCONTF.SHCONTF_FOLDERS : 0;
			flags |= (includeHiddenItems) ? Native.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0;

			List<ShellItem> children = new List<ShellItem>();
			foreach (Pidl pidl in EnumerateChildPidls(flags))
			{
				children.Add(new ShellItem(pidl, this));
				pidl.Dispose();
			}
			return children;
		}

		public IEnumerable<Pidl> EnumerateChildPidls()
		{
			return this.EnumerateChildPidls(ChildType.Files | ChildType.Folders, true);
		}

		public IEnumerable<Pidl> EnumerateChildPidls(bool includeHiddenItems)
		{
			return this.EnumerateChildPidls(ChildType.Files | ChildType.Folders, includeHiddenItems);
		}

		public IEnumerable<Pidl> EnumerateChildPidls(ChildType types)
		{
			return this.EnumerateChildPidls(types, true);
		}

		public IEnumerable<Pidl> EnumerateChildPidls(ChildType types, bool includeHiddenItems)
		{
			Native.SHCONTF flags = 0;
			flags |= ((types & ChildType.Files) == ChildType.Files) ? Native.SHCONTF.SHCONTF_NONFOLDERS : 0;
			flags |= ((types & ChildType.Folders) == ChildType.Folders) ? Native.SHCONTF.SHCONTF_FOLDERS : 0;
			flags |= (includeHiddenItems) ? Native.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0;
			return this.EnumerateChildPidls(flags);
		}

		private IEnumerable<Pidl> EnumerateChildPidls(Native.SHCONTF flags)
		{
			if (!_isFolder)
				throw new InvalidOperationException("Children can only be enumerated on a folder-type item.");
			if (_shellFolder == null)
				return new Pidl[0];

			// Get the IEnumIDList interface pointer.
			Native.IEnumIDList pEnum = null;
			uint hRes = _shellFolder.EnumObjects(IntPtr.Zero, flags, out pEnum);
			if (hRes != 0)
				throw new Exception("IShellFolder::EnumObjects failed to enumerate child objects.", Marshal.GetExceptionForHR((int) hRes));

			try
			{
				return Pidl.ConvertPidlEnumeration(pEnum);
			}
			finally
			{
				// Free the interface pointer.
				Marshal.ReleaseComObject(pEnum);
			}
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

		[Flags]
		public enum ChildType
		{
			Files = 1,
			Folders = 2
		}
	}

	// ReSharper restore RedundantAssignment
}