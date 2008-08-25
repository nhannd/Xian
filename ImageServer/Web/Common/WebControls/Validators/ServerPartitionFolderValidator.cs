using System;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
	public class ServerPartitionFolderValidator : BaseValidator
	{
		private string _originalFolder = "";

		public string OriginalPartitionFolder
		{
			get { return _originalFolder; }
			set { _originalFolder = value; }
		}

		protected override void RegisterClientSideValidationExtensionScripts()
		{ }

		protected override bool OnServerSideEvaluate()
		{
			String partitionFolder = GetControlValidationValue(ControlToValidate);

			if (String.IsNullOrEmpty(partitionFolder))
			{
				ErrorMessage = "Folder must be specified";
				return false;
			}

			if (OriginalPartitionFolder.Equals(partitionFolder))
				return true;

			ServerPartitionConfigController controller = new ServerPartitionConfigController();
			ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
			criteria.PartitionFolder.EqualTo(partitionFolder);

			IList<ServerPartition> list = controller.GetPartitions(criteria);

			if (list.Count > 0)
			{
				ErrorMessage = String.Format("Partition Folder '{0}' is already in use", partitionFolder);
				return false;
			}

			return true;
		}
	}
}
