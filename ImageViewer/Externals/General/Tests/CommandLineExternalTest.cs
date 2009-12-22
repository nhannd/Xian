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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Externals.General.Tests
{
	[TestFixture]
	public class CommandLineExternalTest
	{
		private const int _processEndWaitDelay = 1000;

		[Test]
		public void TestBasic()
		{
			string workingDirectory = Environment.CurrentDirectory;
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;

				Assert.IsTrue(external.IsValid, "Minimum parameters for Command Line External should be just the command itself");

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					Assert.IsTrue(external.CanLaunch(image), "Check that external can launch the dummy image");

					external.Launch(image);

					Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
					command.Refresh();

					Trace.WriteLine(string.Format("Command Execution Report"));
					Trace.WriteLine(command.ExecutionReport);

					AssertAreEqualIgnoreCase(external.Command, command.ExecutedCommand, "Wrong command was executed");
					AssertAreEqualIgnoreCase(workingDirectory, command.ExecutedWorkingDirectory, "Command executed in wrong working directory.");
				}
			}
		}

		[Test]
		public void TestWorkingDirectory()
		{
			string workingDirectory = Environment.CurrentDirectory;
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.WorkingDirectory = Path.GetPathRoot(workingDirectory);
				external.Command = command.ScriptFilename;

				Assert.IsTrue(external.IsValid, "Minimum parameters for Command Line External should be just the command itself");

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					external.Launch(image);

					Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
					command.Refresh();

					Trace.WriteLine(string.Format("Command Execution Report"));
					Trace.WriteLine(command.ExecutionReport);

					AssertAreEqualIgnoreCase(external.Command, command.ExecutedCommand, "Wrong command was executed");
					AssertAreEqualIgnoreCase(external.WorkingDirectory, command.ExecutedWorkingDirectory, "Command executed in wrong working directory.");
				}
			}
		}

		[Test]
		public void TestArguments()
		{
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;
				external.Arguments = "\"USS Enterprise\" \"USS Defiant\" \"USS Voyager\" \"USS Excelsior\" \"USS Reliant\"";

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					external.Launch(image);

					Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
					command.Refresh();

					Trace.WriteLine(string.Format("Command Execution Report"));
					Trace.WriteLine(command.ExecutionReport);

					Assert.AreEqual("\"USS Enterprise\"", command.ExecutedArguments[0], "Wrong argument passed at index {0}", 0);
					Assert.AreEqual("\"USS Defiant\"", command.ExecutedArguments[1], "Wrong argument passed at index {0}", 1);
					Assert.AreEqual("\"USS Voyager\"", command.ExecutedArguments[2], "Wrong argument passed at index {0}", 2);
					Assert.AreEqual("\"USS Excelsior\"", command.ExecutedArguments[3], "Wrong argument passed at index {0}", 3);
					Assert.AreEqual("\"USS Reliant\"", command.ExecutedArguments[4], "Wrong argument passed at index {0}", 4);
				}
			}
		}

		[Test]
		public void TestArgumentFields()
		{
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;
				external.Arguments = "$$ \"$filename$\"";

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					Assert.IsFalse(external.CanLaunch(image), "Fields are case sensitive - unresolved field should fail the launch");

					external.Arguments = "$$ \"$FILENAME$\"";

					Assert.IsTrue(external.CanLaunch(image), "Fields are case sensitive - unresolved field should fail the launch");

					external.Launch(image);

					Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
					command.Refresh();

					Trace.WriteLine(string.Format("Command Execution Report"));
					Trace.WriteLine(command.ExecutionReport);

					AssertAreEqualIgnoreCase("$", command.ExecutedArguments[0], "Wrong argument passed at index {0}", 0);

					// these file paths may or may not have spaces in them, but we don't care either way for this test
					AssertAreEqualIgnoreCase(image.Filename, command.ExecutedArguments[1].Trim('"'), "Wrong argument passed at index {0}", 1);
				}
			}
		}

		[Test]
		public void TestSingleImage()
		{
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;
				external.AllowMultiValueFields = false;
				external.Arguments = "\"$FILENAME$\" \"$DIRECTORY$\" \"$EXTENSIONONLY$\" \"$FILENAMEONLY$\" \"$00100020$\" \"$00100021$\"";

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					using (MockDicomPresentationImage otherImage = new MockDicomPresentationImage())
					{
						image[0x00100020].SetStringValue("I've got a lovely bunch of coconuts");
						image[0x00100021].SetStringValue("Here they are all standing in a row");
						otherImage[0x00100020].SetStringValue("Big ones, small ones, some as big as your head");

						Assert.IsFalse(external.CanLaunch(new IPresentationImage[] {image, otherImage}));
						Assert.IsTrue(external.CanLaunch(image));

						external.Launch(image);

						Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
						command.Refresh();

						Trace.WriteLine(string.Format("Command Execution Report"));
						Trace.WriteLine(command.ExecutionReport);

						// these file paths may or may not have spaces in them, but we don't care either way for this test
						AssertAreEqualIgnoreCase(image.Filename, command.ExecutedArguments[0].Trim('"'), "Wrong argument for filename field");
						AssertAreEqualIgnoreCase(Path.GetDirectoryName(image.Filename), command.ExecutedArguments[1].Trim('"'), "Wrong argument for directory field");
						AssertAreEqualIgnoreCase(Path.GetExtension(image.Filename), command.ExecutedArguments[2].Trim('"'), "Wrong argument for extension field");
						AssertAreEqualIgnoreCase(Path.GetFileName(image.Filename), command.ExecutedArguments[3].Trim('"'), "Wrong argument for filename only field");
						Assert.AreEqual("I've got a lovely bunch of coconuts", command.ExecutedArguments[4].Trim('"'), "Wrong argument for 00100020 field");
						Assert.AreEqual("Here they are all standing in a row", command.ExecutedArguments[5].Trim('"'), "Wrong argument for 00100021 field");
					}
				}
			}
		}

		[Test]
		public void TestMultipleImages()
		{
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;
				external.AllowMultiValueFields = true;
				external.MultiValueFieldSeparator = "\" \"";
				external.Arguments = "\"$FILENAME$\" \"$DIRECTORY$\" \"$EXTENSIONONLY$\" \"$FILENAMEONLY$\"";

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					using (MockDicomPresentationImage otherImage = new MockDicomPresentationImage())
					{
						using (MockDicomPresentationImage thirdImage = new MockDicomPresentationImage())
						{
							IPresentationImage[] images = new IPresentationImage[] {image, otherImage, thirdImage};
							Assert.IsTrue(external.CanLaunch(images));
							external.Launch(images);

							Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
							command.Refresh();

							Trace.WriteLine(string.Format("Command Execution Report"));
							Trace.WriteLine(command.ExecutionReport);

							// these file paths may or may not have spaces in them, but we don't care either way for this test
							AssertAreEqualIgnoreCase(image.Filename, command.ExecutedArguments[0].Replace("\"", ""), "Wrong argument for 1st filename field");
							AssertAreEqualIgnoreCase(otherImage.Filename, command.ExecutedArguments[1].Replace("\"", ""), "Wrong argument for 2nd filename field");
							AssertAreEqualIgnoreCase(thirdImage.Filename, command.ExecutedArguments[2].Replace("\"", ""), "Wrong argument for 3rd filename field");
							AssertAreEqualIgnoreCase(Path.GetDirectoryName(image.Filename), command.ExecutedArguments[3].Replace("\"", ""), "Wrong argument for directory field");
							AssertAreEqualIgnoreCase(Path.GetExtension(image.Filename), command.ExecutedArguments[4].Replace("\"", ""), "Wrong argument for extension field");
							AssertAreEqualIgnoreCase(Path.GetFileName(image.Filename), command.ExecutedArguments[5].Replace("\"", ""), "Wrong argument for 1st filename only field");
							AssertAreEqualIgnoreCase(Path.GetFileName(otherImage.Filename), command.ExecutedArguments[6].Replace("\"", ""), "Wrong argument for 2nd filename only field");
							AssertAreEqualIgnoreCase(Path.GetFileName(thirdImage.Filename), command.ExecutedArguments[7].Replace("\"", ""), "Wrong argument for 3rd filename only field");
						}
					}
				}
			}
		}

		[Test]
		public void TestDicomFieldsWithMultipleImages()
		{
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;
				external.AllowMultiValueFields = true;
				external.MultiValueFieldSeparator = " ";
				external.Arguments = "\"$00100020$\" \"$00100021$\"";

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					using (MockDicomPresentationImage otherImage = new MockDicomPresentationImage())
					{
						using (MockDicomPresentationImage thirdImage = new MockDicomPresentationImage())
						{
							using (MockDicomPresentationImage fourthImage = new MockDicomPresentationImage())
							{
								using (MockDicomPresentationImage anotherImage = new MockDicomPresentationImage())
								{
									image[0x00100020].SetStringValue("The cake is a lie");
									otherImage[0x00100020].SetStringValue("The cake is a lie");
									thirdImage[0x00100020].SetStringValue("The cake is a lie");
									fourthImage[0x00100020].SetStringValue("The cake is a lie");
									anotherImage[0x00100020].SetStringValue("The cake is a lie");

									image[0x00100021].SetStringValue("Look, my liege!");
									otherImage[0x00100021].SetStringValue("Camelot!");
									thirdImage[0x00100021].SetStringValue("Camelot!");
									fourthImage[0x00100021].SetStringValue("Camelot!");
									anotherImage[0x00100021].SetStringValue("It's only a model");

									IPresentationImage[] images = new IPresentationImage[] {image, otherImage, thirdImage, fourthImage, anotherImage};
									Assert.IsTrue(external.CanLaunch(images));
									external.Launch(images);

									Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
									command.Refresh();

									Trace.WriteLine(string.Format("Command Execution Report"));
									Trace.WriteLine(command.ExecutionReport);

									// these file paths may or may not have spaces in them, but we don't care either way for this test
									Assert.AreEqual("\"The cake is a lie\"", command.ExecutedArguments[0], "Wrong argument for 00100020 field of first image");
									Assert.AreEqual("\"Look, my liege!\"", command.ExecutedArguments[1], "Wrong argument for 00100021 field of first image");
								}
							}
						}
					}
				}
			}
		}

		private static void AssertAreEqualIgnoreCase(string a, string b, string message, params object[] args)
		{
			if (!string.IsNullOrEmpty(a))
				a = a.ToLowerInvariant();
			if (!string.IsNullOrEmpty(b))
				b = b.ToLowerInvariant();
			Assert.AreEqual(a, b, message, args);
		}
	}
}

#endif