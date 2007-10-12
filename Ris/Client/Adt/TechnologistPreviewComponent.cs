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

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.PreviewService;
using ClearCanvas.Ris.Application.Common.Jsml;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class TechnologistPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface ITechnologistPreviewToolContext : IToolContext
    {
        ModalityWorklistItem WorklistItem { get; }
        IDesktopWindow DesktopWindow { get; }
    }

    public class TechnologistPreviewComponent : DHtmlComponent
    {
        class TechnologistPreviewToolContext : ToolContext, ITechnologistPreviewToolContext
        {
            private TechnologistPreviewComponent _component;

            public TechnologistPreviewToolContext(TechnologistPreviewComponent component)
            {
                _component = component;
            }

            #region ITechnologistPreviewToolContext Members

            public ModalityWorklistItem WorklistItem
            {
                get { return _component.WorklistItem; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            #endregion
        }

        private ModalityWorklistItem _worklistItem;
        private ToolSet _toolSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public TechnologistPreviewComponent()
        {
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new TechnologistPreviewToolExtensionPoint(), new TechnologistPreviewToolContext(this));

            SetUrl(TechnologistPreviewComponentSettings.Default.DetailsPageUrl);

            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        #region Public Properties

        public ModalityWorklistItem WorklistItem
        {
            get { return _worklistItem; }
            set
            {
                _worklistItem = value;
                NotifyAllPropertiesChanged();
            }
        }


        #endregion

        protected override ActionModelNode GetActionModel()
        {
            return ActionModelRoot.CreateModel(this.GetType().FullName, "TechnologistPreview-menu", _toolSet.Actions);
        }

        protected override object GetWorklistItem()
        {
            return _worklistItem;
        }
    }
}
