/*
 $(document).ready(function () {
            ITM = ITMCreate({ name: "ITM" });// create the ITM object

            ITM.checkYammerStatus(function (resp) {

                checkExists();
            });



        });
        // check to see if the user exists
        function checkExists() {

            ITM.checkUserExists(function (check) {
                if (!check.userID) {// the userID reflects that the return value is a user's creds 
                    var user = ITM.getCredentials();
                    ITM.registerUser(check.fName, check.lName, check.email, function (data) {
                        alert("had to add new user" + ": " + data.lName + "," + data.fName + " - " + data.email);
                    });
                } else {// user existed, so show the data about the user
                    alert("user existed:" + check.userID + ":" + check.lName + "," + check.fName + " - " + check.email);
                }

            });
        }
*/



var COLL = [];// set the array for the content
var currentSelection = 0;
$(function () {
   
    ITM = ITMCreate({ name: "ITM" });// create the ITM object

    ITM.checkYammerStatus(function (resp) {

        checkExists();
    });




    //$.ajax("api/User", function (data) {

    //})

    //.done(function(data){
    //    var user = data.response;
    //    var role = data.role.trim();
    //    if (role == "admin") {
    //        getAll();
    //    }
    //})
    //.error(function(){
    //    alert("there was an error retrieving the user information");
    //})
});

function checkExists() {

    ITM.checkUserExists(function (check) {
        if (!check.userID) {// the userID reflects that the return value is a user's creds 
            var user = ITM.getCredentials();
            var userProps = ITM.getUserProperties();
			// check if it exists, then if it doesn't, make the user enter a password
			//-- setup a popup with an input so the user can enter a password, then use the registration
			//
            ITM.registerUser(userProps.fName, userProps.lName, userProps.email, function (data) {
                if (data.code == "1") {
                    alert("Error: " + data.error);
                } else {
                    alert("had to add new user" + ": " + data.lName + "," + data.fName + " - " + data.email);
                }
               
            });
        } else {// user existed, so show the data about the user
            //alert("user existed:" + check.userID + ":" + check.lName + "," + check.fName + " - " + check.email);
            $("#firstName").html("<strong>First: </strong>"+check.fName);
            $("#lastName").html("<strong>Last: </strong>" + check.lName);
            $("#email").html("<strong>Email: </strong>" + check.email);
            getAll();
        }

    });
}

function getAll() {
    $.ajax({
        url: "api/build",
        method: "GET",
        beforeSend: function (req) {
            var creds = ITM.getCredentials();
            var up = btoa(creds + ":" + creds);//Base64("bbonnet18:october17");
            req.setRequestHeader("Authorization", "Basic   " + up);
        }

    })
   .done(function (data) {
       // alert("SUCCESS Getting All");
       var response = data;
       COLL = data;// sets this to the full set

       $("#mainList").empty();
       $.each(response, function (i, val) {
           var img = new Image(160, 120);
           img.src = "media/_thumb.jpg";
           img.onerror = imgError;
           var link = $("<li/>").html("<a href='#' onclick='getItem(" + val.applicationID + ")'><img src='media/" + val.buildID + "_thumb.jpg" + "'/>" + val.title + "</a>by: " + val.email + " | " + val.active + " <a href='#changeStatusDialog' data-rel='dialog' onclick='setCurrent("+val.applicationID+")'>Activate</a>");//onclick='activate("+val.applicationID+")'


           $("#mainList").append(link);

       });
       $("#mainList").listview("refresh");

   })

   .error(function () {
       alert("nothing to worry about");
   })
}



function imgError(evt) {
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

function setCurrent(id) {
    var itemTitle = null;
    for (var i = 0; i < COLL.length; i++) {
        if (COLL[i].applicationID == id) {
            itemTitle = COLL[i].title;
        }
    }
    currentSelection = id;
    $("#msgText").text("title: "+itemTitle);

}



function activate() {
    $.ajax("api/Build/"+currentSelection, function (data) {

    })

    .done(function (data) { 
            getAll();
    })
    .error(function () {
        alert("there was an error setting the status");
    })
}