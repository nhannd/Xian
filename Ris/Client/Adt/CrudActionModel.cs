using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    public class CrudActionModel : ActionModelRoot
    {

        public class CrudAction : ClickAction
        {
            class EnabledObserver : IObservablePropertyBinding<bool>
            {
                private CrudAction _subject;

                public EnabledObserver(CrudAction subject)
                {
                    _subject = subject;
                }

                #region IObservablePropertyBinding<bool> Members

                public event EventHandler PropertyChanged
                {
                    add { _subject.EnabledChanged += value; }
                    remove { _subject.EnabledChanged -= value; }
                }

                public bool PropertyValue
                {
                    get { return _subject.Enabled; }
                    set { _subject.Enabled = value; }
                }

                #endregion
            }


            private ClickHandlerDelegate _handler;
            private bool _enabled;
            private event EventHandler _enabledChanged;

            internal CrudAction(string name, string icon, IResourceResolver resolver)
                : base(name, new ActionPath(string.Format("root/{0}", name), resolver), ClickActionFlags.None, resolver)
            {
                this.SetClickHandler(delegate()
                {
                    if (_handler != null) _handler();
                });
                this.SetEnabledObservable(new EnabledObserver(this));
                this.SetDefaultTooltip(name);
                this.SetDefaultLabel(name);
                this.IconSet = new IconSet(IconScheme.Colour, icon, icon, icon);
            }

            public ClickHandlerDelegate Handler
            {
                get { return _handler; }
                set { _handler = value; }
            }
	
            public new bool Enabled
            {
                get { return _enabled; }
                set
                {
                    if (!_enabled.Equals(value))
                    {
                        _enabled = value;
                        EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                    }
                }
            }

            public new event EventHandler EnabledChanged
            {
                add { _enabledChanged += value; }
                remove { _enabledChanged -= value; }
            }
        }


        private CrudAction _add;
        private CrudAction _edit;
        private CrudAction _delete;

        public CrudActionModel()
            :this(true, true, true)
        {
        }

        public CrudActionModel(bool add, bool edit, bool delete)
        {
            IResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
            if (add)
            {
                this.InsertAction(_add = new CrudAction("Add", "Icons.Add.png", resolver));
            }
            if (edit)
            {
                this.InsertAction(_edit = new CrudAction("Edit", "Icons.Edit.png", resolver));
            }
            if (delete)
            {
                this.InsertAction(_delete = new CrudAction("Delete", "Icons.Delete.png", resolver));
            }
        }

        public CrudAction Add
        {
            get { return _add; }
        }

        public CrudAction Edit
        {
            get { return _edit; }
        }

        public CrudAction Delete
        {
            get { return _delete; }
        }
    }
}
