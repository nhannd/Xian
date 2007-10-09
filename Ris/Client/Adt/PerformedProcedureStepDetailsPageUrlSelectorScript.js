/// scripting language is JScript.NET
/// variable name is "performedProcedureStep", of type ModalityPerformedProcedureStepSummary

var modalityName = performedProcedureStep.ModalityProcedureSteps[0].ModalityName;

if(modalityName == "CT")
    return "http://localhost/RIS/forms/technologist/ct-mpps.htm";
    
if(modalityName == "MRI")
    return "http://localhost/RIS/forms/technologist/mri-mpps.htm";
    
if(modalityName == "PET")
    return "http://localhost/RIS/forms/technologist/petct-mpps.htm";
    
return "about:blank";
