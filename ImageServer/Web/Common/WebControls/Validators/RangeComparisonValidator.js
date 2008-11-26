// This script defines the client-side validator extension class @@CLIENTID@@_ClientSideEvaludator
// to validate an input within a specified range and is greater or less than another input.
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
            '@@INVALID_INPUT_INDICATOR_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_CLIENTID@@'),
            '@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@'),
            '@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@'),
            '@@IGNORE_EMPTY_VALUE@@'
    );
}


// override BaseClientValidator.OnEvaludate() 
// This function is called to evaluate the input
@@CLIENTID@@_ClientSideEvaluator.prototype.OnEvaluate = function()
{
    result = BaseClientValidator.prototype.OnEvaluate.call(this);
    
    if (result.OK==false)
    {
        return result;
    }
        
    compareCtrl = document.getElementById('@@COMPARE_INPUT_CLIENTID@@');
   
    result = new ValidationResult();
    if (this.input.value!=null && this.input.value!='')
    {
        controlValue = parseNumber(this.input.value);
        if (compareCtrl!=null && compareCtrl.value!='' &&  !isNaN(controlValue))
        {
            compareValue = parseNumber(compareCtrl.value);
            result.OK = controlValue >= @@MIN_VALUE@@ && controlValue<= @@MAX_VALUE@@ && controlValue @@COMPARISON_OP@@ compareValue;
            
            if (result.OK == false)
            {
                if ('@@ERROR_MESSAGE@@' == null || '@@ERROR_MESSAGE@@'=='')
                {
                    if ('@@COMPARISON_OP@@' == '>=')
                        result.Message = '@@INPUT_NAME@@ must be between @@MIN_VALUE@@ and @@MAX_VALUE@@ and greater than @@COMPARE_TO_INPUT_NAME@@';
                    else
                        result.Message = '@@INPUT_NAME@@ must be between @@MIN_VALUE@@ and @@MAX_VALUE@@ and less than @@COMPARE_TO_INPUT_NAME@@';
                }
                else
                    result.Message = '@@ERROR_MESSAGE@@';
                
            }
        }
        else
        {
            result.OK = false;
            result.Message = '@@INPUT_NAME@@ is not a valid number';
        }
    }   
    else
    {
          result.OK = false;  
    }
    
    
    
    return result;

};
