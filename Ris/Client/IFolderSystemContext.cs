#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
    public interface IFolderSystemContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }

        IFolder SelectedFolder { get; set; }
    	event EventHandler SelectedFolderChanged;

        ISelection SelectedItems { get; }
		event EventHandler SelectedItemsChanged;
    	event EventHandler SelectedItemDoubleClicked;
	}
}
