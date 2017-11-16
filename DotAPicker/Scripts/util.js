//Credit: https://stackoverflow.com/questions/19736663/appending-elements-to-dom-with-indentation-spacing
function indentedAppend(parent, child) {
    var indent = "",
        elem = parent;

    while (elem && elem !== document.body) {
        indent += "  ";
        elem = elem.parentNode;
    }

    if (parent.hasChildNodes() && parent.lastChild.nodeType === 3 && /^\s*[\r\n]\s*$/.test(parent.lastChild.textContent)) {
        parent.insertBefore(document.createTextNode("\n" + indent), parent.lastChild);
        parent.insertBefore(child, parent.lastChild);
    } else {
        parent.appendChild(document.createTextNode("\n" + indent));
        parent.appendChild(child);
        parent.appendChild(document.createTextNode("\n" + indent.slice(0, -2)));
    }

}

//credit: http://www.dyn-web.com/tutorials/forms/select/option/dom-demo.php
function addOptionToSelect(sel, txt, val, obj) {
    var opt = document.createElement('option');
    opt.appendChild(document.createTextNode(txt));

    if (typeof val === 'string') {
        opt.value = val;
    }

    //if (!obj) {
        sel.appendChild(opt);
        return;
    //}
}

//gets the parent (or grandparent, etc) of an element given a class name. Gets the nearest parent if
//more than one match.
function getParentElement(className, element) {
    var found = false;
    var currentElement = element;
    while (!found) {
        if (currentElement.className.includes(className)) {
            return currentElement;
        } else {
            currentElement = currentElement.parentElement;
            if (currentElement === undefined) {
                return null;
            }
        }
    }

    //this shouldn't ever actually happen
    return null;
}

function resetSelectElement(selectElement) {
    var options = selectElement.options;

    // Look for a default selected option
    for (var i = 0, iLen = options.length; i < iLen; i++) {

        if (options[i].selected) {
            options[i].selected = false;
        }
    }

    selectElement.selectedIndex = -1;
}

function getEl(id) {
    return document.getElementById(id);
}