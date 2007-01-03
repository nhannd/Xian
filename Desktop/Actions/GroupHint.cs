using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// The GroupHint is used to determine a reasonably appropriate point in the 
	/// <see cref="ActionModelStore"/> to put an action that does not yet exist in
	/// the store.
	/// 
	/// The action (call it Left-Hand Action) whose position in the store is to be 
	/// determined is compared with each action in the store (Right-Hand Action).
	/// The comparison of the Left-Hand Action to the Right-Hand Action is given
	/// a score.  The score is based on the GroupHint and the algorithm works 
	/// as follows:
	///	
	///		LHS										RHS										Score
	///		-----------------------------------------------------------------------------------------
	///	1.	Tools.Image.Manipulation.Zoom			""										1
	///	2.	Tools.Image.Manipulation.Zoom			Tools.Image.Manipulation.Pan			4
	///	3.	Tools.Image.Manipulation.Zoom			DisplaySets								0
	/// 4.  ""										""										1
	/// 5.  ""										DisplaySets								0
	/// 
	/// A brief explanation of the logic:
	///	1. For backward compatibility, actions with a non-empty GroupHint, when compared to an 
	///    existing action in the store whose GroupHint="", the score is 1 because it is considered
	///    a better match than 2 actions whose GroupHints are non-empty and are completely different.
	/// 2. Actions with GroupHints that have similar components (separated by '.') are given a score
	///    equal to the number of (consecutive) matching components + 1.  The +1 accounts for the fact 
	///    that any number of equal components is a better score than the first example, whose score is 1.
	/// 3. Actions with completely different components are given an automatic score of zero (0).
	/// 4. Two actions with GroupHints = "" are considered equal, so a score of 1 is given.
	/// 5. In this case, an existing action with an empty GroupHint is being matched to a non-empty
	///    GroupHint.  So, the LHS cannot be considered at all similar to RHS and the 
	///    score is automatically zero (0).
	/// 
	/// </summary>

	public class GroupHint
	{
		private const char SEPARATOR = '.';
		private string _hint;
		private readonly string[] _components;

		public GroupHint(string groupHint)
		{
			if (groupHint == null)
				groupHint = "";

			_hint = groupHint;
			_components = _hint.Split(new char[] { SEPARATOR });
		}

		public string Hint
		{
			get { return _hint; }
		}

		public string[] Components
		{
			get { return _components; }
		}

		public int MatchScore(GroupHint other)
		{
			int i = 0;

			// the group "" is considered the default, and all group hints are considered
			// to be a match for the default group, so the score is 1.
			if (other.Components[0] == "")
				return 1;

			foreach (string otherComponent in other.Components)
			{
				if (_components.Length > i)
				{
					if (otherComponent == _components[i])
						++i;
					else 
						break;
				}
				else 
					break;
			}

			// if there were any matching components, the score is increased by 1 (because
			// the 'default' score is 1.  If the 'other component' is not a default component
			// then the score remains at 0, because there are no matches.
			if (i != 0)
				++i;

			return i;
		}
	}
}
