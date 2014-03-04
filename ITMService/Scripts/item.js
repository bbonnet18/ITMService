var COLL = [];// a collection of resources from the server


$(function () {
    // set jQuery mobile globals
    $.mobile.loading("show", {
        text: "loading",
        textVisible: true,
        theme: "z",
        html: ""
    });

    
    shareurl = window.location.href;
    var idStr = shareurl.split('?')[1];
    idStr = idStr.split('=')[1];

    $("#tocPage").page({ theme: "b" });

    // check the status of the logged in user, if the user is logged into Yammer, then make the call

    getItem(idStr);



})



 


function getItem(id) {
    //$("#slider div").remove();

    $.getJSON("api/item/" + id, null, function (data) {

        COLL = data;
        var d = data;

        if (data.length != 0) {
            $.each(data, function (index, d) {
                // create the list item and attributes
                var itemLink = $("<li/>").append($("<a/>").attr({ "href": "#page" + (eval(index) + 1) }).html(d.title));
                $("#mainList").append(itemLink);
            });

            $("#mainList").listview("refresh");// refresh the list so items show
        } else {
            // tell them that it's empty
        }



    })

    .done(function (data) {
        $.mobile.loading("hide");
    });

}

function share() {
    var winURL = shareurl;
    window.open("mailto:bonnet_ben@bah.com?subject=See%20my%20build&body="+winURL, "_self");
}


$(document).bind('pagebeforechange', function (event, data) {
    var url = $.mobile.path.parseUrl(data.toPage).hash;
    if (url != undefined && url.length > 5 && url.substring(0, 5) == "#page") {

        $.mobile.loading({ text: "Loading..." });
        var id = url.substring(5);
        var index = eval(id) - 1;
        $("#pageTemplate h1").html("page " + id);
        var title = COLL[index].title;
        var caption = COLL[index].caption;
        var type = COLL[index].type;
        var mediaURL = "media/" + COLL[index].fileName;
        $("#mainMedia div").remove();
        var mediaElement = (type == "image") ? $("<div/>").append($("<img/>").attr({ "src": mediaURL, "alt": title, "width": "250px", "height": "250px", alt:"" })).addClass("mediaDiv") : $("<div/>").append($("<video/>").attr({ "src": mediaURL, "type": "video/mp4", "alt": title, "width": "280px", "height": "210px", "controls": true, alt:"" })).addClass("mediaVideo")
        //mediaElement.addClass("mediaDiv");
        $("#pageTemplate h3").html(title);
        $("#pageTemplate #content #caption").html(caption);
        $("#pageTemplate #content #mainMedia").append(mediaElement);

        var prevID = (eval(id) - 1 == 0) ? "#page1" : "#page" + (eval(id) - 1);
        var next = eval(id) + 1;
        var nextID = (next == COLL.length) ? "#page" + COLL.length : "#page" + next;
        $("#back").attr({ "href": prevID });
        $("#next").attr({ "href": nextID });

        $.mobile.changePage($("#pageTemplate"), { dataUrl: data.toPage });
        event.preventDefault();
    }
});



//$(document).bind('pagebeforechange', function (event, data) {
//    var url = $.mobile.path.parseUrl(data.toPage).hash;
//    if (url != undefined && url.length > 5 && url.substring(0, 5) == "#page") {


//        var id = url.substring(5);
//        var index = eval(id) - 1;
//        $("#pageTemplate h1").html("page " + id);
//        var title = COLL[index].title;
//        var caption = COLL[index].caption;
//        var type = COLL[index].type;
//        var mediaURL = "media/" + COLL[index].fileName;
//        $("#mainMedia div").remove();
//        var mediaElement = (type == "image") ? $("<div/>").append($("<img/>").attr({ "src": mediaURL, "alt": title, "width": "250px", "height": "250px" })) : $("<div/>").append($("<video/>").attr({ "src": mediaURL, "type": "video/mp4", "alt": title, "width": "280px", "height": "210px", "controls": true }))
//        mediaElement.addClass("mediaDiv");
//        $("#pageTemplate h3").html(title);
//        $("#pageTemplate #content #caption").html(caption);
//        $("#pageTemplate #content #mainMedia").append(mediaElement);

//        var prevID = (eval(id) - 1 == 0) ? "#page1" : "#page" + (eval(id) - 1);
//        var next = eval(id) + 1;
//        var nextID = (next == COLL.length) ? "#page" + COLL.length : "#page" + next;
//        $("#back").attr({ "href": prevID });
//        $("#next").attr({ "href": nextID });
  
//        $.mobile.changePage($("#pageTemplate"), { dataUrl: data.toPage });
//        event.preventDefault();
//    }
//});
