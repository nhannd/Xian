var errorLogPath = ".\\error.log";

  var connection = WScript.CreateObject("ADODB.connection");
     connection.Provider = "sqloledb";
     connection.Properties("Data Source").Value = "++HOST++++EXPRESS++";
     connection.Properties("Initial Catalog").Value = "master";

  fs = new ActiveXObject("Scripting.FileSystemObject");	 
  if (fs.FileExists(errorLogPath))
  fs.DeleteFile(errorLogPath);
  var text = "";
  try
  {
     connection.Open("", "++ADMINID++", "++ADMINPASS++");
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
  
  WScript.Quit(0);