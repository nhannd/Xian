var sizeTotal;
var sizeUsed;

// This function calls the Web Service method.  
function LoadFilesystemInfo()
{
     path = document.getElementById('@@PATH_INPUT_CLIENTID@@').value;

     ClearCanvas.ImageServer.Web.Application.Services.FilesystemInfoService.GetFilesystemInfo(path, OnLoadFilesystemInfoSuccess, OnLoadFilesystemInfoError);
     
}
    
function RecalculateWatermark()
{
    hwtxt = document.getElementById('@@HW_PERCENTAGE_INPUT_CLIENTID@@').value;
    if (!isNaN(hwtxt))
    {
        hwpct = parseFloat(hwtxt) / 100.0;
        hwsize = hwpct * sizeTotal.value;
                                     
        HighWatermarkSize = document.getElementById('@@HW_SIZE_CLIENTID@@');
        HighWatermarkSize.value = FormatSize(hwsize);
    }   
    else
    {
        HighWatermarkSize.value = '';
    }

    lwtxt = document.getElementById('@@LW_PERCENTAGE_INPUT_CLIENTID@@').value;
    if (!isNaN(lwtxt))
    {
        lwpct = parseFloat(lwtxt) / 100.0;
        lwsize = lwpct * sizeTotal.value;
                                     
        LowWatermarkSize = document.getElementById('@@LW_SIZE_CLIENTID@@');
        LowWatermarkSize.value = FormatSize(lwsize);
    }   
    else
    {
        LowWatermarkSize.value = '';
    }
}

// Returns a string containing a percentage value formatted to 
// the number of decimal places specified by "decimalpoints"
function FormatPercentage(value, decimalpoints)
{
    var pct = new NumberFormat(value * 100.0);
    pct.setPlaces(decimalpoints);
    return pct.toFormatted() + '%';
}

// Returns a string formatted to the appropriate size (MB, GB, TB)
// provided a number that is the size in kilobytes (KB)
function FormatSize(sizeInKB)
{
        MB = 1024; //kb
        GB = 1024*MB;
        TB = 1024*GB;
        
        if (sizeInKB > TB)
        {
            var num = new NumberFormat(sizeInKB / TB);
            num.setPlaces(4);
            return num.toFormatted() + ' TB';
        }
        else if (sizeInKB > GB)
        {
            var num = new NumberFormat(sizeInKB/GB);
            num.setPlaces(3);
            return num.toFormatted() + ' GB';
        }
        else if (sizeInKB > MB)
        {
            var num = new NumberFormat(sizeInKB/MB);
            num.setPlaces(2);
            return num.toFormatted() + ' MB';
        }
        else
        {
            return  '' + sizeInKB + ' KB';
        }
}

function ValidationFilesystemPathParams()
{
    control = document.getElementById('@@PATH_INPUT_CLIENTID@@');
    params = new Array();
    params.path=control.value;

    return params;
}

function OnLoadFilesystemInfoError(result)
{
    alert('Error: ' + result.get_message());
}


// This is the callback function that
// processes the Web Service return value.
function OnLoadFilesystemInfoSuccess(result)
{
    filesysteminfo = result;
    if (filesysteminfo!=null && filesysteminfo.Exists)
    {
    
        sizeTotal = document.getElementById('@@TOTAL_SIZE_CLIENTID@@');
        sizeTotal.value = filesysteminfo.SizeInKB;
        
        sizeUsed = document.getElementById('@@USED_SIZE_CLIENTID@@');
        sizeUsed.value = sizeTotal.value - filesysteminfo.FreeSizeInKB;

        total = document.getElementById('@@TOTAL_SIZE_INDICATOR_CLIENTID@@');                                       
        total.innerHTML = FormatSize(sizeTotal.value);
        
        used = document.getElementById('@@USED_SIZE_INDICATOR_CLIENTID@@');
        used.innerHTML = FormatSize(sizeUsed.value) + ' (' + FormatPercentage(sizeUsed.value/sizeTotal.value, 4) +')';

        RecalculateWatermark();
    }
    else
    {
        sizeTotal = document.getElementById('@@TOTAL_SIZE_CLIENTID@@');
        sizeTotal.value = 0;
        
        sizeUsed = document.getElementById('@@USED_SIZE_CLIENTID@@');
        sizeUsed.value  = 0;
        
        total = document.getElementById('@@TOTAL_SIZE_INDICATOR_CLIENTID@@');                                       
        total.innerHTML = 'Unknown';
        
        used = document.getElementById('@@USED_SIZE_INDICATOR_CLIENTID@@');
        used.innerHTML = 'Unknown';
        
        RecalculateWatermark();
    }
    
}

