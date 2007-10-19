/// scripting language is JScript.NET
/// variable name is "performedProcedureStep", of type ModalityPerformedProcedureStepSummary

var modalityName = performedProcedureStep.ModalityProcedureSteps[0].ModalityName;

if(modalityName == "CT")
    return "http://localhost/RIS/forms/technologist/ct-mpps.htm";
    
if(modalityName == "MRI")
    return "http://localhost/RIS/forms/technologist/mri-mpps.htm";
    
if(modalityName == "PET")
    return "http://localhost/RIS/forms/technologist/petct-mpps.htm";
    
if(modalityName == "Angiography" || modalityName == "Interventional" || modalityName == "Neuro Angiography")
    return "http://localhost/RIS/forms/technologist/angio-interv-mpps.htm";

if(modalityName == "General Radiography" || modalityName == "GI/GU")
    return "http://localhost/RIS/forms/technologist/cr-mpps.htm";
    
if(modalityName == "Breast Imaging")
    return "http://localhost/RIS/forms/technologist/mammo-mpps.htm";

if(modalityName == "Ultrasound" || modalityName == "Prostate Centre" || modalityName == "CEOU OB" || modalityName == "Hydro Gyn")
    return "http://localhost/RIS/forms/technologist/ultrasound-mpps.htm";
    
if(modalityName == "Nuclear Medicine")
    return "http://localhost/RIS/forms/technologist/nucmed-mpps.htm";

return "about:blank";
