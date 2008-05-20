using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents a single page in a <see cref="StackTabComponentContainer"/>.
	/// </summary>
	public class StackTabPage : TabPage
	{
		private string _preText;
		private string _text;
		private string _postText;
		private IconSet _iconSet;
		private IResourceResolver _resourceResolver;

		private event EventHandler _textChanged;
		private event EventHandler _iconChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the page.</param>
		/// <param name="component">The <see cref="IApplicationComponent"/> to be hosted in this page.</param>
		/// <param name="preText">The text to display on the title bar.</param>
		/// <param name="text">The text to display on the title bar.</param>
		/// <param name="postText">The text to display on the title bar.</param>
		/// <param name="iconSet">The icon to display on the title bar.</param>
		/// <param name="fallbackResolver">Resource resolver to fall back on in case the default failed to find resources.</param>
		public StackTabPage(string name, 
			IApplicationComponent component, 
			string preText, 
			string text, 
			string postText, 
			IconSet iconSet,
			IResourceResolver fallbackResolver)
			: base(name, component)
		{
			_preText = preText;
			_text = text;
			_postText = postText;
			_iconSet = iconSet;

			// establish default resource resolver on this assembly (not the assembly of the derived class)

			if (fallbackResolver == null)
				_resourceResolver = new ResourceResolver(typeof(StackTabPage).Assembly);
			else
				_resourceResolver = new ResourceResolver(typeof(StackTabPage).Assembly, fallbackResolver);
		}

		/// <summary>
		/// Set the title of this page
		/// </summary>
		/// <param name="preText">The text to display on the title bar.</param>
		/// <param name="text">The text to display on the title bar.</param>
		/// <param name="postText">The text to display on the title bar.</param>
		public void SetTitle(string preText, string text, string postText)
		{
			_preText = preText;
			_text = text;
			_postText = postText;

			EventsHelper.Fire(_textChanged, this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when the <see cref="SetTitle"/> method is called.
		/// </summary>
		public event EventHandler TextChanged
		{
			add { _textChanged += value; }
			remove { _textChanged -= value; }
		}

		/// <summary>
		/// Allows the folder to nofity that it's icon has changed
		/// </summary>
		public event EventHandler IconChanged
		{
			add { _iconChanged += value; }
			remove { _iconChanged -= value; }
		}

		/// <summary>
		/// Gets the iconset that should be displayed for the folder
		/// </summary>
		public IconSet IconSet
		{
			get { return _iconSet; }
			set 
			{
				_iconSet = value;
				EventsHelper.Fire(_iconChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets and sets the resource resolver that is used to resolve the Icon
		/// </summary>
		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
			set { _resourceResolver = value; }
		}

		/// <summary>
		/// The first part of the text to display on the title bar
		/// </summary>
		public string PreText
		{
			get { return _preText; }
		}

		/// <summary>
		/// The main part of the text to display on the title bar
		/// </summary>
		public string Text
		{
			get { return _text; }
		}

		/// <summary>
		/// The last part of the text to display on the title bar
		/// </summary>
		public string PostText
		{
			get { return _postText; }
		}
	}
}
