// This script defines the client-side validator extension class @@CLIENTID@@_ClientSideEvaludator
// to validate an input against a regular expression.
//
// This class defines how the validation is carried and what to do afterwards.


// derive this class from BaseClientValidator
ClassHelper.extend(@@CLIENTID@@_ClientSideEvaluator, BaseClientValidator);

//constructor
function @@CLIENTID@@_ClientSideEvaluator()
{
    BaseClientValidator.call(this, 
            '@@INPUT_CLIENTID@@',
            '@@INPUT_NORMAL_BKCOLOR@@',
            '@@INPUT_INVALID_BKCOLOR@@',
            '@@INPUT_NORMAL_BORDERCOLOR@@',
            '@@INPUT_INVALID_BORDERCOLOR@@',
            '@@INPUT_NORMAL_CSS@@',
            '@@INPUT_INVALID_CSS@@',                                    
            '@@INVALID_INPUT_INDICATOR_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_CLIENTID@@'),
            '@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@'),
            '@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@'),
            @@IGNORE_EMPTY_VALUE@@,
            '@@CONDITION_CHECKBOX_CLIENTID@@'=='null'? null:document.getElementById('@@CONDITION_CHECKBOX_CLIENTID@@'),
            @@VALIDATE_WHEN_UNCHECKED@@
    );
    
    this.regex = new RegExp('@@REGULAR_EXPRESSION@@');
    
    this.isRequired = @@REQUIRED@@;
}

// override BaseClientValidator.OnEvaludate() 
// This function is called to evaluate the input
@@CLIENTID@@_ClientSideEvaluator.prototype.OnEvaluate = function()
{
    result = BaseClientValidator.prototype.OnEvaluate.call(this);
    
    if (!result.OK || result.Skipped)
    {
        return result;
    }
    
    if (this.input.value=='' && this.isRequired)
    {
        result.OK = false;
        result.Message = 'Required';
    }
    
    else{
        if (this.input.value.match(this.regex)) {
            result.OK = true;
        } else {
            result.OK = false;
        }

        if (result.OK == false)
        {
            if ('@@ERROR_MESSAGE@@' == null || '@@ERROR_MESSAGE@@'=='')
            {
                result.Message = 'Invalid format';
            }
            else
                result.Message = '@@ERROR_MESSAGE@@';
            
        }  
    }
    
 
    
    return result;
};

//@@CLIENTID@@_ClientSideEvaluator.prototype.OnValidationPassed = function()
//{
//    //alert('Length validator: input is valid');
//    BaseClientValidator.prototype.OnValidationPassed.call(this);
//}

//@@CLIENTID@@_ClientSideEvaluator.prototype.OnValidationFailed = function(error)
//{
//    //alert('Length validator: input is valid : ' + error);
//    BaseClientValidator.prototype.OnValidationFailed.call(this, error);
//        
//}

//@@CLIENTID@@_ClientSideEvaluator.prototype.SetErrorMessage = function(result)
//{
//    BaseClientValidator.prototype.SetErrorMessage.call(this, result);
//    alert(result.Message);
//}

