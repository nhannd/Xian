// Create some global objects for file and shell access
var fso = WScript.CreateObject("Scripting.FileSystemObject");
var shell = WScript.CreateObject("WScript.Shell");

//Build Configuration must be passed in a data object
Assert(WScript.Arguments.Count() > 0, "Data file must be supplied as the argument.");

GenerateDatabase();

function GenerateDatabase()
{
	// Execute scripts using cscript
	function ExecuteCScripts(message, scripts)
	{
		WScript.Echo(message);
		var shellCommand = "cscript " + scripts;
		var err = shell.Run(shellCommand, 7, true);
		if (err == 1)
			HandleException("ExecuteCScripts " + shellCommand);
	}

	// Execute command in a command shell
	function ExecuteShellCommand(message, command, options)
	{
		WScript.Echo(message);
		var shellCommand = "cmd /c " + command + " " + options;
		//var shellCommand = command + " " + options;
		var err = shell.Run(shellCommand, 1, true);
		if (err == 1)
			HandleException("ExecuteShellCommand " + shellCommand);
	}

	// Load configuration data from a file
	function LoadConfiguration(file) 
	{
		try 
		{
			var content = ReadFromFile(file);
			var config = eval('({' + content + '})');
			Assert(fso.FileExists(config.EXECUTABLE), config.EXECUTABLE + " could not be found.  Ensure that build has been performed or that the specified path to the exe is correct.");
			return config;
		}
		catch (e)
		{
			HandleException("LoadConfiguration", e);
		}
	}

	// Update scripts in JscriptTemplates and SqlScriptTemplates and place the new scripts in the temp directory
	function UpdateSQLTemplate(config)
	{
		//this array should contain any subfolder locations that contain scripts made into templates
		var templateDirs = new Array(".\\JscriptTemplates", ".\\SqlScriptTemplates");

		//this section loops through each of the script files and replaces any tokens with their specified values
		//the exception being the SQLEXPRESS flag token; if this is true then \SQLEXPRESS has to be appended to the host string
		for (var i in templateDirs)
		{
			var folder = fso.GetFolder(templateDirs[i]);
			var e = new Enumerator(folder.files);

			for (; !e.atEnd();e.moveNext())
			{
				var text = ReadFromFile(e.item().Path);

				for (var propertyName in config) 
				{
					var value = config[propertyName];
					if(propertyName == "EXPRESS")
					{
						value = (value == true || value == "true") ? "\\\\SQLEXPRESS" : "";
					}
					
					var regex = new RegExp("\\+\\+" + propertyName + "\\+\\+", "g");
					if(text.match(regex) !=null)
					{
						text = text.replace(regex,value);             
					}

					WriteToFile(config.TEMPFOLDER + "\\" + e.item().Name, text);
				}
			}
		}
	}

	var config = LoadConfiguration(WScript.Arguments.Item(0));
	RecreateDirectory(config.TEMPFOLDER);
	UpdateSQLTemplate(config);

	//WScript.Echo(WScript.ScriptFullName);
	//have to perform a little light gymnastics here to get the temporary directory in case it's specified above as relative
	//the issue is that when using the ddlwriter the scripts needs to pass an absolsolute output ddl file path so that it can be accessed in the following step
	var fileInTemp = fso.GetFile(config.TEMPFOLDER + "\\dbfile.js");
	var exeDir = fso.GetFile(config.EXECUTABLE).ParentFolder.Path;
	fso.CopyFile(config.TEMPFOLDER + "\\hibernate.cfg.xml", exeDir + "\\hibernate.cfg.xml", true);

	ExecuteCScripts("Testing Database Connection...", config.TEMPFOLDER + "\\dbConnectionTest.js ");
	ExecuteCScripts("Creating Database...", config.TEMPFOLDER + "\\dbfile.js " + config.TEMPFOLDER + "\\CreateDB.sql");
	ExecuteCScripts("Creating Database User...", config.TEMPFOLDER + "\\dbfile.js " + config.TEMPFOLDER + "\\User.sql");
	ExecuteShellCommand("Creating Table Creation Script...",  "\"" + config.EXECUTABLE + "\" ClearCanvas.Enterprise.Hibernate.DdlWriter.DdlWriterApplication", "/enums:all /ix /fki /out:" + fileInTemp.ParentFolder.Path + "\\model.ddl");
	ExecuteCScripts("Creating Tables...", config.TEMPFOLDER + "\\dbfile.js " + config.TEMPFOLDER + "\\model.ddl");

	if (config.EMBEDDED_ENTERPRISE)
		ExecuteShellCommand("Running Enterprise Authentication setup applications...", "\"" + config.EXECUTABLE + "\" ClearCanvas.Enterprise.Authentication.Setup.SetupApplication");

	ExecuteShellCommand("Running Enterprise Common setup applications...", "\"" + config.EXECUTABLE + "\" ClearCanvas.Enterprise.Common.Setup.SetupApplication");

	// Execute a list of import actions
	for (var actionIndex in config.IMPORTACTIONS)
	{
		ExecuteShellCommand(config.IMPORTACTIONS[actionIndex].Message, config.IMPORTACTIONS[actionIndex].Command, config.IMPORTACTIONS[actionIndex].Argument);
	}
}

function Assert(condition, errorMessage) 
{
	if (!condition)
		HandleException("Assertion Failed", new Error(errorMessage));
}

function ReadFromFile(file)
{
	try
	{
		var ForReading = 1;
		var s = fso.OpenTextFile(file, ForReading, true);
		var content = s.ReadAll(); //also we can use s.ReadAll() to read all the lines;
		s.Close();
		return content;
	}
	catch (e)
	{
		HandleException("ReadFromFile " + file, e);
	}
}

function WriteToFile(file, content)
{
	try
	{
		var ForWriting = 2;
		var file = fso.OpenTextFile(file, ForWriting, true);
		file.Write(content);
		file.Close();
	}
	catch (e)
	{
		HandleException("WriteToFile " + file, e);
	}
}

function RecreateDirectory(path)
{
	try
	{
		if (fso.FolderExists(path))
			fso.DeleteFolder(path);

		fso.CreateFolder(path);
	}
	catch(e)
	{
		HandleException("RecreateDirectory " + path, e);
	}
}

function HandleException(functionName, e)
{
	function ShowErrorInConsole()
	{
		if (fso.FileExists(".\\error.log"))
		{
			file = fso.OpenTextFile(".\\error.log", 1);
			text = file.ReadAll();
			WScript.Echo(text);
			file.Close();
		}
	}

	WScript.Echo("An error has occurred and the script cannot continue.");

	if (functionName)
		WScript.Echo("Function name: " + functionName);

	if (e)
	{
		WScript.Echo("Error name: " + e.name);
		WScript.Echo("Error message: " + e.message);
	}

	ShowErrorInConsole();
	WScript.Quit(1);
}
