document.addEventListener('DOMContentLoaded', function () {
    let currentTab = 0;
    showTab(currentTab);

    function showTab(n) {
        let tabs = document.getElementsByClassName("tab");
        tabs[n].classList.add("active");

        if (n === 0) {
            document.getElementById("prevBtn").style.display = "none";
        } else {
            document.getElementById("prevBtn").style.display = "inline";
        }

        if (n === (tabs.length - 1)) {
            document.getElementById("nextBtn").style.display = "none";
            document.getElementById("submitBtn").style.display = "inline";
        } else {
            document.getElementById("nextBtn").style.display = "inline";
            document.getElementById("submitBtn").style.display = "none";
        }

        fixStepIndicator(n);
        updateConfirmationDetails();
    }

    function nextPrev(n) {
        let tabs = document.getElementsByClassName("tab");

        // Prevent moving to the next tab if validation fails
        if (n === 1 && !validateForm()) return false;

        tabs[currentTab].classList.remove("active");

        currentTab = currentTab + n;

        if (currentTab >= tabs.length) {
            document.getElementById("regForm").submit();
            return false;
        }

        showTab(currentTab);
    }

    async function validateForm() {
        let tabs, inputs, valid = true;
        tabs = document.getElementsByClassName("tab");
        inputs = tabs[currentTab].getElementsByTagName("input");

        for (let i = 0; i < inputs.length; i++) {
            if (inputs[i].value === "") {
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

            let dob = new Date(document.getElementById("DateOfBirth").value);
            let currentYear = new Date().getFullYear();
            if (dob.getFullYear() >= currentYear) {
                document.getElementById("DateOfBirth").className += " invalid";
                document.getElementById("dobError").innerText = "Date of Birth cannot be the current year or later";
                valid = false;
            } else {
                document.getElementById("DateOfBirth").className = document.getElementById("DateOfBirth").className.replace(" invalid", "");
                document.getElementById("dobError").innerText = "";
            }

            let accountNumber = document.getElementById("AccountNumber").value;
            if (accountNumber.length !== 10) {
                document.getElementById("AccountNumber").className += " invalid";
                document.getElementById("accountNumberError").innerText = "Account Number must be exactly 10 characters";
                valid = false;
            } else {
                let isRegistered = await checkAccountNumberRegistered(accountNumber);
                if (isRegistered) {
                    document.getElementById("AccountNumber").className += " invalid";
                    document.getElementById("accountNumberError").innerText = "This account number is already registered.";
                    valid = false;
                } else {
                    document.getElementById("AccountNumber").className = document.getElementById("AccountNumber").className.replace(" invalid", "");
                    document.getElementById("accountNumberError").innerText = "";
                }
            }

            let contactNumber = document.getElementById("ContactNumber").value;
            if (!validateContactNumber(contactNumber)) {
                document.getElementById("ContactNumber").className += " invalid";
                document.getElementById("contactNumberError").innerText = "Please enter a valid contact number in the format +63xxxxxxxxxx";
                valid = false;
            } else {
                // Check if contact number is already registered
                let isRegistered = await checkContactNumberRegistered(contactNumber);
                if (isRegistered) {
                    document.getElementById("ContactNumber").className += " invalid";
                    document.getElementById("contactNumberError").innerText = "This contact number is already registered.";
                    valid = false;
                } else {
                    document.getElementById("ContactNumber").className = document.getElementById("ContactNumber").className.replace(" invalid", "");
                    document.getElementById("contactNumberError").innerText = "";
                }
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

            if (pin.length !== 4) {
                document.getElementById("pin").className += " invalid";
                document.getElementById("pinError").innerText = "PIN must be exactly 4 characters";
                valid = false;
            } else {
                document.getElementById("pin").className = document.getElementById("pin").className.replace(" invalid", "");
                document.getElementById("pinError").innerText = "";
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

        let fieldNames = {
            "Username": "Username",
            "Name": "Full Name",
            "DateOfBirth": "Date of Birth",
            "ContactNumber": "Contact Number",
            "Email": "Email Address",
            "Address": "Address",
            "Branch": "Branch",
            "AccountType": "Card Type",
            "AccountNumber": "Account Number",
            "SecurityQuestion1": "Security Question 1",
            "answer1": "Answer 1",
            "SecurityQuestion2": "Security Question 2",
            "answer2": "Answer 2"
        };

        for (let [key, value] of formData.entries()) {
            if (excludeFields.includes(key)) {
                continue;
            }

            if (key in fieldNames) {
                let label = fieldNames[key];
                confirmationDetails.innerHTML += `<p><strong>${label}:</strong> ${value}</p>`;
            }
        }
    }

    function validateContactNumber(contactNumber) {
        let regex = /^\+63\d{10}$/;
        return regex.test(contactNumber);
    }

    async function checkContactNumberRegistered(contactNumber) {
        let response = await fetch('/Account/CheckContactNumber', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ contactNumber: contactNumber })
        });

        if (response.ok) {
            let result = await response.json();
            return result.isRegistered;
        } else {
            console.error('Failed to check contact number');
            return false;
        }
    }

    async function checkAccountNumberRegistered(accountNumber) {
        let response = await fetch('/Account/CheckAccountNumber', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ accountNumber: accountNumber })
        });

        if (response.ok) {
            let result = await response.json();
            return result.isRegistered;
        } else {
            console.error('Failed to check account number');
            return false;
        }
    }

    document.getElementById("prevBtn").addEventListener("click", function () {
        nextPrev(-1);
    });

    document.getElementById("nextBtn").addEventListener("click", function () {
        nextPrev(1);
    });

});
