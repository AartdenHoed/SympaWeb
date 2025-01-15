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
            $("#FilterData_Filterstr")[0].value = "Y";
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

    var logic = "?";
    var where = document.getElementsByClassName("sy-component");
    if (where.length > 0) {
        logic = "Component";
    }
    var where = document.getElementsByClassName("sy-service");
    if (where.length > 0) {
        logic = "Service";
    }
    //alert(logic);
    //var blank = '%' + '2' + '0';
    //alert(blank)

    if (logic == "Component") {

        var sya2 = document.getElementsByClassName("sy-alist2");
        var kop = document.getElementsByClassName("sy-koppel");

        if ((sya2.length > 0) && (kop.length > 0)) {
            var kop = document.getElementsByClassName("sy-koppel");
            var gekop = document.getElementsByClassName("sy-gekoppeld");
            var oldid = document.getElementsByClassName("sy-oldid");

            var newid = $(".sy-hiddenid")[0].value;

            for (let i = 0; i < sya2.length; i++) {

                // alert(oldid[i].value);
                if (newid == oldid[i].value) {
                    $(kop[i]).hide();
                    $(gekop[i]).show();
                }
                else {
                    $(kop[i]).show();
                    $(gekop[i]).hide();
                }
            }
        }

        var fdoc = document.getElementById("FilterData_Filterstr");
        //alert(fdoc);
        if ((fdoc != null) && (typeof fdoc !== 'undefined')) {
            var filterstr = fdoc.value;
        }
        if ((filterstr == null) || (filterstr == "") || (typeof filterstr === 'undefined')) {
            filterstr = "N";
        }
        if (filterstr == "N") {
            //alert("END -> No filterstr");
            return;
        }
        //alert("filterstr = " + filterstr);

        var subset = "&subsetstrP=" + $("#FilterData_Subsetstr")[0].value;
        //alert(subset);
        var component = "&componentFilterP=" + encodeURIComponent($("#FilterData_ComponentFilter")[0].value);
        //alert(component);
        var vendor = "&vendorFilterP=" + encodeURIComponent($("#FilterData_VendorFilter")[0].value);
        //alert(vendor);
        var auth = "&authFilterP=" + $("#FilterData_AuthFilter")[0].value;
        //alert(auth);

        //alert("START - sy-url");

        var syurl = document.getElementsByClassName('sy-url');

        if (syurl.length > 0) {
            // Adapt filter parameter that have to be passed to controllers via URL.      

            var url = "/Components?filterStrP=" + filterstr;
            //alert(url);

            var completeURL = url + subset + component + vendor + auth;
            //alert(completeURL);
            $(".sy-url").attr("href", completeURL);

        }
        //alert("END - sy-url");

        //alert("START sy-alist");       

        var sya = document.getElementsByClassName("sy-alist");

        if (sya.length > 0) {

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

                var newattr = attrStripped + "?id=" + hidden.toString() + "&filterStrP=Y" + subset + component + vendor + auth;
                var newattr2 = newattr.replace(" ", "%20");
                // alert("here?");
                $(sya[i]).attr("href", newattr2);

                teller = teller + 1;

            }
        }
        //alert("END sy-alist");

        //alert("START sy-alist2");       

        var sya = document.getElementsByClassName("sy-alist2");

        if (sya.length > 0) {

            var filt = "?filterstrP=" + $("#FilterData_Filterstr")[0].value;


            for (let i = 0; i < sya.length; i++) {

                var attr = sya[i].getAttribute("href");
                // alert(attr);

                var s1 = "?";
                var ix = attr.indexOf(s1);
                var attrStripped = attr.substring(0, ix);

                var trail = attr.substring(ix + 1);

                var newattr = attrStripped + filt + subset + component + vendor + auth + "&" + trail;
                
                if (i < 3) {
                    //alert(attrStripped);
                    //alert(newattr);
                }

                $(sya[i]).attr("href", newattr);


            }
        }
        //alert("END sy-alist2");

        //alert("START sy-acreate");


        var syc = document.getElementsByClassName("sy-acreate");

        if (syc.length > 0) {


            for (let i = 0; i < syc.length; i++) {

                var attr = syc[i].getAttribute("href");
                // alert(attr);

                var s1 = "/Create";
                // alert(s1);            
                var newattr = attr;
                var f1 = attr.indexOf(s1);
                if (f1 > 0) {
                    var repstring = "/Create?filterStrP=Y" + subset + component + vendor + auth;
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
        //alert("END sy-acreate");
        return
    }

    if (logic == "Service") {
        var fdoc = document.getElementById("FilterData_Filterstr");
        //alert(fdoc);
        if ((fdoc != null) && (typeof fdoc !== 'undefined')) {
            var filterstr = fdoc.value;
        }
        //alert("filterstr = " + filterstr);
        if ((filterstr == null) || (filterstr == "") || (typeof filterstr === 'undefined')) {
            filterstr = "N";
        }
        if (filterstr == "N") {
            //alert("END -> No filterstr");
            return;
        } 
        //alert("here");

        var subset = "&subsetstrP=" + $("#FilterData_Subsetstr")[0].value;
        //alert(subset);
        var systeem = "&systeemfilterP=" + $("#FilterData_SysteemFilter")[0].value;
        //alert(systeem);
        var servicenaam = "&servicenaamfilterP=" + encodeURIComponent($("#FilterData_ServiceNaamFilter")[0].value); 
        //alert(servicenaam);
        var changestate = "&changestatefilterP=" + $("#FilterData_ChangeStateFilter")[0].value;
        //alert(changestatw);
        var component = "&componentfilterP=" + encodeURIComponent($("#FilterData_ComponentFilter")[0].value);
        //alert(component);
        var directory = "&directoryfilterP=" + encodeURIComponent($("#FilterData_DirectoryFilter")[0].value);
        //alert(directory);
        var template = "&templatefilterP=" + encodeURIComponent($("#FilterData_TemplateFilter")[0].value);
        //alert(template);
        var program = "&programfilterP=" + encodeURIComponent($("#FilterData_ProgramFilter")[0].value);
        //alert(program);

        //alert("START - sy-url");

        var syurl = document.getElementsByClassName('sy-url');

        if (syurl.length > 0) {
            // Adapt filter parameter that have to be passed to controllers via URL.      

            var url = "/Services?filterStrP=" + filterstr;
            //alert(url);

            var completeURL = url + subset + systeem + servicenaam + changestate + component + directory + template +  program;
            
            //alert(completeURL);
            $(".sy-url").attr("href", completeURL);

        }
        //alert("END - sy-url");

        //alert("START sy-alist");       

        var sya = document.getElementsByClassName("sy-alist");

        if (sya.length > 0) {

            var hid = document.getElementsByClassName("sy-hiddenid");
            var hid2 = document.getElementsByClassName("sy-hiddenid2");
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
                var hidden2 = encodeURIComponent(hid2[hidindex].value);

                var s1 = "/" + hidden.toString() + "?name=" + hidden2;
                var s2 = "?id=" + hidden.toString() + "&name=" + hidden2;
                var ix = attr.indexOf(s1);
                if (ix == -1) {
                    ix = attr.indexOf(s2);
                }
                var attrStripped = attr.substring(0, ix);
                if (i < 3) {
                    // alert(attrStripped);
                }

                var newattr = attrStripped + "?id=" + hidden.toString() + "&name=" + hidden2 +
                    "&filterStrP=Y" + subset + systeem + servicenaam + changestate + component + directory + template + program;

                //alert(newattr);
                
                $(sya[i]).attr("href", newattr);

                teller = teller + 1;

            }
        }
        //alert("END sy-alist");

        //alert("START sy-alist2");       

        var sya = document.getElementsByClassName("sy-alist2");

        if (sya.length > 0) {

            var filt = "?filterstrP=" + $("#FilterData_Filterstr")[0].value;


            for (let i = 0; i < sya.length; i++) {

                var attr = sya[i].getAttribute("href");
                // alert(attr);

                var s1 = "?";
                var ix = attr.indexOf(s1);
                var attrStripped = attr.substring(0, ix);

                var trail = attr.substring(ix + 1);

                var newattr = attrStripped + filt + subset + systeem + servicenaam + changestate + component + directory + template + program + "&" + trail;
                if (i < 3) {
                    //alert(attrStripped);
                    //alert(newattr);
                }
                
                $(sya[i]).attr("href", newattr);


            }
        }
        //alert("END sy-alist2");

    } 
   
}

/* dwing af dat er precies 1 box wordt gecheckt */
function onlyOne(current, fromwhere) {
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
    var urlletter = ["1", "2"];
    if (fromwhere == "Comp") {
        urlletter = ["A", "Y", "N", "E"];
    }
    else {
        urlletter = ["A", "G"];
    }
    if (!und) {
        /* alert(others.length); */

        for (var i = 0; i < others.length; i++) {
            if (others[i] !== current) {
                others[i].checked = false;
            }
            else {
                if (others[i].checked) {
                    $("#FilterData_Subsetstr")[0].value = urlletter[i];
                    // alert("set to " + urlletter[i]);
                }
                else {
                    others[0].checked = true;
                    $("#FilterData_Subsetstr")[0].value = urlletter[0];
                }
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
            $("#FilterData_Subsetstr")[0].value = urlletter[0];
        }    
    }  
    
    adaptUrls();
    
    //alert("END onlyOne (1)");

}