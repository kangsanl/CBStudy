
"use strict";

/*
getPageStatus

asks server if the user can move to the next session.
The server allows if all users are logged in, and if all users are ready for the decision page

*/

function getPageStatus() {
    var ex_set = $("#MainContent_hdEX_SET").val();
    var ex_session = $("#MainContent_lblGroupID").text();
    var ex_turn = $("#MainContent_lblTrial").text();
    
    $("#main").addClass("hide");
    $("#wait").removeClass("hide");
    // From Login page
    if (ex_turn === undefined || ex_turn === null) {
        ex_turn = 0;
    }


    var info = {
        set: ex_set,
        session: ex_session,
        turn: ex_turn
    };

    var interval = null;
    var request = function () {
        $.ajax({
            url: "Decision.aspx/GetPageStatus",
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
                    clearInterval(interval);
                }
                else {
                    $("#main").addClass("hide");
                    $("#wait").removeClass("hide");
                }
            }
        });
    }
    


    interval = setInterval(request, 1000);
}