using System;
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Summary description for PluginList.
	/// </summary>
	internal class PluginList : IEnumerable<Plugin>
	{
		private List<Plugin> m_PluginList = new List<Plugin>();

		public PluginList()
		{
		}

		public int NumberOfPlugins
		{
			get { return m_PluginList.Count; }
		}

		public Plugin this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, this.NumberOfPlugins - 1, this);
				return m_PluginList[index] as Plugin;
			}
		}

		public void AddPlugin(Plugin plugin)
		{
			Platform.CheckForNullReference(plugin, "plugin");
			m_PluginList.Add(plugin);
		}

		public void RemovePlugin(Plugin plugin)
		{
			Platform.CheckForNullReference(plugin, "plugin");
			m_PluginList.Remove(plugin);
		}

		public bool Contains(Plugin plugin)
		{
			Platform.CheckForNullReference(plugin, "plugin");
			return m_PluginList.Contains(plugin);
		}


		#region IEnumerable<Plugin> Members

		public IEnumerator<Plugin> GetEnumerator()
		{
			return m_PluginList.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_PluginList.GetEnumerator();
		}

		#endregion
	}
}
