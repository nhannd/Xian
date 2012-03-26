using System;

namespace ClearCanvas.Common.Serialization
{
	public class PolymorphicDataContractException : Exception
	{
		public PolymorphicDataContractException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}


	/// <summary>
	/// Assigns a GUID to a class to enable robust polymorphic de/serialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public abstract class PolymorphicDataContractAttribute : Attribute
	{
		private readonly string _guidString;
		private Guid _guid;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dataContractGuid"></param>
		protected PolymorphicDataContractAttribute(string dataContractGuid)
		{
			_guidString = dataContractGuid;
		}

		/// <summary>
		/// Gets the ID that identifies the data-contract.
		/// </summary>
		public string ContractId
		{
			get
			{
				if(_guid == Guid.Empty)
				{
					try
					{
						_guid = new Guid(_guidString);
					}
					catch (FormatException e)
					{
						throw new PolymorphicDataContractException(string.Format("{0} is not a valid GUID.", _guidString), e);
					}
				}
				return _guid.ToString("N");
			}
		}
	}

}
