function confirmDelete(uniqueId, isDeleteClicked) {
    console.log(uniqueId);
    var deleteBtn = 'deleteBtn_' + uniqueId;
    var confirmDeleteBtn = 'confirmDeleteBtn_' + uniqueId;

    if (isDeleteClicked) {
        $('#' + deleteBtn).hide();
        $('#' + confirmDeleteBtn).show();
    } else {
        $('#' + deleteBtn).show();
        $('#' + confirmDeleteBtn).hide();
    }
}