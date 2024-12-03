

$(document).ready(function () {
    let userId = 142;  // Replace this with dynamic logic to get the userId
    let window; // Define window here to use globally

    // Initialize Kendo DatePicker for birth date
    $("#datepicker").kendoDatePicker({
        dateInput: true,
        min: new Date(1990, 0, 1), // Set the min year to 1990
        max: new Date(2005, 0, 0),
        format: "dd/MM/yyyy",
    });

    $('#male').kendoRadioButton({
        label: "Male"
    })
    $('#female').kendoRadioButton({
        label: "Female"
    })


    // Fetch profile data and display in Kendo Cards
    $.ajax({
        url: `http://localhost:5064/api/Student/GetProfileById/${userId}`,
        type: 'GET',
        success: function (data) {
            renderProfileCards(data);
        },
        error: function () {
            alert("Failed to load profile data.");
        }
    });


    function fatch() {
        $.ajax({
            url: `http://localhost:5064/api/Student/GetProfileById/${userId}`,
            type: 'GET',
            success: function (data) {
                renderProfileCards(data);
            },
            error: function () {
                alert("Failed to load profile data.");
            }
        });
    }



    // Function to render profile data in Kendo Cards
    function renderProfileCards(data) {
        var cardsHtml = `
        <div class="k-card">
            <div class="k-card-header">
                <h5 class="k-card-title">${data.name}</h5>
            </div>
            <img alt="${data.name} Profile" class="k-card-media image-large" src="${apihost}/${data.imagepath}" />
            <div class="k-card-body">
                <p>
                    <strong>Email:</strong> ${data.email} <br />
                    <strong>Mobile:</strong> ${data.mobileNumber} <br />
                    <strong>Birth Date:</strong> <span>${new Date(data.birthDate).toLocaleDateString()}</span><br />
                    <strong>Address:</strong> ${data.address} <br />
                    <strong>Gender:</strong> ${data.gender === 0 ? " Male" : "Female"} <br />
                </p>
            </div>
            <div class="k-card-footer">
                <button class="k-button k-button-flat k-button-md k-rounded-md update-button" data-id="${data.user_id}">
                    <span class="k-icon k-i-edit"></span> Edit
                </button>
            </div>
        </div>
        `;

        $("#cardsContainer").html(cardsHtml);

        $(".update-button").click(function () {
            $("#editName").val(data.name);
            $("#editEmail").val(data.email);
            $("#editMobile").val(data.mobileNumber);
            $("#datepicker").data("kendoDatePicker").value(new Date(data.birthDate));
            $("#editAddress").val(data.address);

            if (data.gender === 0) {
                $("#male").prop("checked", true);
            } else if (data.gender === 1) {
                $("#female").prop("checked", true);
            }

            // Initialize Kendo Window for the modal and center it on screen
            window = $("#editModal").kendoWindow({
                width: "400px",
                title: "Edit Profile",
                visible: false,
                modal: true
            }).data("kendoWindow");

            window.center().open();  // This ensures the modal is centered on the screen
        });

    }

    // Save changes when Save button is clicked in the edit profile modal
    $("#saveButton").click(function () {
        let isValid = true;

        // Regular expressions for validation
        const namePattern = /^[a-zA-Z\s]+$/; // Only letters and spaces
        const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        const mobilePattern = /^[0-9]{10}$/; // Exactly 10 digits

        // Reset error messages
        $(".error-message").text("").hide();

        // Name validation
        const nameValue = $("#editName").val();
        if (nameValue === "") {
            $("#nameError").text("Name is required.").show();
            isValid = false;
        } else if (!namePattern.test(nameValue)) {
            $("#nameError").text("Name should contain only alphabetic characters.").show();
            isValid = false;
        }

        // Email validation
        const emailValue = $("#editEmail").val();
        if (emailValue === "") {
            $("#emailError").text("Email is required.").show();
            isValid = false;
        } else if (!emailPattern.test(emailValue)) {
            $("#emailError").text("Please enter a valid email address.").show();
            isValid = false;
        }

        // Gender validation
        if ($("input[name='Gender']:checked").length === 0) {
            $("#genderError").text("Gender is required.").show();
            isValid = false;
        }

        // Mobile number validation
        const mobileValue = $("#editMobile").val();
        if (mobileValue === "") {
            $("#mobileError").text("Mobile number is required.").show();
            isValid = false;
        } else if (!mobilePattern.test(mobileValue)) {
            $("#mobileError").text("Mobile number should be exactly 10 digits.").show();
            isValid = false;
        }

        // Birth date validation
        if ($("#datepicker").val() === "") {
            $("#birthDateError").text("Birth Date is required.").show();
            isValid = false;
        }

        // Address validation
        if ($("#editAddress").val() === "") {
            $("#addressError").text("Address is required.").show();
            isValid = false;
        }


        if (isValid) {

            var formData = new FormData();
            formData.append("name", $("#editName").val());
            formData.append("email", $("#editEmail").val());
            formData.append("mobileNumber", $("#editMobile").val());
            formData.append("birthDate", $("#datepicker").val());
            formData.append("address", $("#editAddress").val());
            formData.append("gender", $("input[name='Gender']:checked").val());
            formData.append("image", $("#editImage")[0].files[0]);

            // Print formData contents to the console
            formData.forEach((value, key) => {
                console.log(key + ": " + (value instanceof File ? value.name : value));
            });

            $.ajax({
                url: `http://localhost:5064/api/Teacher/UpdateTeacherProfile/${userId}`,
                type: 'PUT',
                data: formData,
                contentType: false,
                processData: false,
                success: function () {
                    window.close();
                    fatch();
                },
                error: function () {
                    alert("Failed to update profile.");
                }
            });
        }

    });



    // Close the modal when Cancel button is clicked
    $("#cancelButton").click(function () {
        $("#editModal").data("kendoWindow").close();
    });
});
