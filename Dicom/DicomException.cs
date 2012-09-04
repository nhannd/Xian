#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Dicom
{
    [Serializable]
    public class DicomException : Exception
    {
        public DicomException(){}

        public DicomException(String desc)
            : base(desc)
        {
        }
        public DicomException(String desc, Exception e)
            : base(desc,e)
        {
        }
        protected DicomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
