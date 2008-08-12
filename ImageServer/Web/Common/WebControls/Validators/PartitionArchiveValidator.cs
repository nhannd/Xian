using System;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    public class PartitionArchiveValidator : BaseValidator
    {
        private string _originalAeTitle = "";

        public string OriginalAeTitle
        {
            get { return _originalAeTitle;}
            set { _originalAeTitle = value; }
        }

        protected override void RegisterClientSideValidationExtensionScripts()
        { }

        protected override bool OnServerSideEvaluate()
        {
            String aeTitle = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(aeTitle))
            {
                ErrorMessage = "AE Title must be specified";
                return false;
            }

            if (OriginalAeTitle.Equals(aeTitle))
                return true;

            ServerPartitionConfigController controller = new ServerPartitionConfigController();
            ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
            criteria.AeTitle.EqualTo(aeTitle);

            IList<ServerPartition> list = controller.GetPartitions(criteria);

            if (list.Count > 0)
            {
                ErrorMessage = String.Format("AE Title '{0}' is already in use", aeTitle);
                return false;
            }

            return true;
        }
    }
}
