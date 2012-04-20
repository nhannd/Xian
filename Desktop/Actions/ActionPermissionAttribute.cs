#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Associates authority tokens with an action.
	/// </summary>
	/// <remarks>
	/// This attribute sets the action permissibility via the <see cref="Action.SetPermissibility(ISpecification)"/> method.
	/// If multiple authority tokens are supplied in an array to a single instance of the attribute, those tokens will be combined using AND.  If
	/// multiple instances of this attribute are specified, the tokens associated with each instance are combined
	/// using OR logic.  This allows for the possibility of constructing a permission specification based on a complex boolean
	/// combination of authority tokens.
	/// </remarks>
	public class ActionPermissionAttribute : ActionDecoratorAttribute
	{
		private readonly string[] _authorityTokens;

		/// <summary>
		/// Constructor - the specified authority token will be associated with the specified action ID.
		/// </summary>
		public ActionPermissionAttribute(string actionId, string authorityToken)
			: this(actionId, new[] {authorityToken}) {}

		/// <summary>
		/// Constructor - all of the specified tokens will combined using AND and associated with the specified action ID.
		/// </summary>
		public ActionPermissionAttribute(string actionId, params string[] authorityTokens)
			: base(actionId)
		{
			_authorityTokens = authorityTokens;
		}

		/// <summary>
		/// Feature identification token to be checked against application licensing.
		/// </summary>
		public string FeatureToken { get; set; }

		/// <summary>
		/// Gets the associated authority tokens.
		/// </summary>
		protected string[] AuthorityTokens
		{
			get { return _authorityTokens; }
		}

		/// <summary>
		/// Applies permissions represented by this attribute to an action instance, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
		public override void Apply(IActionBuildingContext builder)
		{
			// if this is the first occurence of this attribute, create the parent spec
			if (builder.Action.PermissionSpecification == null)
				builder.Action.PermissionSpecification = new OrSpecification();

			var specification = CreateSpecification();

			// combine this spec with any previous occurence of this attribute using OR logic
			((OrSpecification) builder.Action.PermissionSpecification).Add(specification);
		}

		/// <summary>
		/// Called to create the permission specification.
		/// </summary>
		protected virtual ISpecification CreateSpecification()
		{
			// combine the specified tokens with AND logic
			var specification = new AndSpecification();

			foreach (string token in _authorityTokens)
			{
				specification.Add(new PrincipalPermissionSpecification(token));
			}

			if (!string.IsNullOrEmpty(FeatureToken))
			{
				specification.Add(new FeatureAuthorizationSpecification(FeatureToken));
			}

			return specification;
		}
	}
}