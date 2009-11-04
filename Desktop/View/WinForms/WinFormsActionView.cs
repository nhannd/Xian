using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
	public abstract class WinFormsActionView : WinFormsView, IActionView
	{
		private IActionViewContext _context;

		protected WinFormsActionView()
		{
		}

		protected IActionViewContext Context
		{
			get { return _context; }	
		}

		protected virtual void OnSetContext()
		{ }

		#region IActionView Members

		IActionViewContext IActionView.Context
		{
			get { return Context; }
			set
			{
				_context = value;
				OnSetContext();
			}
		}

		#endregion
	}

	internal class StandardWinFormsActionView : WinFormsActionView
	{
		private delegate object CreateGuiElementDelegate(IActionViewContext context);

		private object _guiElement;
		private readonly CreateGuiElementDelegate _createGuiElement;

		private StandardWinFormsActionView(CreateGuiElementDelegate createGuiElement)
		{
			_createGuiElement = createGuiElement;
		}

		public override object GuiElement
		{
			get
			{
				if (_guiElement == null)
					_guiElement = _createGuiElement(base.Context);

				return _guiElement;
			}
		}

		public static IActionView CreateDropDownButtonActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					DropDownButtonToolbarItem item = new DropDownButtonToolbarItem((IClickAction)context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}

		public static IActionView CreateDropDownActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					DropDownToolbarItem item = new DropDownToolbarItem((IDropDownAction) context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}

		public static IActionView CreateButtonActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					ActiveToolbarButton item = new ActiveToolbarButton((IClickAction) context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}

		public  static IActionView CreateMenuActionView()
		{
			return new StandardWinFormsActionView(
				delegate(IActionViewContext context)
				{
					ActiveMenuItem item = new ActiveMenuItem((IClickAction)context.Action, context.IconSize);
					context.IconSizeChanged += delegate { item.IconSize = context.IconSize; };
					return item;
				});
		}
	}
}