#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    public class DeviceValidator : BaseValidator
    {
        private string _originalAeTitle = String.Empty;
        private ServerEntityKey _partition;

        public string OriginalAeTitle
        {
            get { return _originalAeTitle; }
            set { _originalAeTitle = value; }
        }

        public ServerEntityKey Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        protected override void RegisterClientSideValidationExtensionScripts()
        {
        }

        protected override bool OnServerSideEvaluate()
        {
            String deviceAE = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(deviceAE))
            {
                ErrorMessage = "AE Title must be specified";
                return false;
            }

            if (OriginalAeTitle.Equals(deviceAE))
                return true;

            var controller = new DeviceConfigurationController();
            var criteria = new DeviceSelectCriteria();
            criteria.AeTitle.EqualTo(deviceAE);
            criteria.ServerPartitionKey.EqualTo(Partition);

            IList<Device> list = controller.GetDevices(criteria);

            if (list.Count > 0)
            {
                ErrorMessage = String.Format("AE Title '{0}' is already in use", deviceAE);
                return false;
            }

            return true;
        }
    }
}
