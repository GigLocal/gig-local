// Function that allows only future dates on an html date input element
function setDateInputMinToToday(elementName) {
    var today = `${new Date().toISOString().split('T')[0]}T00:00`;
    document.getElementsByName(elementName)[0].setAttribute('min', today);
}
