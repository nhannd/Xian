#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using HttpNamespaceManager.Lib.AccessControl;
using HttpNamespaceManager.Lib;
using System.Text;

namespace HttpNamespaceUtility
{
	public class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Exec(args);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		private static void Exec(string[] args)
		{
			//TODO: should use regular expressions, but no time to be elegant.
			bool remove = false;

			bool showUsage = true;
			if (args.Length < 2 || args.Length > 3)
			{
			}
			else if (args[0] == "-a" && args.Length == 3)
			{
				showUsage = false;
				//usage: -a <url> <user>
			}
			else if (args[0] == "-r" && args.Length == 2)
			{
				showUsage = false;
				remove = true;
				//usage: -r <url>
			}

			if (showUsage)
			{
				PrintUsage();
				return;
			}

			string url = args[1];

			if (remove)
			{
				using (HttpApi api = new HttpApi())
				{
					api.RemoveHttpHamespaceAcl(url);
					Console.WriteLine("Successfully removed namespace: {0}", url);
				}
			}
			else
			{
				SecurityIdentity owner = null;
				try
				{
					owner = SecurityIdentity.SecurityIdentityFromName(Environment.UserName);
				}
				catch
				{
				}

				List<string> users = new List<string>((args[2] ?? "").Split(new char[]{'|'}));
				List<SecurityIdentity> identities = new List<SecurityIdentity>();
				foreach (string user in users)
					identities.Add(SecurityIdentity.SecurityIdentityFromName(user));

				if (identities.Count == 0)
				{
					Console.WriteLine("Unrecognized user(s): {0}", users);
				}
				else
				{
					using (HttpApi api = new HttpApi())
					{
						SecurityDescriptor descriptor = new SecurityDescriptor();
						if (owner != null)
							descriptor.Owner = owner;

						descriptor.DACL = new AccessControlList();

						foreach (SecurityIdentity identity in identities)
						{
							AccessControlEntry ace = new AccessControlEntry(identity);
							ace.AceType = AceType.AccessAllowed;
							ace.Rights = AceRights.GenericAll;
							descriptor.DACL.Add(ace);
						}

						api.SetHttpNamespaceAcl(url, descriptor);
					}

					StringBuilder builder = new StringBuilder();
					builder.AppendFormat("Successfully added namespace: {0}", url);
					builder.AppendLine();
					foreach (SecurityIdentity identity in identities)
					{
						builder.AppendFormat("User/Group: {0}", identity.Name);
						builder.AppendLine();
					}

					Console.WriteLine(builder);
				}
			}
		}

		private static void PrintUsage()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("Usage:");
			builder.AppendLine("HttpNamespaceUtility -a <url> <User/Group>|<User/Group>");
			builder.AppendLine("-Or-");
			builder.AppendLine("HttpNamespaceUtility -r <url>");

			Console.WriteLine(builder);
		}
	}
}
