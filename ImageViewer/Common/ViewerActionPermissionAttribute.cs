using ClearCanvas.Common.Specifications;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Common
{
	/// <summary>
	/// Associates authority tokens with a viewer-specific action.
	/// </summary>
	/// <remarks>
	/// <para>
	/// All tokens specified will be ANDed with <see cref="AuthorityTokens.General"/>, which is a global token
	/// intended to limit access to all viewer-specific functionality.
	/// </para>
	/// <para>
	/// This attribute sets the action permissibility via the <see cref="Action.SetPermissibility(ISpecification)"/> method.
	/// If multiple authority tokens are supplied in an array to a single instance of the attribute, those tokens will be combined using AND.  If
	/// multiple instances of this attribute are specified, the tokens associated with each instance are combined
	/// using OR logic.  This allows for the possibility of constructing a permission specification based on a complex boolean
	/// combination of authority tokens.
	/// </para>
	/// </remarks>
	public class ViewerActionPermissionAttribute : ActionPermissionAttribute
	{
		/// <summary>
		/// Constructor - the specified authority token will be associated with the specified action ID.
		/// </summary>
		public ViewerActionPermissionAttribute(string actionID, string authorityToken)
            : this(actionID, new string[] { authorityToken })
        {
        }

		/// <summary>
		/// Constructor - all of the specified tokens will combined using AND and associated with the specified action ID.
		/// </summary>
		public ViewerActionPermissionAttribute(string actionID, params string[] authorityTokens)
			: base(actionID, CreateViewerTokens(authorityTokens))
        {
        }

		private static string[] CreateViewerTokens(string[] authorityTokens)
		{
			authorityTokens = authorityTokens ?? new string[0];
			string[] viewerTokens = new string[authorityTokens.Length + 1];
			
			viewerTokens[0] = AuthorityTokens.General;
			
			for (int i = 0; i < authorityTokens.Length; ++i)
				viewerTokens[i + 1] = authorityTokens[i];

			return viewerTokens;
		}
	}
}
