using System;
using System.Collections;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Summary description for PluginList.
	/// </summary>
	public class PluginList : IEnumerable
	{
		private ArrayList m_PluginList = new ArrayList();

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

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return m_PluginList.GetEnumerator();
		}

		#endregion
	}
}
