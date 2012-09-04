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
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolNoteDetail : DataContractBase
    {
        public ProtocolNoteDetail(StaffSummary author, DateTime timeStamp, string text)
        {
            Author = author;
            TimeStamp = timeStamp;
            Text = text;
        }

        [DataMember]
        public StaffSummary Author;

        [DataMember] 
        public DateTime TimeStamp;

        [DataMember]
        public string Text;
    }
}