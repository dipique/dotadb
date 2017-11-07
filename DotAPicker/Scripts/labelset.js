﻿//This javascript prevents pressing enter to add a label from saving all the settings
$("form").submit(function (e) {
    if ($(".add-label")[0] === document.activeElement) {
        e.preventDefault();
        return false;
    }
});

function getLabelValue(index, setName) {
    var element = $(".label-input-box#" + setName + "_" + index + "_");
    return element[0].value;
}

function setLabelValue(index, setName, newVal) {
    $(".label-input-box#" + setName + "_" + index + "_")[0].value = newVal;

    //make sure to check if this one exists, since it doesn't always
    var hiddenBox = $(".label-hidden-box#" + setName + "_" + index + "_");
    if (hiddenBox.length > 0) {
        hiddenBox[0].value = newVal;
    }    
}

function removeLabel(lnkLabel) {
    var id = parseInt(lnkLabel.id); //wacky things happen without this conversion
    var labelSet = $(lnkLabel.parentElement.parentElement)[0];
    var labelSetName = labelSet.id;
    var lastElement = firstUnusedIndex(labelSet) - 1;
    var textOfRemovedLabel = getLabelValue(id, labelSetName);

    //to be irritating, if the removal of an element results in an index gap,
    //any entries AFTER the index gap will be unceremoniously removed. That means
    //if we've created a gap, we need to...well, uncreate it
    var needToMove = lastElement > id;
    if (needToMove) {
        for (var i = id; i < lastElement; i++) {
            var newValue = getLabelValue(i + 1, labelSetName);
            setLabelValue(i, labelSetName, newValue);
        }
    }

    //remove the last element (the value of which is now a duplicate of things
    //needed to be shuffled around)
    var labelDiv = $(labelSet).find(".label-div#" + lastElement)[0];
    $(labelDiv).remove();

    //add the removed label back to the list of options, if applicable
    var selectQuery = $("select#" + labelSetName);
    if (selectQuery.length > 0) {
        var select = selectQuery[0];
        addOptionToSelect(select, textOfRemovedLabel, textOfRemovedLabel, textOfRemovedLabel);
    }    
}

function addLabel(labelSetDiv, text, disabled) {
    if (disabled != true) {
        disabled = false;
    }

    var txtBox = $(labelSetDiv.parentElement).find(".add-label")[0];
    var fldName = labelSetDiv.id;
    if (text != "") { //enter
        //make sure this isn't a duplicated value
        if (labelAlreadyUsed(labelSetDiv, text)) {
            alert("Duplicate label");
            return;
        }

        //get the first unused index
        var newIndex = firstUnusedIndex(labelSetDiv);

        //create the elements to be added
        var input = $("<input/>", {
            class: 'label-input-box text-box single-line form-control',
            id: fldName + '_' + newIndex + '_',
            name: fldName + '[' + newIndex + ']',
            type: 'text',
            value: text,
            disabled: "disabled"
        });
        if (!disabled) {
            input.removeAttr("disabled");
        }

        var link = $("<a/>", {
            href: '#',
            class: 'remove-label',
            id: newIndex
        }).text("(remove)");
        var div = $("<div/>", {
            class: 'label-div',
            id: newIndex
        });
        indentedAppend(div[0], input[0]); //the indents ensure consistent spacing; appendTo changes the appearance
        indentedAppend(div[0], link[0]);

        //if it's disabled, add a hidden input, otherwise it won't post back
        if (disabled) {
            var hidden = $("<input/>", {
                id: fldName + '_' + newIndex + '_',
                name: fldName + '[' + newIndex + ']',
                type: hidden,
                value: text,
                class: "label-hidden-box",
                style: "display:none"
            });
            indentedAppend(div[0], hidden[0]);
        }

        //add to the dom
        indentedAppend(labelSetDiv, div[0]);
    }
}

function labelAlreadyUsed(labelSetDiv, newLabel) {
    var dup = false;

    $(labelSetDiv).find(".label-input-box").each(function () {
        if (this.value.trim().toUpperCase() == newLabel.toUpperCase()) {
            dup = true;
        }
    });

    return dup;
}

function firstUnusedIndex(labelSetDiv) {
    var found = false;
    var i = 0;

    //loop until a selector comes back with nothing
    while (!found) {
        var element = $(labelSetDiv).find(".label-div#" + i);
        if (element.length == 0) {
            return i;
        } else {
            i++;
        }
    }
}