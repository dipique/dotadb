$(".add-label").keyup(function (e) {
    if (e.keyCode == 13) { //enter
        var labelSetName = this.parentElement.id;
        var labelSet = $("div.label-set#" + labelSetName)[0];
        addLabel(labelSet, this.options[txtBox.selectedIndex].text);
        this.removeChild(this.options[this.selectedIndex]);
    }
});

$("input.add-label-button").click(function () {
    var labelSet = $("div.label-set#Labels")[0];
    var textBox = $("#txtAdd")[0];
    addLabel(labelSet, textBox.value, false);
    textBox.value = "";
});

$(document).on("click", ".remove-label", function () {
    removeLabel(this);
});