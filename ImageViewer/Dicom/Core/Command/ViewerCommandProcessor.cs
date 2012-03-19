#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Utilities.Command;

namespace ClearCanvas.ImageViewer.Dicom.Core.Command
{
    public class ViewerCommandProcessor : CommandProcessor
    {
        public ViewerCommandProcessor(string description) 
            : base(description, new ViewerCommandProcessorContext())
        {
        }
    }
}
