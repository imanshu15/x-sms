var game;

$(document).ready(function () {
    clPreloader();
    game = $.connection.gameHub;
    $.connection.hub.start();
});

var clPreloader = function () {

    $("html").addClass('cl-preload');

    $("#loader").fadeOut("slow", function () {
        $("#preloader").delay(400).fadeOut("slow");
    });
    $("html").removeClass('cl-preload');
    $("html").addClass('cl-loaded');

};

function getAPIUrl() {
    return "http://localhost:1597/api/";
}