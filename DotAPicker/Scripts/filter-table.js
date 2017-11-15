//Derived from: https://www.w3schools.com/howto/howto_js_filter_table.asp
//This is what enables filtering the hero list by name and preference
function filterFunction() {
    // Get search criteria
    var input = document.getElementById("filterInput");
    var filter = input.value.toUpperCase();
    var table = document.getElementById("filterHeroTable");

    //get search preference
    var searchPreference = "";
    var selBox = $("#filterPreference")[0];
    for (var x = 0; x < selBox.options.length; x++) {
        if (selBox.options[x].selected && selBox.options[x].value != "") {
            searchPreference += selBox.options[x].value + "|"; //the pipe just eliminates partial matches
        }
    }

    //loop through the table rows
    for (var i = 0; i < table.childNodes.length; i++) {
        //skip elements other than table rows
        var row = table.childNodes[i];
        if (typeof (row.className) == "undefined") {
            continue;
        } else if (row.className.indexOf("data-row") == -1) {
            continue;
        }

        //get the cell contents
        var heroName, heroAltNames, heroPreference;
        for (var j = 0; j < row.childNodes.length; j++) {
            //only process the loop for cells
            var cell = row.childNodes[j];
            if (typeof (cell.className) == "undefined") {
                continue;
            } else if (cell.className.indexOf("data-row-cell") == -1) {
                continue;
            }

            //get the cell contents
            switch (cell.id) {
                case "heroName":
                    heroName = cell.innerHTML.toUpperCase();
                    break;
                case "heroAltNames":
                    heroAltNames = cell.innerHTML.toUpperCase();
                    break;
                case "heroPreference":
                    var selBox = $(cell).find("select")[0];
                    heroPreference = selBox.options[selBox.selectedIndex].text;
                    break;
            }
        }

        //determine whether to hide the row
        var hideRow = false;
        if (searchPreference != "" && !searchPreference.includes(heroPreference)) { //look for matching preference
            hideRow = true;
        } else if (filter != "" && !(heroName.includes(filter) || heroAltNames.includes(filter))) { //look for matching filter
            hideRow = true;
        } else if (typeof allyArray !== 'undefined' && typeof enemyArray !== 'undefined') { //if there is an enemy/ally array in scope
            hideRow = allyArray.indexOf(row.id) > -1 || enemyArray.indexOf(row.id) > -1;    //check if the hero is already assigned, and hide if so
        }

        //hide the row, if necessary
        if (!hideRow) {
            row.style.display = "";
        } else {
            row.style.display = "none";
        }
    }
}

