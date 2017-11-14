$(".hero-pref").change(function () {
    var heroID = this.id;
    var prefVal = this.options[this.selectedIndex].text;
    $.ajax({
        url: "hero/UpdatePreference",
        data: {
            heroID: heroID,
            preference: prefVal
        }
    }).done(function () {
    });
});