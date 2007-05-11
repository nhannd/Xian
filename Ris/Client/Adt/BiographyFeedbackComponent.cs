using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
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
        private List<FeedbackDetail> _feedbackList;
        private BiographyFeedbackTable _feedbackTable;
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

        public override void Stop()
        {
            base.Stop();
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
