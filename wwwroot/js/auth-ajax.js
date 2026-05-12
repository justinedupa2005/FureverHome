$(document).ready(function () {
    const showToast = (message, type) => {
        const toast = $(`<div class="toast-message toast-${type}"><span>${type === 'success' ? '✅' : '❌'}</span> ${message}</div>`);
        $('.toast-container').append(toast);
        setTimeout(() => {
            toast.fadeOut('slow', function () { $(this).remove(); });
        }, 5000);
    };

    const handleAuthForm = (formId, url) => {
        $(`#${formId}`).on('submit', function (e) {
            e.preventDefault();
            const form = $(this);

            // Clear previous manual errors
            form.find('.field-validation-error').text('').removeClass('field-validation-error').addClass('field-validation-valid');
            form.find('.input-validation-error').removeClass('input-validation-error');

            if (!form.valid()) return;

            const formData = {};
            form.serializeArray().forEach(item => {
                if (item.name === "RememberMe") {
                    formData[item.name] = form.find('[name="RememberMe"]').is(':checked');
                } else {
                    formData[item.name] = item.value;
                }
            });

            const token = $('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(formData),
                headers: {
                    "RequestVerificationToken": token
                },
                success: function (response) {
                    if (response.success) {
                        showToast(response.message, 'success');
                        setTimeout(() => {
                            window.location.href = response.redirectUrl;
                        }, 1500);
                    } else {
                        if (response.errors) {
                            // Display field-specific errors
                            for (let field in response.errors) {
                                // Find the validation span for this field
                                let span = form.find(`[data-valmsg-for="${field}"]`);
                                if (span.length) {
                                    span.text(response.errors[field]);
                                    span.removeClass('field-validation-valid').addClass('field-validation-error');
                                }
                                // Highlight the input
                                form.find(`[name="${field}"]`).addClass('input-validation-error');
                            }
                        } else if (response.message) {
                            showToast(response.message, 'error');
                        }
                    }
                },
                error: function () {
                    showToast("An unexpected error occurred. Please try again.", 'error');
                }
            });
        });
    };

    if ($('#loginForm').length) handleAuthForm('loginForm', '/Account/Login');
    if ($('#registerForm').length) handleAuthForm('registerForm', '/Account/Register');
});
