#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// The GroupHint is used to determine a reasonably appropriate point in the 
	/// action model to put an action that does not yet exist in the stored model.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The action (call it Left-Hand Action) whose position in the store is to be 
	/// determined is compared with each action in the store (Right-Hand Action).
	/// The comparison of the Left-Hand Action to the Right-Hand Action is given
	/// a score.  The score is based on the GroupHint and the algorithm works 
	/// as follows:
	/// </para>
	/// <para>
	///		LHS										RHS										Score
	///		-----------------------------------------------------------------------------------------
	///	1.	Tools.Image.Manipulation.Zoom			""										1
	///	2.	Tools.Image.Manipulation.Zoom			Tools.Image.Manipulation.Pan			4
	///	3.	Tools.Image.Manipulation.Zoom			DisplaySets								0
	/// 4.  ""										""										1
	/// 5.  ""										DisplaySets								0
	/// </para>
	/// <para>
	/// A brief explanation of the logic:
	/// <list type="bullet">
	/// <item>
	/// For backward compatibility, actions with a non-empty GroupHint, when compared to an 
	/// existing action in the store whose GroupHint="", the score is 1 because it is considered
	/// a better match than 2 actions whose GroupHints are non-empty and are completely different.
	/// </item>
	/// <item>
	/// Actions with GroupHints that have similar components (separated by '.') are given a score
	/// equal to the number of (consecutive) matching components + 1.  The +1 accounts for the fact 
	/// that any number of equal components is a better score than the first example, whose score is 1.
	/// </item>
	/// <item>
	/// Actions with completely different components are given an automatic score of zero (0).
	/// Two actions with GroupHints = "" are considered equal, so a score of 1 is given.
	/// </item>
	/// <item>
	/// In this case, an existing action with an empty GroupHint is being matched to a non-empty
	/// GroupHint.  So, the LHS cannot be considered at all similar to RHS and the 
	/// score is automatically zero (0).
	/// </item>
	/// </list>
	/// </para>
	/// </remarks>
	public class GroupHint
	{
		private const char SEPARATOR = '.';
		private readonly string _hint;
		private readonly string[] _components;

        /// <summary>
        /// Constructor.
        /// </summary>
		public GroupHint(string groupHint)
		{
			if (groupHint == null)
				groupHint = "";

			_hint = groupHint;
			_components = _hint.Split(new char[] { SEPARATOR });
		}

        /// <summary>
        /// Gets the hint path.
        /// </summary>
		public string Hint
		{
			get { return _hint; }
		}

        /// <summary>
        /// Gets an array containing the components of the hint path.
        /// </summary>
		protected string[] Components
		{
			get { return _components; }
		}

        /// <summary>
        /// Performs matching based on the algorithm described in <see cref="GroupHint"/>'s class summary.
        /// </summary>
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
