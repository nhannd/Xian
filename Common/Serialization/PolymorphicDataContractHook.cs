using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Serialization
{
	public class PolymorphicDataContractHook<T> : IJsmlSerializerHook
		where T : PolymorphicDataContractAttribute
	{
		private static readonly Dictionary<string, Type> _contractMap;

		static PolymorphicDataContractHook()
		{
			// build the contract map by finding all types having a T attribute
			_contractMap = (from p in Platform.PluginManager.Plugins
							from t in p.Assembly.GetTypes()
							let a = AttributeUtils.GetAttribute<T>(t)
							where (a != null)
							select new { a.ContractId, Contract = t })
				.ToDictionary(entry => entry.ContractId, entry => entry.Contract);
		}

		public static void RegisterKnownType(Type type)
		{
			var a = AttributeUtils.GetAttribute<T>(type);
			if(a == null)
				throw new ArgumentException(string.Format("Specified type must be decorated with {0}", typeof(T).FullName));
			_contractMap.Add(a.ContractId, type);
		}


		#region IJsmlSerializerHook

		bool IJsmlSerializerHook.Serialize(IJsmlSerializationContext context)
		{
			var data = context.Data;
			if (data != null)
			{
				// if we have an attribute, write out the contract ID as an XML attribute
				var a = AttributeUtils.GetAttribute<T>(data.GetType());
				if (a != null)
				{
					context.Attributes.Add("contract", a.ContractId);
				}
			}

			// always return false - we don't handle serialization ourselves
			return false;
		}

		bool IJsmlSerializerHook.Deserialize(IJsmlDeserializationContext context)
		{
			// if we have an XML attribute for the contract ID, change the data type to use the correct contract
			var contract = context.XmlElement.GetAttribute("contract");
			if (!string.IsNullOrEmpty(contract))
			{
				// constrain the data type by the contract id
				context.DataType = GetDataContract(contract);
			}

			// always return false - we don't handle serialization ourselves
			return false;
		}

		#endregion

		private static Type GetDataContract(string contractId)
		{
			Type contract;
			if (!_contractMap.TryGetValue(contractId, out contract))
				throw new ArgumentException("Invalid data contract ID.");

			return contract;
		}
	}
}
