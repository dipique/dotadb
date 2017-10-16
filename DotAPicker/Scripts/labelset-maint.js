$(document).on("click", ".remove-label", function () {
    var id = parseInt(this.id); //wacky things happen without this conversion
    var lastElement = firstUnusedIndex() - 1;

    //to be irritating, if the removal of an element results in an index gap,
    //any entries AFTER the index gap will be unceremoniously removed. That means
    //if we've created a gap, we need to...well, uncreate it
    var needToMove = lastElement > id;
    if (needToMove) {
        for (var i = id; i < lastElement; i++) {
            var newValue = getLabelValue(i + 1);
            setLabelValue(i, newValue);
        }
    }

    //remove the last element (the value of which is now a duplicate of things
    //needed to be shuffled around)
    var lastLabel = $(".label-div#" + lastElement)[0].remove();
});

function getLabelValue(index) {
    var element = $(".label-input-box#Labels_" + index + "_");
    return element[0].value;
}

function setLabelValue(index, newVal) {
    $(".label-input-box#Labels_" + index + "_")[0].value = newVal;
}

$(".add-label").keyup(function (e) {
    if (e.keyCode == 13) { //enter
        addLabel();
    }
});

function addLabel() {
    var txtBox = $(".add-label")[0];
    var text = txtBox.value.trim();
    if (text != "") { //enter
        //make sure this isn't a duplicated value
        if (labelAlreadyUsed(text)) {
            alert("Duplicate label");
            return;
        }

        //get the first unused index
        var newIndex = firstUnusedIndex();

        //create the elements to be added
        var input = $("<input/>", {
            class: 'label-input-box text-box single-line',
            id: 'Labels_' + newIndex + '_',
            name: 'Labels[' + newIndex + ']',
            type: 'text',
            value: text
        });
        var link = $("<a/>", {
            href: '#',
            class: 'remove-label',
            id: newIndex
        }).text("(remove)");
        var div = $("<div/>", {
            class: 'label-div',
            id: firstUnusedIndex
        });
        indentedAppend(div[0], input[0]); //the indents ensure consistent spacing; appendTo changes the appearance
        indentedAppend(div[0], link[0]);

        //add to the dom
        indentedAppend($(".label-set")[0], div[0]);

        //clear the text
        txtBox.value = "";
    }
}

function labelAlreadyUsed(newLabel) {
    var dup = false;
    $(".label-input-box").each(function () {
        if (this.value.trim().toUpperCase() == newLabel.toUpperCase()) {
            dup = true;
        }
    });

    return dup;
}

function firstUnusedIndex() {
    var found = false;
    var i = 0;

    //loop until a selector comes back with nothing
    while (!found) {
        var element = $(".label-div#" + i);
        if (element.length == 0) {
            return i;
        } else {
            i++;
        }
    }
}

