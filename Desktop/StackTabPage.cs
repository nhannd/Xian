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
		private event EventHandler _textChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the page.</param>
		/// <param name="preText">The text to display on the title bar.</param>
		/// <param name="text">The text to display on the title bar.</param>
		/// <param name="postText">The text to display on the title bar.</param>
		/// <param name="component">The <see cref="IApplicationComponent"/> to be hosted in this page.</param>
		public StackTabPage(string name, string preText, string text, string postText, IApplicationComponent component)
			: base(name, component)
		{
			_preText = preText;
			_text = text;
			_postText = postText;
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
