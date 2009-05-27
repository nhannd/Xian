var errorLogPath = ".\\error.log";

WScript.Echo("Opening Script...");

// Create the object 
fs = new ActiveXObject("Scripting.FileSystemObject");
f = fs.GetFile(WScript.Arguments(0));

// Open the file 
is = f.OpenAsTextStream( 1, 0 );

WScript.Echo("Connecting...");

var connection = WScript.CreateObject("ADODB.connection");

connection.Provider = "sqloledb";
connection.Properties("Data Source").Value = "++HOST++++EXPRESS++";
connection.Properties("Initial Catalog").Value = "master";
connection.Open("", "++ADMINID++", "++ADMINPASS++");

WScript.Echo("Connected.");


// start and continue to read until we hit
// the end of the file. 
if (fs.FileExists(errorLogPath))
   fs.DeleteFile(errorLogPath);
var file, text = "";
while( !is.AtEndOfStream )
{   
   var line = is.ReadLine();
   try
   {
      connection.Execute(line);
   }
   catch(e)
   {   
	  if (fs.FileExists(errorLogPath))
	  {
	     file = fs.OpenTextFile(errorLogPath, 1);
         text = file.ReadAll();
	     file.Close();
	  }
	  
	  file = fs.OpenTextFile(errorLogPath, 2, true);
      file.Write(text + "\n" + e.description);
	  file.Close();
	  WScript.Quit(1);
   }  
   WScript.Echo(line);
}

// Close the stream 
is.Close();

WScript.Echo("Done");