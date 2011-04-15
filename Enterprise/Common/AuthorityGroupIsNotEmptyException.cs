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

namespace ClearCanvas.Enterprise.Common
{
    [Serializable]
    public class AuthorityGroupIsNotEmptyException : Exception
    {
        public string GroupName { get; set; }
        public int UserCount { get; set; }

        
        public AuthorityGroupIsNotEmptyException(string groupName, int userCount)
            : base(userCount == 1 ? string.Format(SR.ExceptionAuthorityGroupIsNotEmpty_OneUser, groupName) : string.Format(SR.ExceptionAuthorityGroupIsNotEmpty_MultipleUsers, groupName, userCount))
        {
            GroupName = groupName;
            UserCount = userCount;
        }

        /// <summary>
        /// Creates an instance of <see cref="AuthorityGroupIsNotEmptyException"/> from the serialization stream.
        /// This constructor is used by the client.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AuthorityGroupIsNotEmptyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // get the custom properties out of the serialization stream and 
            // set the object's properties
            GroupName = info.GetString("GroupName");
            UserCount = info.GetInt32("UserCount");
        }

        
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // add the custom properties into the serialization stream
            info.AddValue("GroupName", GroupName);
            info.AddValue("UserCount", UserCount);

            base.GetObjectData(info, context);
        }
    }
}