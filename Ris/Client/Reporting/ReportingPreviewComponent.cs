#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    [ExtensionPoint]
    public class ReportingPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IReportingPreviewToolContext : IToolContext
    {
        ReportingWorklistItem WorklistItem { get; }
        IDesktopWindow DesktopWindow { get; }
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="ReportingPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ReportingPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ReportingPreviewComponent class
    /// </summary>
    [AssociateView(typeof(ReportingPreviewComponentViewExtensionPoint))]
    public class ReportingPreviewComponent : DHtmlComponent
    {
        class ReportingPreviewToolContext : ToolContext, IReportingPreviewToolContext
        {
            private readonly ReportingPreviewComponent _component;

            public ReportingPreviewToolContext(ReportingPreviewComponent component)
            {
                _component = component;
            }

            public ReportingWorklistItem WorklistItem
            {
                get { return _component.WorklistItem; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private ReportingWorklistItem _worklistItem;
        private ToolSet _toolSet;

        public ReportingWorklistItem WorklistItem
        {
            get { return _worklistItem; }
            set
            {
                _worklistItem = value;
                NotifyAllPropertiesChanged();
            }
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new ReportingPreviewToolExtensionPoint(), new ReportingPreviewToolContext(this));

            SetUrl(ReportingPreviewComponentSettings.Default.DetailsPageUrl);

            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        #region Presentation Model

        protected override ActionModelNode GetActionModel()
        {
            return ActionModelRoot.CreateModel(this.GetType().FullName, "ReportingPreview-menu", _toolSet.Actions);
        }

        protected override object GetWorklistItem()
        {
            return _worklistItem;
        }

        #endregion
    }
}
