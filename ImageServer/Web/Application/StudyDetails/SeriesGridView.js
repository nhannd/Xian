
function OpenSelectedSeries()
{
    list = @@GRIDVIEW_CONTROL_JS_OBJECT@@.SelectedRows.list;
    for(i=0; i<list.length; i++)
    {
        uid = list[i].getAttribute('seriesuid');
        
        url = '@@SERIES_DETAILS_PAGE_URL@@?seriesuid=' + uid + '&serverae=@@PARTITION_AE@@&studyuid=@@STUDY_INSTANCE_UID@@';
        window.open(url);
    }
}

function OnClick(row)
{
    
}