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

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Specifies the <see cref="GraphicAnnotationSerializer"/> to use when serializing a DICOM graphic annotation (DICOM PS 3.3 C.10.5)
	/// </summary>
	/// <remarks>
	/// Only one attribute may be specified on any given class. Attributes decorating base classes are inherited (and can be overriden)
	/// by the derived class.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DicomSerializableGraphicAnnotationAttribute : Attribute
	{
		private readonly Type _serializerType;
		private GraphicAnnotationSerializer _serializer;

		/// <summary>
		/// Constructs a new <see cref="DicomSerializableGraphicAnnotationAttribute"/>.
		/// </summary>
		/// <param name="serializerType">The concrete implementation of <see cref="GraphicAnnotationSerializer"/> to use.</param>
		public DicomSerializableGraphicAnnotationAttribute(Type serializerType)
		{
			if (!typeof (GraphicAnnotationSerializer).IsAssignableFrom(serializerType))
				throw new ArgumentException("Serializer type must derive from GraphicAnnotationSerializer.", "serializerType");
			if (serializerType.IsAbstract)
				throw new ArgumentException("Serializer type must not be abstract.", "serializerType");
			if (serializerType.GetConstructor(Type.EmptyTypes) == null)
				throw new ArgumentException("Serializer type must have a public, parameter-less constructor.", "serializerType");

			_serializerType = serializerType;
		}

		/// <summary>
		/// Gets an instance of the specified serializer.
		/// </summary>
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

	/// <summary>
	/// Base class for a state-less class that serializes <see cref="IGraphic"/>s into <see cref="GraphicAnnotationSequenceItem"/>s according to DICOM PS 3.3 C.10.5.
	/// </summary>
	/// <remarks>
	/// Concrete implementations of this class must have a public, parameter-less constructor.
	/// </remarks>
	public abstract class GraphicAnnotationSerializer
	{
		/// <summary>
		/// Serializes the specified graphic to the supplied serialization state object.
		/// </summary>
		/// <param name="graphic">The graphic to serialize.</param>
		/// <param name="serializationState">The state to which the graphic should be serialized.</param>
		protected abstract void Serialize(IGraphic graphic, GraphicAnnotationSequenceItem serializationState);

		/// <summary>
		/// Helper method to serialize a graphic to the supplied serialization state object.
		/// </summary>
		/// <param name="graphic">The graphic to serialize.</param>
		/// <param name="serializationState">The state to which the graphic should be serialized.</param>
		/// <returns>True if the graphic was serializable; False otherwise.</returns>
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

	/// <summary>
	/// Typed class for a state-less class that serializes a particular type of <see cref="IGraphic"/>s according to DICOM PS 3.3 C.10.5.
	/// </summary>
	/// <remarks>
	/// Concrete implementations of this class must have a public, parameter-less constructor.
	/// </remarks>
	/// <typeparam name="T">The type of <see cref="IGraphic"/> that the serializer supports.</typeparam>
	public abstract class GraphicAnnotationSerializer<T> : GraphicAnnotationSerializer where T : IGraphic
	{
		/// <summary>
		/// Serializes the specified graphic to the supplied serialization state object.
		/// </summary>
		/// <param name="graphic">The graphic to serialize.</param>
		/// <param name="serializationState">The state to which the graphic should be serialized.</param>
		protected abstract void Serialize(T graphic, GraphicAnnotationSequenceItem serializationState);

		/// <summary>
		/// Serializes the specified graphic to the supplied serialization state object.
		/// </summary>
		/// <param name="graphic">The graphic to serialize.</param>
		/// <param name="serializationState">The state to which the graphic should be serialized.</param>
		protected override sealed void Serialize(IGraphic graphic, GraphicAnnotationSequenceItem serializationState)
		{
			this.Serialize((T) graphic, serializationState);
		}
	}
}