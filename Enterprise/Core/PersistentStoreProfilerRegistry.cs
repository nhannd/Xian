using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
	[ExtensionPoint]
	public class PersistentStoreProfilerExtensionPoint : ExtensionPoint<IPersistentStoreProfiler>
	{
	}

	public class PersistentStoreProfilerRegistry
	{
		private static IPersistentStoreProfiler _profiler;

		public static void Initialize()
		{
			if (_profiler != null) return;
			
			_profiler = (IPersistentStoreProfiler) (new PersistentStoreProfilerExtensionPoint()).CreateExtension();

			if (_profiler == null)
			{
				Platform.Log(LogLevel.Warn, "No persistent store profiler was found; profiling will be disabled.");
				return;
			}
	
			_profiler.Initialize();
		}
	}
}