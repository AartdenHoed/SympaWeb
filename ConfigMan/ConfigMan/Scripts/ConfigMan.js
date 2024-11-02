$(window).on("orientationchange", function () { initPage("orient") });
$(document).ready(function () { initPage("ready") });
function initPage(how) {
    // alert(how);
    $(".menu>li>a").on("click", function () { lightButton(this) });
    $(".menu2>li>a").on("click", function () { lightButton(this) });
    
    // alert("script tag");
    // var x = document.querySelector('.sy-switch').checked
    // alert(x);
    valueChanged();
    SetFilterUrl("0");   

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
    var doit = document.getElementsByClassName('sy-switch');
    // alert("valueChanged");
    if (doit.length > 0) {

        var x = document.querySelector('.sy-switch').checked
        // alert(x);

        if (x) {
            $(".sy-filter").show();
        }
        else {
            $(".sy-filter").hide();
        }
    }
    return;
}
function SetFilterUrl(what) {

    // alert("SetFilterUrl");
    var doit = document.getElementsByClassName('sy-url');

    if (doit.length > 0) { 
        // Adapt filter parameter that have to be passed to controllers via URL. 
        var yesorno = $("#Filterstr")[0].value;
        if ((what == "Y") || (yesorno == "Y")) {
            var f = "Y";
        }
        else {
            var f = "N";
        }
    
        var url = "/Components?filterStr=" + f;
        //alert(url);
        var part1 = "&componentFilter=" + $("#ComponentFilter")[0].value;
        //alert(part1);
        var part2 = "&vendorFilter=" + $("#VendorFilter")[0].value;
        //alert(part2);
        var part3 = "&authFilter=" + $("#AuthFilter")[0].value;
        //alert(part3);
        var completeURL = url + part1 + part2 + part3;
        // alert(completeURL);
        $(".sy-url").attr("href", completeURL);

        SetListUrl();
        SetCreateUrl();

    }
    return;

}

function SetListUrl() {
    // alert("SetListUrl");

    var filterstr = $("#Filterstr")[0].value;
    // alert(filterstr);
    if (filterstr == "N") {        
        return;
    }

    var sya = document.getElementsByClassName("sy-alist");

    if (sya.length > 0) {

        var part1 = "&componentFilter=" + $("#ComponentFilter")[0].value;
        //alert(part1);
        var part2 = "&vendorFilter=" + $("#VendorFilter")[0].value;
        //alert(part2);
        var part3 = "&authFilter=" + $("#AuthFilter")[0].value;
        //alert(part3);
        
        var hid = document.getElementsByClassName("sy-hiddenid");
        // alert(sya.length);
        var teller = 1;
        var hidindex = 0
        for (let i = 0; i < sya.length; i++) {
            //for (let i = 0; i < 4; i++) {
            // alert("in loop");

            if (teller > 3) {
                teller = 1;
                hidindex = hidindex + 1;
            }
            var attr = sya[i].getAttribute("href");
            // alert(attr);
            var hidden = hid[hidindex].value;

            var s1 = "/" + hidden.toString();
            // alert(s1);
            var s2 = "?id=" + hidden.toString();
            // alert(s2); 
            var repstring = "?id=" + hidden.toString() + "&filterStr=Y" + part1 + part2 + part3;
            var f2 = attr.indexOf(s2);
            if (f2 > 0) {
                newattr = attr.replace(s2, repstring);
                // alert(newattr);
            }
            else {
                var f1 = attr.indexOf(s1);
                if (f1 > 0) {
                    newattr = attr.replace(s1, repstring);
                    // alert(newattr);
                }
            }
            // alert("here?");
            $(sya[i]).attr("href", newattr);

            teller = teller + 1;

        }
    }
    return;
    
}
function SetCreateUrl() {
    // alert("SetCreateUrl");

    var filterstr = $("#Filterstr")[0].value;
    // alert(filterstr);
    if (filterstr == "N") {
        return;
    }

    var syc = document.getElementsByClassName("sy-acreate");

    if (syc.length > 0) {

        var part1 = "&componentFilter=" + $("#ComponentFilter")[0].value;
        //alert(part1);
        var part2 = "&vendorFilter=" + $("#VendorFilter")[0].value;
        //alert(part2);
        var part3 = "&authFilter=" + $("#AuthFilter")[0].value;
        //alert(part3);
                
        for (let i = 0; i < syc.length; i++) {
            
            var attr = syc[i].getAttribute("href");
            // alert(attr);
            
            var s1 = "/Create?Length=0" 
            // alert(s1);            
            var newattr = attr;
            var f1 = attr.indexOf(s1);
            if (f1 > 0) {
                var repstring = "/Create?filterStr=Y" + part1 + part2 + part3;
                newattr = attr.replace(s1, repstring);
                // alert(newattr);
            }
            else {
                var newattr = attr;
            }
            // alert("here?");
            $(syc[i]).attr("href", newattr);

        }
    }
    return;

}