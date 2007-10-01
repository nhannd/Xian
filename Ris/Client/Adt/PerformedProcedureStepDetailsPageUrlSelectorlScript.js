/// scripting language is JScript.NET
/// variable name is "pps", of type ModalityPerformedProcedureStepSummary

var modalityId = pps.ModalityProcedureSteps[0].ModalityId;

if(modalityId == "10001")
    return "http://localhost/RIS/breastimaging.htm";
else
    return "http://localhost/RIS/nuclearmedicine.htm";
