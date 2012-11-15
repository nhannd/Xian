#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Entities;
using ClearCanvas.Web.Common.Messages;

namespace ClearCanvas.Web.Services.View
{
    [ExtensionOf(typeof(ProgressDialogComponentViewExtensionPoint))]
    internal class ProgressDialogComponentView : WebApplicationComponentView<ProgressComponent>
    {
        private ProgressDialogComponent _component;
        private bool _cancelled;

        private int ProgressPercent
        {
            get { return Math.Min(100, _component.ProgressBar/_component.ProgressBarMaximum); }
        }

        private bool IsMarquee
        {
            get { return _component.ProgressBarStyle == ProgressBarStyle.Marquee; }
        }

        #region Overrides of EntityHandler

        public override void SetModelObject(object modelObject)
        {
            _component = (ProgressDialogComponent) modelObject;
        }

        public override void ProcessMessage(Message message)
        {
            var update = message as UpdatePropertyMessage;
            if (update == null)
                return;

            if (update.PropertyName == "Cancelled" && !_cancelled)
            {
                _component.Cancel();
                _cancelled = true;
            }
        }

        protected override void Initialize()
        {
            _component.ProgressUpdateEvent += OnProgressUpdate;
            _component.ProgressTerminateEvent += OnProgressTerminate;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)return;

            _component.ProgressUpdateEvent -= OnProgressUpdate;
            _component.ProgressTerminateEvent -= OnProgressTerminate;
        }

        private void OnProgressTerminate(object sender, EventArgs e)
        {
            NotifyEntityPropertyChanged("CancelButtonVisible", _component.ShowCancel);
            NotifyEntityPropertyChanged("ButtonText", _component.ButtonText);
            NotifyEntityPropertyChanged("Message", _component.ProgressMessage);
            NotifyEntityPropertyChanged("IsMarquee", IsMarquee);
            NotifyEntityPropertyChanged("ProgressPercent", ProgressPercent);

        }

        private void OnProgressUpdate(object sender, EventArgs e)
        {
            NotifyEntityPropertyChanged("Message", _component.ProgressMessage);
            NotifyEntityPropertyChanged("IsMarquee", IsMarquee);
            NotifyEntityPropertyChanged("ProgressPercent", ProgressPercent);
        }

        #endregion

        #region Overrides of EntityHandler<ProgressComponent>

        protected override void UpdateEntity(ProgressComponent entity)
        {
            entity.CancelButtonVisible = _component.ShowCancel;
            entity.ButtonText = _component.ButtonText;
            entity.Message = _component.ProgressMessage;
            entity.ProgressPercent = ProgressPercent;
            entity.IsMarquee = IsMarquee;
        }

        #endregion
    }
}
