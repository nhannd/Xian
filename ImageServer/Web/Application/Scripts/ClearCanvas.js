function InputHover() {
$(document).ready(function(){
    $(".SearchTextBox").mouseover(function() { $(this).addClass('TextInputHover'); }).mouseout(function() { $(this).removeClass('TextInputHover'); }).focus( function() {	$(this).addClass('TextInputFocus'); }).blur( function() { $(this).removeClass('TextInputFocus') });
    $(".SearchDateBox").mouseover(function() { $(this).addClass('DateInputHover'); }).mouseout(function() { $(this).removeClass('DateInputHover'); }).focus( function() {	$(this).addClass('DateInputFocus'); }).blur( function() { $(this).removeClass('DateInputFocus') });
    $(".GridViewTextBox").mouseover(function() { $(this).addClass('TextInputhover'); }).mouseout(function() { $(this).removeClass('TextInputHover'); }).focus( function() {	$(this).addClass('TextInputFocus'); }).blur( function() { $(this).removeClass('TextInputFocus') });
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
