
var patientNumber=1;
enum upto{
	createPatients,
	createVisits,
	createOrders,
	toBeScheduled,
	scheduled,
	checkIn,
	perform,
	report,
	verify,
	cancel
}
var meanVisitsPerPatient = 1;
var maxVisitsPerPatient = 1;
var meanOrdersPerVisit = 1;
var maxOrdersPerVisit = 1;
var infoAuthInput:String[];
var modality:String="";
var target:upto;
var newPatients:Array;
var infoAuth:Array;
var visitResult:Array;
var visitInfoAuth:Array;
var orders:ArrayList;
var visitCount=0;
var verbose=true;
var parametersSet=false;
var emergency:boolean=false;
var date:String=null;
var facility:String=null;
var diagnosticServiceName:String;
var fileName=".\\input.txt";
var Randomize:boolean = false;
var latEnum:ArrayList=new ArrayList();

function addMoreLat(value){
	latEnum.Add(value);
}
function getOrders():ArrayList{
	return orders;
}
function beVerbose(value){
   verbose=value;
}

function setStats(meanVP,maxVP,meanOV,maxOV,randomize){
   meanVisitsPerPatient=meanVP;
   maxVisitsPerPatient=maxVP;
   meanOrdersPerVisit=meanOV;
   maxOrdersPerVisit=maxOV;
   Randomize=randomize;

}
function setParameters(t,p,i,e,m,d,f,s){
	if(t!=null){
		target=t;
	}
	if(p!=null){
		patientNumber=p;
	}
	if(i!=null){
		infoAuthInput=i;
	}
	if(e!=null){
		emergency=e;
	}
	if(m!=null){
	     modality=m;
	}
	if(d!=null){
		date=d;
	}
	if(f!=null){
	   facility=f;
	}
	if(s!=null){
	   diagnosticServiceName=s;
	}
		
	parametersSet=true;
}

function genOrders()	
{
    if(!parametersSet){
	    readFile();
	}
    var service = CreateService("GenerateDataService");
    var wfservice=CreateService("SimpleWorkflowService");
	var current=0;

	if(int(target)>=0){
		createPatients(service);
	}
	if(int(target)>=1){
		createVisits(service);
	}
	if(int(target)>=2){
		createOrders(service);
	}
	if(int(target)>=3){
		processOrders(wfservice,orders);
	}
	if(int(target)<0){
		LogError("Error took place, program exits");
	}
	if(verbose){
	    LogInfo("Done.");
	}
}
function readFile(){
	var SR;
    var aLine;
	var items=new Array("patientNumber","upto","modality","infoAuth","AdmissionType","Date","FacilityCode");
	var index;
	var valid=false;
    SR=File.OpenText(fileName);
    aLine=SR.ReadLine();
	
    while(aLine!=null)
    {
	  for (var item in items){
		if(aLine.IndexOf("//")==0){valid=true;break;}
		if (aLine.IndexOf(items[item])!=-1){
			valid=true;
	       index=aLine.IndexOf(":");
		   if(index==-1){
			   LogInfo(String.Format("Colon charactor is not found in input file. text:\" {0} \"",aLine));
			   target=-1;
			   break;
		   }
		   if(items[item]=="patientNumber"){		   
				patientNumber=parseInt(aLine.Remove(0,index+1));
		   }
		   if(items[item]=="upto"){
			   target=aLine.Remove(0,index+1);
		   }
		   if(items[item]=="infoAuth"){
			   infoAuthInput=aLine.Remove(0,index+1).Split([',']);
			   if(infoAuthInput.length==1&&infoAuthInput[0]==""){
				   infoAuthInput=null;
			   }
		   }
		   if(items[item]=="AdmissionType"){
			   if(aLine.Remove(0,index+1)=="E"){
			       emergency=true;
			   }
		   }
		   if(items[item]=="modality"){
			   modality=aLine.Remove(0,index+1);
		   }
		   if(items[item]=="Date"){
			   date=aLine.Remove(0,index+1);
		   }
		   if(items[item]=="FacilityCode"){
			   facility=aLine.Remove(0,index+1);
		   }
		}
	};
	if(!valid){
		LogInfo(String.Format("Invalid config item in \"{0}\"",aLine));
		target=-1;
		break;
	}
    aLine=SR.ReadLine();
	valid=false;
    }
    SR.Close();

}
function createPatients(service):Array{
			newPatients=new Array(patientNumber);
	        infoAuth=new Array(patientNumber);
			for(var p = 0; p < patientNumber ; p++)						
			{
				try
				{			
					// create a patient
					var patientResult = service.CreateRandomPatient({infoAuth:infoAuthInput  });
			
					newPatients[p] = patientResult.PatientProfile.PatientRef;
					infoAuth[p] = patientResult.PatientProfile.Mrn.AssigningAuthority;					
					
				}		
				catch(e)
				{
					LogError(e);target=-1;
				}
			}
			if(verbose){
			    LogInfo(String.Format("{0} patients are created",patientNumber));
			}
			return newPatients;
}
function createVisits(service):Array{
	visitResult=new Array();
	visitInfoAuth=new Array();
	visitCount=0;
	for(var i=0; i<patientNumber; i++){
		try{
		  for(var v = 0; v < maxVisitsPerPatient; v++)
			{   
				Probably(1/((v+1)*(v+1)),
					function()
					{
						var emerg:String[]=["E","Emergency"];
						var admissionType=null;
						
						// create a visit
						if(emergency){
							admissionType=emerg;
						}
						
						
						visitResult[visitCount] = service.CreateRandomVisit({
									PatientRef: newPatients[i],
									InformationAuthority: infoAuth[i],
									AdmissionType:admissionType
									});
						visitInfoAuth[visitCount]=infoAuth[i];
						
						visitCount++;
					});
			}
		}catch(e)
				{
					LogError(e);target=-1;
				}
	}
	if(verbose){
        LogInfo(String.Format("{0} visits are created.",visitCount));
    }
	return visitResult;
}

	
function createOrders(service):ArrayList{
	orders=new ArrayList();
	if(latEnum.Count==0){
		latEnum.Add(["N","None"]);
	}
	for(var i=0; i<visitCount; i++){
		if(Randomize && Random.NextDouble()<0.4)
			continue;
		try{
			var randomOrder=int(Math.pow(Random.NextDouble(),2)*maxOrdersPerVisit)+1;
			
			for(var o = 0; o <randomOrder ; o++)
						{	
						for ( var x in latEnum){							
							var newOrder=service.CreateRandomOrder({
												Visit: visitResult[i].Visit,
												InformationAuthority: visitInfoAuth[i],
												Modality: modality,
												FacilityCode:facility,
												ServiceName:diagnosticServiceName,
												Laterality:{Code:x[0],Value:x[1]}
									            });
							orders.Add(newOrder.Order);
							}
						}
		}catch(e)
				{

					LogError(e);target=-1;
				}
	}
	if(verbose){
	    LogInfo(String.Format("{0} orders are created",orders.Count));
	}
	return orders;
}

    
function processOrders(wfservice,orders){
	
	var performResults=new ArrayList();
	if(int(target)==3){
	        for ( var order in orders ){
                wfservice.Reschedule({OrderRef:order.OrderRef});
			} 
	}
	if(int(target)==4){
		var scheduledTime:DateTime = DateTime.Now;
		if(date!=null){
			scheduledTime=DateTime.Parse(date);				
		}

		for ( var order in orders ){
                wfservice.Reschedule({OrderRef:order.OrderRef,
				                      ScheduledTime:scheduledTime});
		}

	}
	
	if(int(target)>=5){
		
		for ( var order in orders ){
                wfservice.CheckIn({OrderRef:order.OrderRef});				                    
		}

	}

	if(int(target)>=6){
		
		
		for ( var order in orders ){
                performResults.Add(wfservice.Perform({ OrderRef:order.OrderRef}));				                    
		}
        
	}
	
	if(int(target)==7){
		
		
		for ( var perform in performResults ){
                wfservice.Report({ InterpretationStepRefs:perform.InterpretationStepRefs,
				                      Verify:false,
									  Publish:false});				                    
		}

	}
	if(int(target)==8){
		
		
		for ( var perform in performResults ){
                wfservice.Report({ InterpretationStepRefs:perform.InterpretationStepRefs,
				                      Verify:true,
									  Publish:true});				                    
		}			
	}
	if(int(target)==9){
		for ( var order in orders ){
                wfservice.Cancel({OrderRef:order.OrderRef});				                    
		}
	}
	if(verbose){
	   	LogInfo(target);
	}
	
}
function processOrder(wfservice,order){
    var orderList:ArrayList=new ArrayList();
    orderList.Add(order);
    processOrders(wfservice,orderList);
}

function timeShift(wfService,order,timeShift){

	wfService.TimeShift({
						OrderRef: order.OrderRef,
						Minutes: timeShift
						});
}
