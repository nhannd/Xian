using System;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// When applied to an entity class, specifies whether that class should be published in entity change-sets.
	/// </summary>
	/// <remarks>
	/// Entity classes are published in change sets by default.  Therefore this attribute need only be applied for the purpose
	/// of excluding an entity class from change-sets.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ChangeSetPublishableAttribute : Attribute
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="isPublishable"></param>
		public ChangeSetPublishableAttribute(bool isPublishable)
		{
			IsPublishable = isPublishable;
		}

		/// <summary>
		/// Gets a value indicating whether the entity is publishable.
		/// </summary>
		public bool IsPublishable { get; private set; }
	}
}
