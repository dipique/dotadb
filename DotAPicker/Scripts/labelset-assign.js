$(document).on("click", ".remove-label", function () {
    removeLabel(this);
});

$("input.add-label-button").click(function () {
    var selectBox = $(this.parentElement).children("select")[0];
    var selectedOption = selectBox.options[selectBox.selectedIndex].text;
    if (selectedOption == "Add a label...") {
        return;
    }

    var labelSetName = this.parentElement.id;
    var labelSet = $("div.label-set#" + labelSetName)[0];
    addLabel(labelSet, selectedOption, true);
    selectBox.removeChild(selectBox.options[selectBox.selectedIndex]);
});
