using System.Configuration;
using System.Xml;

namespace ClearCanvas.Healthcare
{


	[SettingsGroupDescription("Settings that configure the behaviour of the procedure builder.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class ProcedureBuilderSettings
	{
		private XmlDocument _planXml;

		public XmlDocument RootProcedurePlan
		{
			get
			{
				if(_planXml == null)
				{
					_planXml = new XmlDocument();
					_planXml.LoadXml(this.RootProcedurePlanXml);
				}
				return _planXml;
			}
		}
	}
}
