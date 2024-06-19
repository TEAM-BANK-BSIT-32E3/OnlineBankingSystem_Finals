// Function to send OTP
function sendOtp() {
    var senderAccountNumber = $('#senderAccountNumber').val();
    var recipientAccountNumber = $('#recipientAccountNumber').val();
    var amountToSend = $('#amountToSend').val();
    var transactionType = $('#transactionType').val();
    var description = $('#description').val();


    sessionStorage.setItem('senderAccountNumber', senderAccountNumber);
    sessionStorage.setItem('recipientAccountNumber', recipientAccountNumber);
    sessionStorage.setItem('amountToSend', amountToSend);
    sessionStorage.setItem('transactionType', transactionType);
    sessionStorage.setItem('description', description);

    fetch('/Account/SendOtp', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            phoneNumber: $('#phoneNumber').val()
        })
    }).then(response => {
        if (response.ok) {
            console.log('OTP sent successfully');

            $('#sendMoneyModal').modal('hide');

            $('#otpVerificationModal').modal('show');
        } else {
            console.error('Error sending OTP');
        }
    }).catch(error => {
        console.error('Error:', error);
    });
}

// Function to verify OTP
function verifyOtp() {
    var otp = $('#otp').val();

    $.ajax({
        type: 'POST',
        url: '/Account/VerifyOtp',
        data: JSON.stringify({ otp: otp }),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {

                alert(response.message);

                $('#otpVerificationModal').modal('hide');
                window.location.href = '/Home/Index';
            } else {

                alert(response.message);

            }
        },
        error: function (xhr, status, error) {

            alert('Error verifying OTP: ' + xhr.responseText);
        }
    });
}

// Populate hidden fields with session data on modal show
$(document).ready(function () {
    $('#otpVerificationModal').on('show.bs.modal', function (e) {
        $('#hiddenSenderAccountNumber').val(sessionStorage.getItem('senderAccountNumber'));
        $('#hiddenRecipientAccountNumber').val(sessionStorage.getItem('recipientAccountNumber'));
        $('#hiddenAmountToSend').val(sessionStorage.getItem('amountToSend'));
        $('#hiddenTransactionType').val(sessionStorage.getItem('transactionType'));
        $('#hiddenDescription').val(sessionStorage.getItem('description'));
    });
});

// Automatically hide alerts after 3 seconds
$(document).ready(function () {
    setTimeout(function () {
        $("#successMessage, #errorMessage, #depositSuccess, #withdrawSuccess, #depositError, #withdrawError").fadeOut(1000);
    }, 3000);
});

function sendWithdrawalOtp() {
    var withdrawAmount = document.getElementById("withdrawAmount").value;


    $.ajax({
        type: "POST",
        url: "/Account/SendOtpWithdrawal",
        data: { withdrawAmount: withdrawAmount },
        success: function (response) {
            alert("OTP sent successfully.");


            $('#otpInput').show();
            $('#pinInput').show();

            $('#withdrawForm').find('button[type="button"]').hide();
            $('#withdrawForm').find('button[type="submit"]').show();
        },
        error: function (error) {
            alert("Error sending OTP: " + error.responseText);
        }
    });
}