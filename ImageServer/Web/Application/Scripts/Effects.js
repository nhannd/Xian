$(document).ready(function(){
    $(".SearchTextBox").mouseover(function() { $(this).addClass('hover'); }).mouseout(function() { $(this).removeClass('hover'); }).focus( function() {	$(this).addClass('focus'); }).blur( function() { $(this).removeClass('focus') });
    $(".SearchDateBox").mouseover(function() { $(this).addClass('hover'); }).mouseout(function() { $(this).removeClass('hover'); }).focus( function() {	$(this).addClass('focus'); }).blur( function() { $(this).removeClass('focus') });
});
