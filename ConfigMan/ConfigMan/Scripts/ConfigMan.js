$(window).on("orientationchange", function () { initPage("orient") });
$(document).ready(function () { initPage("ready") });
function initPage(how) {
    //alert(how);
    $(".menu>li>a").on("click", function () { lightButton(this) });
    $(".menu2>li>a").on("click", function () { lightButton(this) });
    
    // alert("script tag");
    // var x = document.querySelector('.sy-switch').checked
    // alert(x);
    
    onlyOne(this);
    valueChanged();

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
    //alert("START - valueChanged");
    var doit = document.getElementsByClassName('sy-switch');
    // alert("valueChanged");
    if (doit.length > 0) {

        var x = document.querySelector('.sy-switch').checked
        // alert(x);

        if (x) {
            $(".sy-filter").show();
            $("#Filterstr")[0].value = "Y";
            //alert("set to Y");
        }
        else {
            $(".sy-filter").hide();            
        }
        
    }
    //alert("END - valueChanged");
    adaptUrls();

    return;
}

function adaptUrls() {
    //alert("Common");

    var fdoc = document.getElementById("Filterstr");
    if (fdoc != null) {
        var filterstr = fdoc.value;
        if ((filterstr == null) || (filterstr == "")) {
            filterstr = "N";
        }
    }
    if (filterstr == "N") {
        //alert("END -> No filterstr");
        return;
    }

    //alert("START - SetFilterUrl");
    var syurl = document.getElementsByClassName('sy-url');

    if (syurl.length > 0) { 
        // Adapt filter parameter that have to be passed to controllers via URL.      
            
        var url = "/Components?filterStr=" + filterstr;
        //alert(url);
        var part0 = "&subsetstr=" + $("#Subsetstr")[0].value;
        //alert(part0);
        var part1 = "&componentFilter=" + $("#ComponentFilter")[0].value;
        //alert(part1);
        var part2 = "&vendorFilter=" + $("#VendorFilter")[0].value;
        //alert(part2);
        var part3 = "&authFilter=" + $("#AuthFilter")[0].value;
        //alert(part3);
        var completeURL = url + part0 + part1 + part2 + part3;
        //alert(completeURL);
        $(".sy-url").attr("href", completeURL);      

    }    
    //alert("END - SetFilterUrl");
    
    //alert("START SetListUrl");       
    
    var sya = document.getElementsByClassName("sy-alist");

    if (sya.length > 0) {

        var part0 = "&subsetstr=" + $("#Subsetstr")[0].value;
        //alert(part0);
        var part1 = "&componentFilter=" + $("#ComponentFilter")[0].value;
        //alert(part1);
        var part2 = "&vendorFilter=" + $("#VendorFilter")[0].value;
        //alert(part2);
        var part3 = "&authFilter=" + $("#AuthFilter")[0].value;
        //alert(part3);
        
        var hid = document.getElementsByClassName("sy-hiddenid");
        // alert(sya.length);
        var teller = 1;
        var hidindex = 0;
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
            var s2 = "?id=" + hidden.toString();
            var ix = attr.indexOf(s1);
            if (ix == -1) {
                ix = attr.indexOf(s2);
            }
            var attrStripped = attr.substring(0, ix);
            if (i < 3) { 
                // alert(attrStripped);
            }

            var newattr = attrStripped + "?id=" + hidden.toString() + "&filterStr=Y" + part0 + part1 + part2 + part3;
            
            // alert("here?");
            $(sya[i]).attr("href", newattr);

            teller = teller + 1;

        }
    }
    //alert("END SetListUrl");

    //alert("START SetListUrl2");       
 
    var sya = document.getElementsByClassName("sy-alist2");
   
    if (sya.length > 0) {
        var kop = document.getElementsByClassName("sy-koppel");
        var gekop = document.getElementsByClassName("sy-gekoppeld");
        var oldid = document.getElementsByClassName("sy-oldid");

        var filt = "?filterstr="+ $("#Filterstr")[0].value;
        var part0 = "&subsetstr=" + $("#Subsetstr")[0].value;
        //alert(part0);
        var part1 = "&componentFilter=" + $("#ComponentFilter")[0].value;
        //alert(part1);
        var part2 = "&vendorFilter=" + $("#VendorFilter")[0].value;
        //alert(part2);
        var part3 = "&authFilter=" + $("#AuthFilter")[0].value;
        //alert(part3);
        var newid = $(".sy-hiddenid")[0].value;
        // alert(newid);
                
        for (let i = 0; i < sya.length; i++) {
            
            // alert(oldid[i].value);
            if (newid == oldid[i].value) {
                $(kop[i]).hide();
                $(gekop[i]).show();
            }
            else {
                $(kop[i]).show();
                $(gekop[i]).hide();
            }

            var attr = sya[i].getAttribute("href");
            // alert(attr);
            
            var s1 = "?";
            var ix = attr.indexOf(s1);
            var attrStripped = attr.substring(0, ix);
            
            var trail = attr.substring(ix + 1);

            var newattr = attrStripped + filt + part0 + part1 + part2 + part3 + "&" +  trail;
            if (i < 3) {
                //alert(attrStripped);
                //alert(newattr);
            }
            
            $(sya[i]).attr("href", newattr);
                      

        }
    }
    //alert("END SetListUrl2");
  
    //alert("START SetCreateUrl");
    
      
    var syc = document.getElementsByClassName("sy-acreate");

    if (syc.length > 0) {

        var part0 = "&subsetstr=" + $("#Subsetstr")[0].value;
        //alert(part0);
        var part1 = "&componentFilter=" + $("#ComponentFilter")[0].value;
        //alert(part1);
        var part2 = "&vendorFilter=" + $("#VendorFilter")[0].value;
        //alert(part2);
        var part3 = "&authFilter=" + $("#AuthFilter")[0].value;
        //alert(part3);
                
        for (let i = 0; i < syc.length; i++) {
            
            var attr = syc[i].getAttribute("href");
            // alert(attr);
            
            var s1 = "/Create";
            // alert(s1);            
            var newattr = attr;
            var f1 = attr.indexOf(s1);
            if (f1 > 0) {
                var repstring = "/Create?filterStr=Y" + part0 + part1 + part2 + part3;
                var attrStripped = attr.substring(0, f1);
                newattr = attrStripped + repstring;
                // alert(newattr);
            }
            else {
                var newattr = attr;
            }
            // alert("here?");
            $(syc[i]).attr("href", newattr);

        }
    }
    //alert("END SetCreateUrl");
   
}

/* dwing af dat er maar 1 box wordt gecheckt */
function onlyOne(current) {
    //alert("Geklikt op:" + current.id);
    var und = false;
    
    if (typeof current.id === 'undefined') {
        und = true;
    }
    //alert("START onlyOne");
    
    var others = document.querySelectorAll('.sy-checky');   
    //alert(others.length);
    if (others.length == 0) {
        //alert("END onlyOne (0)");
        return;
    }
    const urlletter = ["A", "Y", "N", "E"];
    if (!und) {
        /* alert(others.length); */

        for (var i = 0; i < others.length; i++) {
            if (others[i] !== current) {
                others[i].checked = false;
            }
            else {
                $("#Subsetstr")[0].value = urlletter[i];
                // alert("set to " + urlletter[i]);
            }
        }
    }
    else {
        var onefound = false;
        for (var i = 0; i < others.length; i++) {
            if (others[i].checked) {
                onefound = true;
            }
        }
        if (!onefound) {
            others[0].checked = true;
            $("#Subsetstr")[0].value = urlletter[0];
        }    
    }  
    
    adaptUrls();
    
    //alert("END onlyOne (1)");

}