using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Rules
{
	public class StudyRuleDataContractAttribute : PolymorphicDataContractAttribute
	{
		public StudyRuleDataContractAttribute(string dataContractGuid)
			: base(dataContractGuid)
		{
		}
	}
}
