
document.addEventListener("DOMContentLoaded", function() {
    // Hide <h4> elements containing the specific text
    var h4Elements = document.querySelectorAll("h4");
    h4Elements.forEach(function(h4) {
        if (h4.textContent.includes("Use another service to register")) {
            h4.style.display = "none";
        }
    });

    // Hide <p> elements containing the specific text
    var pElements = document.querySelectorAll("p");
    pElements.forEach(function(p) {
        if (p.textContent.includes("There are no external authentication services configured")) {
            p.style.display = "none";
        }
    });
});
