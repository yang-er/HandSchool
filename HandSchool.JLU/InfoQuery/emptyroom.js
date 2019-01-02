
function getList() {
    var buildings = $("#buildings").val();
    var campus = $("#campus").val();
    $("#buildings").children().hide();
    $("option[data-campus='" + campus + "']").show();
    $("#buildings").val($("option[data-campus='" + campus + "']:visible:first")[0].value);
}

function changeClassList() {
    var start = $("#startclass").val();
    $(".endclass").show();
    $(".endclass:lt(" + start + ")").hide();
    $("#endclass").val(start);
}

function p(s) { return s < 10 ? ("0" + s) : ("" + s); }

function getdata() {
    var myDate = new Date();
    var year = myDate.getFullYear();
    var month = myDate.getMonth() + 1;
    var date = myDate.getDate();
    var time = year + "-" + p(month) + "-" + p(date);
    var bid = $("#buildings").val();
    var start = $("#startclass").val();
    var cs = 0; var end = $("#endclass").val();

    for (var i = start; i <= end; i++) {
        cs += Math.pow(i, 2);
    }

    invokeCSharpAction("time " + time + " bid " + bid + " cs " + cs);
}

function callback(resp) {
    $(".item").remove();
    for (var p = 0; p < resp.value.length; p++) {
        $("#rooms").append('<tr class="item" id="' + resp.value[p].roomId + '"><td>' + resp.value[p].fullName.split("#")[1] + "</td><td>" + resp.value[p].volume + "</td><td>" + (resp.value[p].notes == null ? "" : resp.value[p].notes) + "</td>" + "</tr>");
    }
}