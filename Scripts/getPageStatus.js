
"use strict";

/*
getPageStatus

asks server if the user can move to the next session.
The server allows if all users are logged in, or if all users are ready/finish the current page.


// turn 0: login page
*/

function getPageStatus(turn, max) {
    var ex_set = $("#MainContent_hdEX_SET").val();
    var ex_turn = $("MainContent_hdTURN").val();
    var ex_groudId = $("MainContent_hdGROUPID").val();

    // From Login page
    if (ex_turn === undefined || ex_turn === null) {
        ex_turn = 0;
    }

    var info = {
        set: ex_set,
        turn: ex_turn,
        groupId: ex_groudId
    };

    setInterval(function () {
        $.ajax({
            url: _pageName,
            type: "GET",
            data: info,
        }).done(function(data){
            if (data.valid === true) {
                //redirect the page
            }
            else {
                //do nothing, and ask again
            }
        });
    }, 3000);
}