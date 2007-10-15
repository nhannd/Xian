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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="BiographyFeedbackComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class BiographyFeedbackComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// BiographyFeedbackComponent class
    /// </summary>
    [AssociateView(typeof(BiographyFeedbackComponentViewExtensionPoint))]
    public class BiographyFeedbackComponent : ApplicationComponent
    {
        private readonly List<FeedbackDetail> _feedbackList;
        private readonly BiographyFeedbackTable _feedbackTable;
        private FeedbackDetail _selectedFeedback;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyFeedbackComponent()
        {
            _feedbackTable = new BiographyFeedbackTable();
            _feedbackList = new List<FeedbackDetail>();

            AddDummyFeedbacks();
        }

        public override void Start()
        {
            base.Start();

            _feedbackTable.Items.AddRange(_feedbackList);
        }

        #region Presentation Model

        public ITable Feedbacks
        {
            get { return _feedbackTable; }
        }

        public ISelection SelectedFeedback
        {
            get { return new Selection(_selectedFeedback); }
            set
            {
                _selectedFeedback = (FeedbackDetail)value.Item;
                FeedbackSelectionChanged();
            }
        }

        public string Subject
        {
            get { return (_selectedFeedback == null ? null : _selectedFeedback.Subject); }
        }

        public string Comments
        {
            get { return (_selectedFeedback == null ? null : _selectedFeedback.Comments); }
        }

        #endregion

        private void FeedbackSelectionChanged()
        {
            NotifyAllPropertiesChanged();
        }

        private void AddDummyFeedbacks()
        {
            _feedbackList.Add(new FeedbackDetail("Complaint", "Waiting time too long", "In a galaxy far far away..."));
            _feedbackList.Add(new FeedbackDetail("Complaint", "Lost in hospital", "Boba Fett? Boba Fett? Where? "));
            _feedbackList.Add(new FeedbackDetail("General", "ClearCanvas team rocks!", "Obi-Wan has taught you well."));
        }

    }
}
