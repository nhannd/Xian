//Build Configuration must be passed in as a parameter else Debug is assumed
var configuration;
if (WScript.Arguments.Count()==0)
{
   configuration = Debug;
}
else
{
   configuration = WScript.Arguments.Item(0);
}   

//variables used for file access
var ForReading = 1, ForWriting = 2;

//script environment variables
var xmlFile = "EnterpriseDatabaseGenerator.xml";
var tempDir = ".\\temp";
var exePath = "..\\..\\Enterprise\\Server\\Executable\\bin\\" + configuration + "\\ClearCanvas.Enterprise.Server.Executable.exe";
var sampleDataPath = "..\\..\\Enterprise\\SampleData";

//this array should contain any subfolder locations that contain scripts made into templates
var templateDirs = new Array(".\\JscriptTemplates", ".\\SqlScriptTemplates");

//creation of temporary directory
var fs = WScript.CreateObject("Scripting.FileSystemObject");
if (fs.FolderExists(tempDir))
   fs.DeleteFolder(tempDir);
fs.CreateFolder(tempDir);

//loading of input xml file
var xml = new ActiveXObject("Microsoft.XMLDOM");
xml.validateOnParse = true;
xml.async = false;
xml.load(xmlFile);

if (xml.parseError.errorCode != 0)
    WScript.Echo("XML Parse Error : " + xml.parseError.reason);
else
{
//this section loops through each of the script files and replaces any tokens with their specified values
//the exception being the SQLEXPRESS flag token; if this is true then \SQLEXPRESS has to be appended to the host string
   var fso = WScript.CreateObject("Scripting.FileSystemObject");
   var file, text, root, token, value, regex, matches;

   //read list of template files
   for (var i=0;i<templateDirs.length;i++)
   {
      var folder = fs.GetFolder(templateDirs[i]);
   
      var e = new Enumerator(folder.files);
      for (; !e.atEnd();e.moveNext())
      {
         file = fso.OpenTextFile(e.item().Path, ForReading);
		 text = file.ReadAll();
		 file.Close();

	     root = xml.documentElement;
	     for (var j=0;j<root.childNodes.length;j++)
	     {  
			token = root.childNodes[j].getAttribute("name");
	        value = root.childNodes[j].getAttribute("value");
			
			regex = new RegExp("\\+\\+" + token + "\\+\\+", "g");
			matches = text.match(regex);
			
			if(matches !=null)
            {
			   if(token == "EXPRESS" && value == "true")
			   {
			      value="\\\\SQLEXPRESS";
			      if(e.item().Name == "hibernate.cfg.xml")
				     value="\\SQLEXPRESS";
			   }

               text = text.replace(regex,value);	  	                          
            }
			file = fso.OpenTextFile(tempDir + "\\" + e.item().Name, ForWriting, true);
            file.Write(text);
			file.Close();
	     }
	  }
   }
   
   if (fso.FileExists(exePath))
   {
   //   WScript.Echo(WScript.ScriptFullName);
      //have to perform a little light gymnastics here to get the temporary directory in case it's specified above as relative
	  //the issue is that when using the ddlwriter the scripts needs to pass an absolsolute output ddl file path so that it can be accessed in the following step
      var fileInTemp = fso.GetFile(tempDir + "\\dbfile.js");
      var exeDir = fso.GetFile(exePath).ParentFolder.Path;
	  var err = 0;
      
	  fso.CopyFile(tempDir + "\\hibernate.cfg.xml", exeDir + "\\hibernate.cfg.xml", true);
	  
	  shell = WScript.CreateObject("WScript.Shell");
	 
	  WScript.Echo("Testing Database Connection...");
	  err = shell.Run("cscript " + tempDir + "\\dbConnectionTest.js ", 7, true);
	  if (err == 1) HandleError();	
	  WScript.Echo("Creating Database...");
	  err = shell.Run("cscript " + tempDir + "\\dbfile.js " + tempDir + "\\CreateDB.sql", 7, true);
	  if (err == 1) HandleError();
	  WScript.Echo("Creating Database User...");	 
	  err = shell.Run("cscript " + tempDir + "\\dbfile.js " + tempDir + "\\User.sql", 7, true);	  
	  if (err == 1) HandleError();
	  WScript.Echo("Creating Table Creation Script...");	 
	  err = shell.Run("cmd /c \"" + exePath + "\" ClearCanvas.Enterprise.Hibernate.DdlWriter.DdlWriterApplication /enums:all /ix /fki /out:" + fileInTemp.ParentFolder.Path + "\\model.ddl" , 7,true);
	  if (err == 1) HandleError();
	  WScript.Echo("Creating Tables...");	 
	  err = shell.Run("cscript " + tempDir + "\\dbfile.js " + tempDir + "\\model.ddl", 7,true);			
	  if (err == 1) HandleError();
	  WScript.Echo("Running setup applications...");	 
	  err = shell.Run("cmd /c \"" + exePath + "\" ClearCanvas.Enterprise.Authentication.Setup.SetupApplication", 7,true);
	  if (err == 1) HandleError();
	  err = shell.Run("cmd /c \"" + exePath + "\" ClearCanvas.Enterprise.Common.Setup.SetupApplication", 7,true);
	  if (err == 1) HandleError();
	  
	  WScript.Echo("Importing Sample Data...");	 	  
	  shell.Run("cmd /c CD \"" + sampleDataPath + "\" & ImportSampleData.bat " + exePath, 1, true);	  
   }
   else
   {
      WScript.Echo("ClearCanvas.Enterprise.Server.Executable.exe could not be found.  Ensure that build has been performed or that the specified path to the exe is correct.");
   }   
}

function HandleError()
{
   WScript.Echo("An error has occurred and the script cannot continue.");
   
   if (fs.FileExists(".\\error.log"))
   {
	   file = fs.OpenTextFile(".\\error.log", 1);
       text = file.ReadAll();
	   WScript.Echo(text);
	   file.Close();
   }
   
   WScript.Quit(1);
}


