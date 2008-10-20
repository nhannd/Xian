function @@CLIENTID@@_ShowOtherNameFormats()
{
    var row=$get('@@PHONETIC_ROW_CLIENTID@@');
    row.style.visibility='visible';
    if (/MSIE (\d+\.\d+);/.test(navigator.userAgent))
    { 
        row.style.display='block';
    }
    else 
    { 
        row.style.display='table-row';
    }

    row=$get('@@IDEOGRAPHY_ROW_CLIENTID@@');
    row.style.visibility='visible';
    if (/MSIE (\d+\.\d+);/.test(navigator.userAgent))
    { 
        row.style.display='block';
    }
    else 
    { 
        row.style.display='table-row';
    }
}
