function main(){
	var Service=CreateService("OrderGenerator");
	var GenData=CreateService("GenerateDataService");
	var WFService = CreateService("SimpleWorkflowService");
	
	Service.createPatients(GenData);
	Service.createVisits(GenData);
	Service.beVerbose(false);
	var diagnosticServices=GenData.GetDiagnosticServices({}).DiagnosticServices;
	Service.addMoreLat(["N","None"]);
	Service.addMoreLat(["B","Biliteral"]);
	Service.addMoreLat(["L","Left"]);
	Service.addMoreLat(["R","Right"]);
	Console.Write("Generating orders ");
	for(var i=0;i<diagnosticServices.length;i++){

		Service.setParameters("checkIn",null,null,null,null,null,null,diagnosticServices[i].Name);
		var order = Service.createOrders(GenData)
		Service.processOrders(WFService,order);
		Console.Write(".");
	}	
	Console.WriteLine(System.Environment.NewLine+"Orders for each procedure are generated");
	LogInfo("Done.");
}
	
