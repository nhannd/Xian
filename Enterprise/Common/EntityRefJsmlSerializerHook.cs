#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Serialization;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
	[ExtensionOf(typeof(JsmlSerializerHookExtensionPoint))]
	class EntityRefJsmlSerializerHook : IJsmlSerializerHook
	{
		#region Implementation of IJsmlSerializerHook

		public bool Serialize(IJsmlSerializationContext context)
		{
			if (context.Data is EntityRef)
			{
				context.XmlWriter.WriteElementString(context.Name, ((EntityRef)context.Data).Serialize());
				return true;
			}
			return false;
		}

		public bool Deserialize(IJsmlDeserializationContext context)
		{
			if (context.DataType == typeof(EntityRef))
			{
				context.Data = new EntityRef(context.XmlElement.InnerText);
				return true;
			}
			return false;
		}

		#endregion
	}
}
