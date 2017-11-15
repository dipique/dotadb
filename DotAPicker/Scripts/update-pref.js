function heroPrefChange(selBox, controller) {
    var heroID = selBox.id;
    var prefVal = selBox.options[selBox.selectedIndex].text;
    $.ajax({
        url: controller + "/UpdatePreference",
        data: {
            heroID: heroID,
            preference: prefVal
        }
    }).done(function () {
    });
}