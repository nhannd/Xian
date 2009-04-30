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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using System.IO;

namespace ClearCanvas.Desktop.Applets.WebBrowser
{
	class FavouritesBuilder
	{
		public static ActionSet Build(WebBrowserComponent webBrowser)
		{
			List<IAction> actions = new List<IAction>();

			string favouritesFolder = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
			string[] favourites = Directory.GetFiles(favouritesFolder, "*.*", SearchOption.AllDirectories);

			// The action ID is arbitrary--it's just an identifier--as long as all the 
			// action IDs of all the actions in a given ActionModel are unqiue.
			// So, we'll just generate a new action ID for each action here
			// simply by incrementing an integer.
			int actionID = 0;

			foreach (string favouritePath in favourites)
			{
				IAction action = CreateAction(
					actionID.ToString(),
					favouritePath,
					webBrowser);
				
				actions.Add(action);
				actionID++;
			}

			return new ActionSet(actions);
		}

		private static IAction CreateAction(
			string actionID,
			string path,
			WebBrowserComponent webBrowser)
		{
			string url = ExtractUrlFromFavourite(path);
			string menuPath = CreateMenuPath(path);

			// Create the menu action
			ActionPath actionPath = new ActionPath(menuPath, null);
			MenuAction action = new MenuAction(actionID, actionPath, ClickActionFlags.None, null);
			action.Label = actionPath.LastSegment.LocalizedText;

			// Set what we're supposed to do when the menu item is clicked
			action.SetClickHandler(
				delegate
				{
					// Navigate to the URL
					webBrowser.Url = url;
					webBrowser.Go();
				});

			return action;
		}

		private static string ExtractUrlFromFavourite(string path)
		{
			// Read the .url file into a single string
			string text = File.ReadAllText(path);

			if (text.Length == 0)
				return String.Empty;

			// Break up the file into separate lines
			string[] lines = text.Split('\n');

			foreach (string line in lines)
			{
				// Find the first line that contains "URL=http";
				// that should be the link we want.
				if (line.Contains("URL=http"))
				{
					// The URL is everything to the right of the =
					string[] substrings = line.Split('=');

					if (substrings.Length >= 2)
						return substrings[1];
				}
			}

			return String.Empty;
		}

		private static string CreateMenuPath(string path)
		{
			string favouritesFolder = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
			string menuPath = path.Replace(favouritesFolder, "");
			menuPath = menuPath.Replace(".url", "");
			menuPath = menuPath.Replace("\\", "/");
			// We want the menu item to be in the global menu
			menuPath = "global-menus/Favourites" + menuPath;
			return menuPath;
		}
	}
}
