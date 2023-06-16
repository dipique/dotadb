// a filter set is a configuration object required for each filterTable. It must be a variable named "filterSet" accessible by filterFunction()
var sampleFilterSet = [[
        "nameFilter",  // id of object with filter value
        "multi",       // type of element with filter value (multi: multi-select, single: single-select, text: input box, row-index-exclusion)
        "nameValue",   // id of header cells that contain the value
        "inner"        // method of retrieving cell value (inner: inner HTML, select: select box option)
    ], [
        "preferenceFilter",
        "single",
        "preferenceValue",
        "select"
    ], [
        "typeFilter",
        "single",
        "typeValue",
        "inner"
    ], [
        "textFilter",
        "text",
        "textValue",
        "inner"
    ], [
        "rowIdFilter",
        "row-index-exclusion", // special kind of filter that excludes items based on an array of row IDs
        "row",
        "array"
    ]
];


//Hide the table rows that do not meet the criteria
function filterFunction() {
    //get table
    var table = document.getElementById("filterTable");

    //get filters
    var filters = getSearchCriteriaFromFilterSet(filterSet);
    var rowFilter = filters.get("row");
    var hasRowFilter = typeof rowFilter != "undefined";

    //loop through the table rows
    for (var i = 0; i < table.childNodes.length; i++) {
        //skip elements other than table rows
        var row = table.childNodes[i];
        if (typeof row.className == "undefined") {
            continue;
        } else if (row.className.indexOf("data-row") == -1) {
            continue;
        }

        //check for a row exclusions before checking all the cell values
        var hideRow = false;
        var rowId = row.id;
        if (hasRowFilter && rowId) { //if there's a row ID filter and the row has an ID...
            if (rowFilter.searchValue.includes("|" + rowId + "|")) {
                hideRow = true;
            }
        }

        //now check the cell values
        if (!hideRow) {
            var cellID, cellValue, filter;
            for (var j = 0; j < row.childNodes.length; j++) {
                //only process the loop for cells
                var cell = row.childNodes[j];
                if (typeof cell.className == "undefined") {
                    continue;
                } else if (cell.className.indexOf("data-row-cell") == -1) {
                    continue;
                }

                //check if there is a filter on this cell
                filter = filters.get(cell.id);
                if (typeof filter == "undefined") { //if there's no filter on this cell
                    continue;
                }

                //get the cell contents
                var cellContents;
                switch (filter.fetchType) {
                    case "inner":
                        cellContents = cell.innerHTML.toUpperCase().trim();
                        break;
                    case "select":
                        var selBox = $(cell).find("select")[0];
                        cellContents = selBox.options[selBox.selectedIndex].text.toUpperCase().trim();
                        break;
                }

                //now see if it matches the filter
                switch (filter.matchType) {
                    case "partial":
                        hideRow = !cellContents.includes(filter.searchValue);
                        break;
                    case "partial-reverse":
                        hideRow = !filter.searchValue.includes(cellContents);
                        break;
                    case "full":
                        hideRow = !cellContents == filter.searchValue;
                        break;
                    case "full-multi":
                        hideRow = !allOfPipedResultSetContained(filter.searchValue, cellContents);
                        break;
                }

                if (hideRow) { //don't keep looking if we've found exclusion criteria
                    break;
                }
            }
        }

        //hide the row, if necessary
        if (!hideRow) {
            row.style.display = "";
        } else {
            row.style.display = "none";
        }
    }

    //if necessary, hide/show any elements that are based on whether any items are remaining
    showNoDataElements();
}

//processes the filterSet into criteria that can be applied to the filter table
function getSearchCriteriaFromFilterSet() {
    var returnSet = new Map();
    filterSet.forEach(function (filter) {
        var element = document.getElementById(filter[0]); //contains the component that contains the search value
        var filterType = filter[1];                       //single, multi, text, etc.
        if (!element && filterType != "row-index-exclusion") { //if filter doesn't exist, exit
            return;                                            //except row exclusions, which 
        }                                                      //aren't mapped to elements
        var searchValue, matchType;
        switch (filterType) {
            case "single":
                searchValue = element.options[element.selectedIndex].text.toUpperCase();
                matchType = "partial";         //entire search term is contained within cell contents
                break;
            case "multi":
                searchValue = getPipedMultiSelectOptions(element);
                matchType = "partial-reverse";  //entire cell contents contained in search term
                break;
            case "multi-value":
                searchValue = getPipedMultiSelectOptions(element);
                matchType = "full-multi";       //all search terms contained within cell contents
                break;
            case "text":
                searchValue = element.value.toUpperCase();
                matchType = "partial";          //entire search term is equal to cell contents
                break;
            case "row-index-exclusion":
                searchValue = arrayToPipedOptions(window[filter[0]]); //get the array of index values
                matchType = "row-exclusion";    //row index (|ind|) contained in search term
                break;
        }
        var valueCellID = filter[2];  //contains the ID of the header column to be filtered
        if (searchValue.length < 1) { //if there's no search filter here, don't add it to the map
            return;
        }

        returnSet.set(valueCellID, {
            searchValue: searchValue,
            matchType: matchType,
            fetchType: filter[3]
        });
    });

    return returnSet;
}

function allOfPipedResultSetContained(pipedResultSet, valueText) {
    var results = pipedResultSet.split("|");
    for (var x = 0; x < results.length; x++) {
        if (results[x].trim().length < 1) continue;
        if (!valueText.includes(results[x])) return false;
    }

    //all of the search terms are contained
    return true;
}

//gets all the options selected in a multi-select box and returns them as a pipe-delimited string
function getPipedMultiSelectOptions(selBox) {
    var searchPreference = "";
    for (var x = 0; x < selBox.options.length; x++) {
        if (selBox.options[x].selected && selBox.options[x].value != "") {
            searchPreference += selBox.options[x].value + "|"; //the pipe eliminates (most) partial matches
        }
    }

    return searchPreference.toUpperCase();
}

function arrayToPipedOptions(array) {
    var retVal = "|";  //this seed is what guarantees partial match elimination
    for (var x = 0; x < array.length; x++) {
        retVal += array[x] + "|";                //the pipe eliminates all partial matches
    }
    if (retVal == "|") return "";

    return retVal.toUpperCase();
}

function countVisibleItems() {
    // Get search criteria
    var table = document.getElementById("filterTable");
    var visibleCount = 0;

    //loop through the table rows
    for (var i = 0; i < table.childNodes.length; i++) {
        //skip elements other than table rows
        var row = table.childNodes[i];
        if (typeof row.className == "undefined") {
            continue;
        } else if (row.className.indexOf("data-row") == -1) {
            continue;
        }

        //record if it is visible
        if (row.style.display == "") {
            visibleCount++;
        }
    }

    return visibleCount;
}

function showNoDataElements() {
    var noData = $(".no-data");
    if (noData.length < 1) {
        return;
    }

    if (countVisibleItems() == 0) {
        noData[0].style.display = "";
    } else {
        noData[0].style.display = "none";
    }
}

//Gets the if of the first visible row
function firstVisibleItemIndex() {
    // Get search criteria
    var table = document.getElementById("filterTable");

    //loop through the table rows
    for (var i = 0; i < table.childNodes.length; i++) {
        //skip elements other than table rows
        var row = table.childNodes[i];
        if (typeof (row.className) == "undefined") {
            continue;
        } else if (row.className.indexOf("data-row") == -1) {
            continue;
        }

        //return the row ID if it is visible
        if (row.style.display == "") {
            return row.id;
        }
    }

    return -1;
}
