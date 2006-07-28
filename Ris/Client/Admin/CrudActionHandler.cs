using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Admin
{
    public abstract class CrudActionHandler
    {
        class ObservableProperty<T> : IObservablePropertyBinding<T>
        {
            private object _owner;
            private T _value;
            private event EventHandler _valueChanged;

            internal ObservableProperty(object owner, T initialValue)
            {
                _owner = owner;
                _value = initialValue;
            }

            #region IObservablePropertyBinding<T> Members

            public event EventHandler PropertyChanged
            {
                add { _valueChanged += value; }
                remove { _valueChanged -= value; }
            }

            public T PropertyValue
            {
                get { return _value; }
                set
                {
                    if (!_value.Equals(value))
                    {
                        _value = value;
                        EventsHelper.Fire(_valueChanged, _owner, new EventArgs());
                    }
                }
            }

            #endregion
        }

        private ActionModelRoot _actionModel;
        private Dictionary<string, ObservableProperty<bool>> _enabledState;

        public CrudActionHandler()
        {
            _actionModel = new ActionModelRoot("");
            _enabledState = new Dictionary<string, ObservableProperty<bool>>();

            AddAction("Add", Add, "Icons.NewDocumentHS.png");
            AddAction("Edit", Edit, "Icons.Edit.png");
            AddAction("Delete", Delete, "Icons.Delete.png");
        }

        public ActionModelRoot ActionModel
        {
            get { return _actionModel; }
        }

        private void AddAction(string name, ClickHandlerDelegate clickHandler, string icon)
        {
            _enabledState.Add(name, new ObservableProperty<bool>(this, false));

            Path actionPath = Path.ParseAndLocalize(name, new ResourceResolver(this.GetType().Assembly));

            ButtonAction action = new ButtonAction(name, actionPath, this, ClickActionFlags.None);
            action.Tooltip = name;
            if (icon != null)
            {
                action.IconSet = new IconSet(IconScheme.Colour, icon, icon, icon);
            }
            action.SetClickHandler(clickHandler);
            action.SetEnabledObservable(_enabledState[name]);

            _actionModel.InsertAction(action);
        }

        public bool AddEnabled
        {
            get { return _enabledState["Add"].PropertyValue; }
            set { _enabledState["Add"].PropertyValue = value; }
        }

        public bool EditEnabled
        {
            get { return _enabledState["Edit"].PropertyValue; }
            set { _enabledState["Edit"].PropertyValue = value; }
        }
        public bool DeleteEnabled
        {
            get { return _enabledState["Delete"].PropertyValue; }
            set { _enabledState["Delete"].PropertyValue = value; }
        }


        protected abstract void Add();
        protected abstract void Edit();
        protected abstract void Delete();
    }
}
