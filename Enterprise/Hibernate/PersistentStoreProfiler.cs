using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using HibernatingRhinos.Profiler.Appender.StackTraces;

namespace ClearCanvas.Enterprise.Hibernate
{
	[ExtensionOf(typeof(PersistentStoreProfilerExtensionPoint))]
	public class PersistentStoreProfiler : IPersistentStoreProfiler
	{
		public void Initialize()
		{
			Platform.Log(LogLevel.Info, "Initializing NHProf...");

			NHibernateProfiler.Initialize(new NHibernateAppenderConfiguration()
				{
					// TODO: StackTraceFilters should not be set once issues with DynamicProxy2 and the stack trace are resolved
					StackTraceFilters = new IStackTraceFilter[0],
					DotNotFixDynamicProxyStackTrace = true
				});

			Platform.Log(LogLevel.Info, "NHProf initialization complete.");
		}
	}
}
