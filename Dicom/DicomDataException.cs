#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Dicom
{
	[Serializable]
    public class DicomDataException : DicomException
    {
        public DicomDataException(String desc)
            : base(desc)
        {
        }
    }
}
