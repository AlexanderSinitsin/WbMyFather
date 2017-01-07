function disableSubmitOnEditForm() {
  $('#object-edit-content button[type="submit"]').attr('disabled', 'disabled');
  $('#recovery-password-content button[type="submit"]').attr('disabled', 'disabled');
  $('#change-password-content button[type="submit"]').attr('disabled', 'disabled');
  $('#login-content button[type="submit"]').attr('disabled', 'disabled');
}

//TODO нужно переименовать и в объект сложить
function removeDisabledSubmit() {
  $('#object-edit-content button[type="submit"]').removeAttr('disabled');
  $('#recovery-password-content button[type="submit"]').removeAttr('disabled');
  $('#change-password-content button[type="submit"]').removeAttr('disabled');
  $('#login-content button[type="submit"]').removeAttr('disabled');
}
