function startCountdown(remainingTime) {
    var countdown = remainingTime;
    var countdownText = $('#countdownText');
    var countdownSpan = $('#countdown');
    var sendOtpButton = $('#sendOtpButton');

    if (countdown > 0) {
        sendOtpButton.prop('disabled', true);
        countdownText.show();
    }

    var timer = setInterval(function () {
        countdown--;
        countdownSpan.text(countdown);

        if (countdown <= 0) {
            clearInterval(timer);
            countdownText.hide();
            sendOtpButton.prop('disabled', false);
        }
    }, 1000);
}

$(document).ready(function () {
    var remainingTime = @ViewBag.RemainingTime;
    if (remainingTime > 0) {
        startCountdown(remainingTime);
    }


    var otpSent = @Html.Raw(Json.Serialize(ViewBag.OtpSent));
    var otpVerified = @Html.Raw(Json.Serialize(ViewBag.OtpVerified));

    if (otpSent && !otpVerified) {
        $('#reopenOtpModalButton').show(); 
    } else {
        $('#reopenOtpModalButton').hide();
    }


    $('#reopenOtpModalButton').click(function () {
        $('#otpModal').modal('show');
    });
});