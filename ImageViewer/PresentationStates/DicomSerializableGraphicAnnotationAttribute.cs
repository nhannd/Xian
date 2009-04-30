#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	//TODO: call this DicomGraphicAnnotationSerializerAttribute to be consistent w/ the corresponding abstract class?

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DicomSerializableGraphicAnnotationAttribute : Attribute
	{
		private Type _serializerType;
		private GraphicAnnotationSerializer _serializer;

		public DicomSerializableGraphicAnnotationAttribute(Type serializerType)
		{
			if (!typeof (GraphicAnnotationSerializer).IsAssignableFrom(serializerType))
				throw new ArgumentException("Serializer type must derive from GraphicAnnotationSerializer.", "serializerType");

			_serializerType = serializerType;
		}

		public GraphicAnnotationSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					_serializer = (GraphicAnnotationSerializer) _serializerType.GetConstructor(Type.EmptyTypes).Invoke(null);
				}
				return _serializer;
			}
		}
	}

	public abstract class GraphicAnnotationSerializer
	{
		protected abstract void Serialize(IGraphic graphic, GraphicAnnotationSequenceItem serializationState);

		public static bool SerializeGraphic(IGraphic graphic, GraphicAnnotationSequenceItem serializationState)
		{
			Platform.CheckForNullReference(graphic, "graphic");
			Platform.CheckForNullReference(serializationState, "serializationState");

			object[] attributes = graphic.GetType().GetCustomAttributes(typeof (DicomSerializableGraphicAnnotationAttribute), true);
			if (attributes.Length > 0)
			{
				((DicomSerializableGraphicAnnotationAttribute) attributes[0]).Serializer.Serialize(graphic, serializationState);
				return true;
			}
			return false;
		}
	}

	public abstract class GraphicAnnotationSerializer<T> : GraphicAnnotationSerializer where T : IGraphic
	{
		protected abstract void Serialize(T graphic, GraphicAnnotationSequenceItem serializationState);

		protected override sealed void Serialize(IGraphic graphic, GraphicAnnotationSequenceItem serializationState)
		{
			this.Serialize((T) graphic, serializationState);
		}
	}
}