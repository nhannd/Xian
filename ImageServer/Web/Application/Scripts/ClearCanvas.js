function InputHover() {
$(document).ready(function(){
    $(".SearchTextBox").mouseover(function() { $(this).addClass('hover'); }).mouseout(function() { $(this).removeClass('hover'); }).focus( function() {	$(this).addClass('focus'); }).blur( function() { $(this).removeClass('focus') });
    $(".SearchDateBox").mouseover(function() { $(this).addClass('hover'); }).mouseout(function() { $(this).removeClass('hover'); }).focus( function() {	$(this).addClass('focus'); }).blur( function() { $(this).removeClass('focus') });
    $(".GridViewTextBox").mouseover(function() { $(this).addClass('hover'); }).mouseout(function() { $(this).removeClass('hover'); }).focus( function() {	$(this).addClass('focus'); }).blur( function() { $(this).removeClass('focus') });
});
}

function UserInformationLink_Hover(objectID) {
    $(document).ready(function() {
        $("#" + objectID).hover(
            function() { $(this).css("text-decoration", "underline"); }, 
            function() { $(this).css("text-decoration", "none"); 
        });
    });
}
