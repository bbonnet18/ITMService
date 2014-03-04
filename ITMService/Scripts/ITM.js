// this file is dependent on Yammer - make sure to load the Yammer JS file on any page that
// uses this code


var ITMCreate = function(ITMobj){
    var itm = {};// create object to hold accessor methods

    // private method to set the credentials
    var setCredentials = function (user, pass) {
        ITMobj.username = user;
        ITMobj.password = pass;
    }
    // this is used for access to the username
    var getCreds = function () {
        return ITMobj.username;
    }

    var setUserProperties = function (lName, fName, email, role) {
        ITMobj.lName = lName;
        ITMobj.fName = fName;
        ITMobj.email = email;
        ITMobj.role = role;
    }

    // set methods on object to manipulate passed in object
    itm.getCredentials = function () {
        return getCreds();
    }
    // set the user properties for the object
    itm.setUserProperties = function (lName,fName,email,role) {
        setUserProperties(lName, fName, email, role)
    }
    // return the user properties as an object
    itm.getUserProperties = function () {
        return { "lName": ITMobj.lName, "fName": ITMobj.fName, "email": ITMobj.email };
    }

    itm.checkYammerStatus = function (statusCallback) {
        // check Yammer login status 
        var authLogin = yam.getLoginStatus(function (resp) {
            if (resp.authResponse) {// this means the user is authenticated
                // set the local user/pw so the ITM calls can happen

                if (statusCallback && typeof statusCallback === "function") {// check that the status callback is there

                    for (var s = 0; s < resp.user.contact.email_addresses.length; s++) {
                        var useremail = resp.user.contact.email_addresses[s].address;
                        if (useremail.indexOf("bah.com")) {
                            tempemail = useremail;
                            break;
                        } else {
                            var test2;
                        }
                    }
                    if (tempemail != null) {
                        setCredentials(tempemail, tempemail);// set these for any attempts on the ITMGo API
                        var last = resp.user.last_name;
                        var first = resp.user.first_name;
                        itm.setUserProperties(last, first, tempemail);
                        statusCallback(resp);
                    } else {
                        return null;
                    }
                }
                // call the callbackfunction

            } else {
                window.location = "https://itmgo.com/login.html";
                return null;
            }
        });

    }

    itm.checkUserExists = function (existsCallback) {
        // check the ITM service to see if the user exists, if so, return the userID 
        // if the user does not exist, register the user and return the userID
        
        var creds = getCreds();

        $.ajax("api/user/"+creds, function (data) {

        })
         .done(function (data) {
       // alert("SUCCESS Getting All");
             var response = data;
            setUserProperties(data.lName, data.fName, data.email, data.role);
            existsCallback(response);
      
        })

        .error(function (data) {
            alert("nothing to worry about"+data);
        })
    }
    // attempt to register the user and return the new user or the error data
    itm.registerUser = function (fnameVar,lnameVar,emailVar,registerCallback) {
        var user = itm.getCredentials();
            $.post("api/User/register", { userName: user, fName: fnameVar, lName: lnameVar, email: emailVar, password: user }, function (data) {
                setCredentials(data.email, data.email);// the attempt was successful, so now you can set the credentials to this data
                setUserProperties(data.lName, data.fName, data.email, data.role);
                registerCallback(data);
            })

           .error(function (data) {
               registerCallback({ "error": "Could not register user", "code":"1" });
           })
    }
    return itm;
};// create the namespace  for the ITM variables 



// this can be called if the user/pw on the page is nonexistent

