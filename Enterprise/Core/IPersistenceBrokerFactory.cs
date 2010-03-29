using System;

namespace ClearCanvas.Enterprise.Core
{
	public interface IPersistenceBrokerFactory
	{
		/// <summary>
		/// Returns a broker that implements the specified interface to retrieve data into this persistence context.
		/// </summary>
		/// <typeparam name="TBrokerInterface">The interface of the broker to obtain</typeparam>
		/// <returns></returns>
		TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker;

		/// <summary>
		/// Returns a broker that implements the specified interface to retrieve data into this persistence context.
		/// </summary>
		/// <param name="brokerInterface"></param>
		/// <returns></returns>
		object GetBroker(Type brokerInterface);
	}
}
