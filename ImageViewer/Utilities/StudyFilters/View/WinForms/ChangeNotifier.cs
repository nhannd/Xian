namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	internal class ChangeNotifier<T> where T : class
	{
		public delegate void ValueChangeEventHandler(T oldValue, T newValue);

		public event ValueChangeEventHandler BeforeValueChange;
		public event ValueChangeEventHandler AfterValueChange;

		public ChangeNotifier() {}

		public ChangeNotifier(ValueChangeEventHandler beforeValueChangeEventHandler, ValueChangeEventHandler afterValueChangeEventHandler)
		{
			this.BeforeValueChange += beforeValueChangeEventHandler;
			this.AfterValueChange += afterValueChangeEventHandler;
		}

		private T _value;

		public T Value
		{
			get { return _value; }
			set
			{
				if (_value != value)
				{
					T oldValue = _value;
					T newValue = value;

					if (this.BeforeValueChange != null)
						this.BeforeValueChange(oldValue, newValue);

					_value = value;

					if (this.AfterValueChange != null)
						this.AfterValueChange(oldValue, newValue);
				}
			}
		}
	}
}