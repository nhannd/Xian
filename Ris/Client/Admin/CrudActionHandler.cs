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

        private ActionModelRoot _buttonModel;
        private ActionModelRoot _menuModel;
        private Dictionary<string, ObservableProperty<bool>> _enabledState;

        public CrudActionHandler()
        {
            _buttonModel = new ActionModelRoot("");
            _menuModel = new ActionModelRoot("");

            _enabledState = new Dictionary<string, ObservableProperty<bool>>();

            _enabledState.Add("Add", new ObservableProperty<bool>(this, false));
            _enabledState.Add("Edit", new ObservableProperty<bool>(this, false));
            _enabledState.Add("Delete", new ObservableProperty<bool>(this, false));

            AddAction(ActionCategory.ToolbarAction, _buttonModel, "Add", Add, "Icons.Add.png");
            AddAction(ActionCategory.ToolbarAction, _buttonModel, "Edit", Edit, "Icons.Edit.png");
            AddAction(ActionCategory.ToolbarAction, _buttonModel, "Delete", Delete, "Icons.Delete.png");

            AddAction(ActionCategory.MenuAction, _menuModel, "Add", Add, "Icons.Add.png");
            AddAction(ActionCategory.MenuAction, _menuModel, "Edit", Edit, "Icons.Edit.png");
            AddAction(ActionCategory.MenuAction, _menuModel, "Delete", Delete, "Icons.Delete.png");
        }

        public ActionModelRoot ToolbarModel
        {
            get { return _buttonModel; }
        }

        public ActionModelRoot MenuModel
        {
            get { return _menuModel; }
        }

        private void AddAction(ActionCategory category, ActionModelRoot model, string name, ClickHandlerDelegate clickHandler, string icon)
        {
            IResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
            ActionPath actionPath = new ActionPath(name, resolver);

            ClickAction action = category == ActionCategory.ToolbarAction ?
                (ClickAction)new ButtonAction(name, actionPath, ClickActionFlags.None, resolver)
                : (ClickAction)new MenuAction(name, actionPath, ClickActionFlags.None, resolver);
            
            action.Tooltip = name;
            if (category == ActionCategory.MenuAction)
            {
                action.Label = actionPath.LastSegment.LocalizedText;
            }
            if (icon != null)
            {
                action.IconSet = new IconSet(IconScheme.Colour, icon, icon, icon);
            }
            action.SetClickHandler(clickHandler);
            action.SetEnabledObservable(_enabledState[name]);

            model.InsertAction(action);
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
