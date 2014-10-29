
"use strict";

/*
getPageStatus

asks server if the user can move to the next session.
The server allows if all users are logged in, or if all users are ready/finish the current page.


// turn 0: login page
*/

function getPageStatus() {
    var ex_set = $("#MainContent_hdEX_SET").val();
    var ex_turn = $("MainContent_hdTURN").val();
    var ex_groudId = $("MainContent_hdGROUPID").val();

    // From Login page
    if (ex_turn === undefined || ex_turn === null) {
        ex_turn = 0;
    }

    if (ex_groudId === undefined || ex_groudId === null) {
        ex_groudId = 0;
    }

    var info = {
        set: ex_set,
        turn: ex_turn,
        groupId: ex_groudId
    };


    var request = function () {
        $.ajax({
            url: "Login.aspx/GetPageStatus",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(info),
            dataType: "json",
            success: function (data) {
                console.log(data.d);
                var result = JSON.parse(data.d)
                if (result.redirect === true) {
                    //redirect to the next
                    $("#main").removeClass("hide");
                    $("#wait").addClass("hide");
                }
                else {
                    $("#main").addClass("hide");
                    $("#wait").removeClass("hide");
                }
            }
        });
    }
    


    setInterval(request, 3000);
}