let currentTab = 0;
showTab(currentTab);

function showTab(n) {
    let tabs = document.getElementsByClassName("tab");
    tabs[n].classList.add("active");

    if (n == 0) {
        document.getElementById("prevBtn").style.display = "none";
    } else {
        document.getElementById("prevBtn").style.display = "inline";
    }

    if (n == (tabs.length - 1)) {
        document.getElementById("nextBtn").innerHTML = "Submit";
    } else {
        document.getElementById("nextBtn").innerHTML = "Next";
    }

    fixStepIndicator(n);
    updateConfirmationDetails();
}

function nextPrev(n) {
    let tabs = document.getElementsByClassName("tab");

    if (n == 1 && !validateForm()) return false;

    tabs[currentTab].classList.remove("active");

    currentTab = currentTab + n;

    if (currentTab >= tabs.length) {
        document.getElementById("regForm").submit();
        return false;
    }

    showTab(currentTab);
}

function validateForm() {
    let tabs, inputs, valid = true;
    tabs = document.getElementsByClassName("tab");
    inputs = tabs[currentTab].getElementsByTagName("input");

    for (let i = 0; i < inputs.length; i++) {
        if (inputs[i].value == "") {
            inputs[i].className += " invalid";
            valid = false;
        }
    }

    if (currentTab === 0) {
        let password = document.getElementById("Password").value;
        let rePassword = document.getElementById("RePassword").value;
        if (password !== rePassword) {
            document.getElementById("Password").className += " invalid";
            document.getElementById("RePassword").className += " invalid";
            document.getElementById("passwordMatchError").innerText = "Passwords do not match";
            valid = false;
        } else {
            document.getElementById("Password").className = document.getElementById("Password").className.replace(" invalid", "");
            document.getElementById("RePassword").className = document.getElementById("RePassword").className.replace(" invalid", "");
            document.getElementById("passwordMatchError").innerText = "";
        }
    }

    if (currentTab === 1) {
        let pin = document.getElementById("pin").value;
        let rePin = document.getElementById("re-pin").value;
        if (pin !== rePin) {
            document.getElementById("pin").className += " invalid";
            document.getElementById("re-pin").className += " invalid";
            document.getElementById("pinMatchError").innerText = "PINs do not match";
            valid = false;
        } else {
            document.getElementById("pin").className = document.getElementById("pin").className.replace(" invalid", "");
            document.getElementById("re-pin").className = document.getElementById("re-pin").className.replace(" invalid", "");
            document.getElementById("pinMatchError").innerText = "";
        }
    }

    if (valid) {
        document.getElementsByClassName("step")[currentTab].className += " finish";
    }
    return valid;
}

function fixStepIndicator(n) {
    let steps = document.getElementsByClassName("step");
    for (let i = 0; i < steps.length; i++) {
        steps[i].classList.remove("active");
    }
    steps[n].classList.add("active");
}
function updateConfirmationDetails() {
    if (currentTab !== 2) return;

    let confirmationDetails = document.getElementById("confirmation-details");
    let formData = new FormData(document.getElementById("regForm"));
    confirmationDetails.innerHTML = "";


    let excludeFields = ["pin", "re-pin"];

    for (let [key, value] of formData.entries()) {

        if (excludeFields.includes(key) || value.includes("CfDJ")) {
            continue;
        }

        let element = document.querySelector(`[name="${key}"]`);
        if (element && element.type !== "password" && element.type !== "checkbox") {
            confirmationDetails.innerHTML += `<p><strong>${element.previousElementSibling.innerText}:</strong> ${value}</p>`;
        }
    }
}