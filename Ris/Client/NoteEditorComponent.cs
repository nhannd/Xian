using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class NoteEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(NoteEditorComponentViewExtensionPoint))]
    public class NoteEditorComponent : ApplicationComponent
    {
        private NoteDetail _note;
        private IList<NoteCategorySummary> _noteCategoryChoices;

        public NoteEditorComponent(NoteDetail noteDetail, List<NoteCategorySummary> noteCategoryChoices)
        {
            _note = noteDetail;
            _noteCategoryChoices = noteCategoryChoices;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public string Comment
        {
            get { return _note.Comment; }
            set 
            { 
                _note.Comment = value;
                this.Modified = true;
            }
        }

        public DateTime? ValidRangeFrom
        {
            get { return _note.ValidRangeFrom; }
            set { _note.ValidRangeFrom = value; }
        }

        public DateTime? ValidRangeUntil
        {
            get { return _note.ValidRangeUntil; }
            set { _note.ValidRangeUntil = value; }
        }

        public string Category
        {
            get 
            { 
                return _note.Category == null ? "" :
                    String.Format("Severity {0} - {1}", _note.Category.Severity.Value, _note.Category.Name); 
            }
            set
            {
                _note.Category = (value == "") ? null :
                    CollectionUtils.SelectFirst<NoteCategorySummary>(_noteCategoryChoices,
                        delegate(NoteCategorySummary category) 
                        {
                            return (String.Format("Severity {0} - {1}", category.Severity.Value, category.Name) == value); 
                        });

                SignalCategoryChanged();
                this.Modified = true;
            }
        }

        public string CategoryDescription
        {
            get { return _note.Category == null ? "" : _note.Category.Description; }
        }

        public List<string> CategoryChoices
        {
            get 
            {
                List<string> categoryStrings = new List<string>();
                categoryStrings.Add("");
                categoryStrings.AddRange(
                    CollectionUtils.Map<NoteCategorySummary, string>(
                        _noteCategoryChoices, 
                        delegate(NoteCategorySummary category) 
                        {
                            return String.Format("Severity {0} - {1}", category.Severity.Value, category.Name);
                        }));

                return categoryStrings;
            }
        }

        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion

        private void SignalCategoryChanged()
        {
            NotifyPropertyChanged("CategoryDescription");
        }
    }
}
