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
connection.CommandTimeout = 120;
connection.Open("", "++ADMINID++", "++ADMINPASS++");

WScript.Echo("Connected.");

try
{
	var inputfile = fs.OpenTextFile(WScript.Arguments(0), 1);
	var sql = inputfile.ReadAll();
	inputfile.Close();
	WScript.Echo(sql);
	connection.Execute(sql);
	WScript.Echo("Done.");
	WScript.Quit(0);
}
catch(e)
{
	WScript.Echo(e.message);
}