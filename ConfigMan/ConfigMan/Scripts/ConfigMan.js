$(window).on("orientationchange", function () { initPage("orient") });
$(document).ready(function () { initPage("ready") });
function initPage(how) {
    // alert(how);
    $(".menu>li>a").on("click", function () { lightButton(this) });
    $(".menu2>li>a").on("click", function () { lightButton(this) });
    

    var setWidth = window.innerWidth;
    var setHeight = window.innerHeight;
    var pxWidth = String(setWidth) + "px";
    var pxHeight = String(setHeight) + "px";

    var ourBody = document.getElementById("wrapper");
    ourBody.style.width = pxWidth;
    ourBody.style.height = pxHeight;

    if (how != "resize") {
        var canvas = document.getElementById("sympaSprite");
        var ctx = canvas.getContext("2d");
        var w = canvas.width;
        // alert(w);
        var h = canvas.height;
        // alert(h);
        ctx.clearRect(0, 0, w, h);

        ctx.font = "75px Comic Sans MS";
        ctx.fontWeight = "1200";
        ctx.textAlign = "center";
        var gradient = ctx.createLinearGradient(.1 * w, .9 * h, .9 * w, .1 * h);
        gradient.addColorStop(0, "red");
        gradient.addColorStop(0.4, "orange");
        gradient.addColorStop(0.8, "yellow");
        ctx.fillStyle = gradient;
        ctx.rotate(-Math.PI / 8);
        ctx.fillText("SYMPA", 0.4 * w, 1 * h);
    } 

    return;
    
}
function lightButton(e) {       
    
    e.parentElement.style.backgroundColor = "Yellow";
    e.style.fontSize = "smaller";
    e.style.color = "red";
    e.style.fontWeight = "bold"
   

    e.parentElement.style.backgroundSize = "cover";
    e.parentElement.style.backgroundRepeat = "no-repeat";
    e.parentElement.style.backgroundPosition = "center";
    e.parentElement.style.backgroundImage = "url(/Images/Button.png)";
    //e.parentElement.style.background = "Yellow url('/Images/LightButton.png') no-repeat center cover";
    
    setTimeout(function () { var x = 1; }, 1000);
}

function valueChanged()
{
    // alert("yes");
    var x = document.querySelector('.sy-switch').checked
    // alert(x);

    if (x) {
        $(".sy-filter").show();
    }
    else {
        $(".sy-filter").hide();
    }
}
