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