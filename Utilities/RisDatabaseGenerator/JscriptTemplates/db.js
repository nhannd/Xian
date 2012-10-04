WScript.Echo("Connecting...");

var connection = WScript.CreateObject("ADODB.connection");

connection.Provider = "sqloledb";
connection.Properties("Data Source").Value = "++HOST++++EXPRESS++";
connection.Properties("Initial Catalog").Value = "master";
connection.Open("", "++ADMINID++", "++ADMINPASS++");

WScript.Echo("Connected.");
while(!WScript.StdIn.AtEndOfStream)
{
	var line = WScript.StdIn.ReadLine();
	connection.Execute(line);
	WScript.Echo(line);
}

WScript.Echo("Done");