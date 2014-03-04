

$(function () {
    console.log("hey");

    $.mobile.loading("show", {
        text: "loading",
        textVisible: true,
        theme: "z",
        html: ""
    });
    getAll();

});

function getAll() {
    $.ajax("api/build", function (data) {

    })
   .done(function (data) {
       // alert("SUCCESS Getting All");
       var response = data;

       $.each(response, function (i, val) {
           var img = new Image(160, 120);
           img.src = "media/_thumb.jpg";
           img.onerror = imgError;
           var link = $("<li/>").html("<a href='#' onclick='getItem(" + val.applicationID + ")'><img src='media/" + val.applicationID + "_preview.jpg" + "'/>" + val.title + "</a>by: " + val.email);


           $("#mainList").append(link);

       });
       $("#mainList").listview("refresh");
       $.mobile.loading("hide");
   })

   .error(function () {
       alert("nothing to worry about");
   })
}


function imgError(evt){
   this.src = "media/10_thumb.jpg";
}
function getItem(id) {
    window.open("ItemViewer.html?id=" + id, "_self");
    //$("#slider div").remove();
    //$.getJSON("api/item/" + id, null, function (data) {
    //    var d = data;
    //    $.each(data, function (index, d) {
    //        var wholeDiv = $("<div/>").css("border", "2px solid blue");// create the div
    //        var media = (d.type == "image") ? $("<div/>").append($("<img/>").attr({ "src": "media/" + d.fileName, "alt": d.title })) : $("<div/>").append($("<video/>").attr({ "src": "media/" + d.fileName, "type": "video/mp4", "alt": d.title, "width": "480px", "height": "360px" }))
    //        wholeDiv.append($("<h3/>").html(d.title));
    //        wholeDiv.append(media);
    //        wholeDiv.append($("<p/>").html(d.caption));
    //        $("#slider").append(wholeDiv);
    //    });
    //});

}