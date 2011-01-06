#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Web.Common;

namespace ClearCanvas.ImageViewer.Web.Common.Entities
{
    /// <summary>
    /// Enumeration for different standard icon sizes.
    /// </summary>
    [DataContract(Namespace = ViewerNamespace.Value)]
    public enum WebIconSize
    {
        /// <summary>
        /// Small icon.
        /// </summary>
        [EnumMember]
        Small,

        /// <summary>
        /// Medium icon.
        /// </summary>
        [EnumMember]
        Medium,

        /// <summary>
        /// Large icon.
        /// </summary>
        [EnumMember]
        Large
    };

	[DataContract(Namespace = ViewerNamespace.Value)]
	public class WebIconSet
	{
		[DataMember(IsRequired = false)]
		public byte[] SmallIcon { get; set; }

		[DataMember(IsRequired = false)]
		public byte[] MediumIcon { get; set; }

		[DataMember(IsRequired = false)]
		public byte[] LargeIcon { get; set; }

        [DataMember(IsRequired = false)]
        public bool HasOverlay { get; set; }
	}

    [DataContract(Namespace = ViewerNamespace.Value)]
    public class WebActionNode : Entity
    {
        [DataMember(IsRequired = true)]
        public string LocalizedText { get; set; }

        [DataMember(IsRequired = false)]
        public WebActionNode[] Children { get; set; }
    }

	[DataContract(Namespace = ViewerNamespace.Value)]
	public class WebAction : WebActionNode
	{
		public WebAction()
		{
		}

		[DataMember(IsRequired = true)]
		public bool Visible { get; set; }

		[DataMember(IsRequired = true)]
		public bool Enabled { get; set; }

		[DataMember(IsRequired = true)]
		public string ToolTip { get; set; }

		[DataMember(IsRequired = false)]
		public string Label { get; set; }

		[DataMember(IsRequired = false)]
		public WebIconSet IconSet { get; set; }

		public override string ToString()
		{
			return string.Format("{0} [{1}]", base.ToString(), Label);
		}
	}

	[DataContract(Namespace = ViewerNamespace.Value)]
	public class WebClickAction : WebAction
	{
		public WebClickAction()
		{
		}

		[DataMember(IsRequired = true)]
		public bool IsCheckAction { get; set; }

		[DataMember(IsRequired = true)]
		public bool Checked { get; set; }
	}

	[DataContract(Namespace = ViewerNamespace.Value)]
	public class WebDropDownButtonAction : WebClickAction
	{
		public WebDropDownButtonAction()
		{}

		[DataMember(IsRequired = false)]
		public WebActionNode[] DropDownActions { get; set; }		
	}

    [DataContract(Namespace = ViewerNamespace.Value)]
    public class WebDropDownAction : WebAction
    {
        public WebDropDownAction()
        { }

        [DataMember(IsRequired = false)]
        public WebActionNode[] DropDownActions { get; set; }
    }

    [DataContract(Namespace = ViewerNamespace.Value)]
    public class WebLayoutChangerAction : WebAction
    {
        public WebLayoutChangerAction()
        { }

        [DataMember(IsRequired = true)]
        public int MaxRows { get; set; }

        [DataMember(IsRequired = true)]
        public int MaxColumns { get; set; }

        [DataMember(IsRequired = true)]
        public string ActionID { get; set; }

		public override string ToString()
		{
			return string.Format("{0} [{1}]", base.ToString(), ActionID);
		}
	}
}