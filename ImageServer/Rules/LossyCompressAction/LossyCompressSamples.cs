using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.LossyCompressAction
{
	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class LossyCompressSample : ISampleRule
	{
		private readonly IList<ServerRuleApplyTimeEnum> _applyTime = new List<ServerRuleApplyTimeEnum>();

		public LossyCompressSample()
		{
			_applyTime.Add(ServerRuleApplyTimeEnum.GetEnum("StudyProcessed"));
		}
		public string Name
		{
			get { return "LossyCompressExempt"; }
		}
		public string Description
		{
			get { return "Lossy Compress Exempt Rule"; }
		}

		public ServerRuleTypeEnum Type
		{
			get { return ServerRuleTypeEnum.GetEnum("LossyCompressStudy"); }
		}

		public IList<ServerRuleApplyTimeEnum> ApplyTimeList
		{
			get { return _applyTime; }
		}

		public XmlDocument Rule
		{
			get
			{
				XmlDocument doc = new XmlDocument();
				XmlNode node = doc.CreateElement("rule");
				doc.AppendChild(node);
				XmlElement conditionNode = doc.CreateElement("condition");
				node.AppendChild(conditionNode);
				conditionNode.SetAttribute("expressionLanguage", "dicom");
				XmlNode actionNode = doc.CreateElement("action");
				node.AppendChild(actionNode);

				XmlElement andNode = doc.CreateElement("or");
				conditionNode.AppendChild(andNode);
				XmlElement equalNode = doc.CreateElement("equal");
				equalNode.SetAttribute("test", "$Modality");
				equalNode.SetAttribute("refValue", "MG");
				andNode.AppendChild(equalNode);
				equalNode = doc.CreateElement("equal");
				equalNode.SetAttribute("test", "$TransferSyntaxUid");
				equalNode.SetAttribute("refValue", "1.2.840.10008.1.2.4.50");
				andNode.AppendChild(equalNode);
				equalNode = doc.CreateElement("equal");
				equalNode.SetAttribute("test", "$TransferSyntaxUid");
				equalNode.SetAttribute("refValue", "1.2.840.10008.1.2.4.51");
				andNode.AppendChild(equalNode);
				equalNode = doc.CreateElement("equal");
				equalNode.SetAttribute("test", "$TransferSyntaxUid");
				equalNode.SetAttribute("refValue", "1.2.840.10008.1.2.4.91");
				andNode.AppendChild(equalNode);

				XmlElement losslessCompress = doc.CreateElement("no-op");
				actionNode.AppendChild(losslessCompress);
				return doc;
			}
		}
	}
}
