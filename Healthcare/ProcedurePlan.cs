#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
	public class ProcedurePlan
	{
		/// <summary>
		/// Creates an instance of a default plan for the specified procedure name and default modality.
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="defaultModality"></param>
		/// <returns></returns>
		public static ProcedurePlan CreateDefaultPlan(string procedureName, Modality defaultModality)
		{
			Platform.CheckForNullReference(procedureName, "procedureName");
			Platform.CheckForNullReference(defaultModality, "defaultModality");

			// create the default procedure plan, which has exactly one MPS referencing the default modality
			var planXml = new XmlDocument();
			var planNode = planXml.CreateElement("procedure-plan");
			planXml.AppendChild(planNode);
			planNode.SetAttribute("type", "default");	// this is the default plan

			var stepsNode = planXml.CreateElement("procedure-steps");
			planNode.AppendChild(stepsNode);
			var stepNode = planXml.CreateElement("procedure-step");
			stepsNode.AppendChild(stepNode);
			stepNode.SetAttribute("class", "ClearCanvas.Healthcare.ModalityProcedureStep");
			stepNode.SetAttribute("description", procedureName);
			stepNode.SetAttribute("modality", defaultModality.Id);

			return new ProcedurePlan(planXml);
		}

		/// <summary>
		/// Creates an instance of a procedure plan using the specified procedure as a prototype.
		/// </summary>
		/// <param name="procedure"></param>
		/// <returns></returns>
		public static ProcedurePlan CreateFromProcedure(Procedure procedure)
		{
			var builder = new ProcedureBuilder();
			var xmldoc = builder.CreatePlanFromProcedure(procedure);
			return new ProcedurePlan(xmldoc);
		}

		/// <summary>
		/// Creates an instance of the root procedure plan.
		/// </summary>
		/// <returns></returns>
		public static ProcedurePlan GetRootPlan()
		{
			return new ProcedurePlan(new ProcedureBuilderSettings().RootProcedurePlanXml);
		}

		private readonly XmlDocument _planXml;
		private bool _isDefault;
		private Modality _defaultModality;
		private bool _parsed;

		public ProcedurePlan(string planXml)
		{
			_planXml = new XmlDocument();
			if (!string.IsNullOrEmpty(planXml))
				_planXml.LoadXml(planXml);
		}

		public ProcedurePlan(XmlDocument planXml)
		{
			_planXml = planXml;
		}

		/// <summary>
		/// Gets a value indicating whether this plan is a default plan or not.
		/// </summary>
		public bool IsDefault
		{
			get
			{
				ParseOnce();
				return _isDefault;
			}
		}

		/// <summary>
		/// Gets the default modality, assuming this is a default plan (<see cref="IsDefault"/> returns true).
		/// </summary>
		/// <remarks>
		/// If <see cref="IsDefault"/> return false, then this property will always return null.
		/// </remarks>
		public Modality DefaultModality
		{
			get
			{
				ParseOnce();
				return _defaultModality;
			}
		}

		/// <summary>
		/// Gets the list of procedure step nodes in this plan.
		/// </summary>
		public IEnumerable<XmlElement> ProcedureStepNodes
		{
			get
			{
				var stepNodes = _planXml.SelectNodes("procedure-plan/procedure-steps/procedure-step");
				return stepNodes == null ? new XmlElement[0] : stepNodes.Cast<XmlElement>();
			}
		}

		/// <summary>
		/// Gets this plan as an XML document.
		/// </summary>
		/// <returns></returns>
		public XmlDocument AsXml()
		{
			// return a copy so the caller can't modify ours
			var xml = new XmlDocument();
			xml.LoadXml(_planXml.ToString());
			return xml;
		}

		/// <summary>
		/// Gets this plan as an XML string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string planXml;
			var sb = new StringBuilder();
			using (var writer = new XmlTextWriter(new StringWriter(sb)))
			{
				writer.Formatting = Formatting.Indented;
				_planXml.Save(writer);
				planXml = sb.ToString();
			}
			return planXml;
		}

		private void ParseOnce()
		{
			if (_parsed)
				return;

			var type = _planXml.DocumentElement == null ? null : _planXml.DocumentElement.GetAttribute("type");
			if (type == "default")
			{
				_isDefault = true;
				var mpsNodes = _planXml.SelectNodes("procedure-plan/procedure-steps/procedure-step[@class='ClearCanvas.Healthcare.ModalityProcedureStep']");
				var mpsNode = mpsNodes == null ? null : mpsNodes.Cast<XmlElement>().FirstOrDefault();
				if (mpsNode != null)
				{
					var where = new ModalitySearchCriteria();
					where.Id.EqualTo(mpsNode.GetAttribute("modality"));
					_defaultModality = PersistenceScope.CurrentContext.GetBroker<IModalityBroker>().FindOne(where);
				}
			}
			_parsed = true;
		}
	}
}
