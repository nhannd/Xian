#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Web.Common.Events
{
    public enum EntityEventType
    {
        Created,
        Updated,
        Destroyed
    }

    [DataContract(Namespace = Namespace.Value)]
	public class EntityEvent : Event
	{
        [DataMember(IsRequired = true)]
        public EntityEventType EventType { get; set; }
        
        [DataMember(IsRequired = true)]
		public Entity Entity { get; set; }
	}
}