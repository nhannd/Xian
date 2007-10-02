/// scripting language is JScript.NET
/// variable name is "performedProcedureStep", of type ModalityPerformedProcedureStepSummary

var modalityId = performedProcedureStep.ModalityProcedureSteps[0].ModalityId;

if(modalityId == "10001")
    return "http://localhost/RIS/breastimaging.htm";
else
    return "http://localhost/RIS/nuclearmedicine.htm";
