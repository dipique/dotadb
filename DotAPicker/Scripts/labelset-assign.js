$(document).on("click", ".remove-label", function () {
    removeLabel(this);
});

$(".add-label").keyup(function (e) {
    if (e.keyCode == 13) { //enter
        addLabel(this.parentElement.parentElement, this.options[txtBox.selectedIndex].text);
        this.removeChild(this.options[this.selectedIndex]);
    }
});

$("input.add-button-label").click(function () {
    var selectBox = $(this.parentElement).children("select")[0];
    addLabel(this.parentElement.parentElement, selectbox.options[selectBox.selectedIndex].text);
    selectBox.removeChild(selectbox.options[selectBox.selectedIndex]);
});
