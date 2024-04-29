$(document).ready(function () {
    $('a').click(function (event) {
        event.preventDefault();
        var target = $(this).attr('target');
        var href = $(this).attr('href');
        if (target === '_blank') {
            window.open(href, '_blank');
        } else {
            $("body div main").fadeOut(500, function () {
                window.location.href = href;
            });
        }
    });
    $("svg, svg *").fadeIn(100);
    $("body div main").fadeIn(500);
});