#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Web.Common;

namespace ClearCanvas.ImageViewer.Web.Common.Entities
{

    [DataContract(Namespace = ViewerNamespace.Value)]
    public enum WebAlertLevel
    {
        /// <summary>
        /// An Ok button should be shown.
        /// </summary>
        
        /// <summary>
        /// An informational alert notifies the user of an event that is not a problem.
        /// </summary>
        [EnumMember]
        Info,

        /// <summary>
        /// A warning alert notifies the user of a potentially problematic event.
        /// </summary>
        [EnumMember]
        Warning,

        /// <summary>
        /// An error alert notifies the user of a failure which will likely require some corrective action.
        /// </summary>
        [EnumMember]
        Error
    }

    [DataContract(Namespace = ViewerNamespace.Value)]
    public class Alert : Entity
    {
        /// <summary>
        /// Gets or sets the alert level.
        /// </summary>
        [DataMember(IsRequired = true)]
        public WebAlertLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the link text, if the alert has a contextual link.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string LinkText { get; set; }
        
        /// <summary>
        /// Gets or sets a value that determines whether the alert notification is dismissed upon clicking the link.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool DismissOnLinkClicked { get; set; }

        /// <summary>
        /// Gets the Icon to display with the alert.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Icon { get; set; }

        public override string ToString()
        {
            return String.Format("{0} [Message={1}]", base.ToString(), Message ?? "");
        }
    }
}
