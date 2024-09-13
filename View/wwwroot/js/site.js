$(document).ready(function () {
    $("#brandC").removeAttr("style");
    var changeMode = function (e) {
        $("html").addClass("change");
        var mode = $("html").attr("data-bs-theme");
        var evalue = true;
        if (mode == "dark") {
            mode = "light";
            evalue = false;
        } else {
            mode = "dark";
            evalue = true;
        }
        $("html").attr("data-bs-theme", mode);
        localStorage.setItem("DarkMode",mode);
        $(e).prop('checked', evalue);
        setTimeout(function () {
            $("html").removeClass("change");
        }, 1100);
        
    }
    var systemMode = function () {
        var mode = localStorage.getItem("DarkMode");
        var evalue = true;
        if (mode == null) {
            mode = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        }
        if (mode == "dark") {
            mode = 'dark';
            evalue = true;
        } else {
            mode = 'light';
            evalue = false;
        }

        $("html").attr("data-bs-theme", mode);
        $("#modeChanger").prop('checked', evalue).click(function () {
            changeMode(this);
        });
    }
    systemMode();
    $('a[href]:not([href^="#"])').click(function (event) {
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