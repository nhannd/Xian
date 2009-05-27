WScript.Echo("Connecting...");

var connection = WScript.CreateObject("ADODB.connection");

connection.Provider = "sqloledb";
connection.Properties("Data Source").Value = "172.16.10.167\\SQLEXPRESS";
connection.Properties("Initial Catalog").Value = "master";
connection.Open("", "sa", "redhat5.1");

WScript.Echo("Connected.");
while(!WScript.StdIn.AtEndOfStream)
{
	var line = WScript.StdIn.ReadLine();
	connection.Execute(line);
	WScript.Echo(line);
}

WScript.Echo("Done");