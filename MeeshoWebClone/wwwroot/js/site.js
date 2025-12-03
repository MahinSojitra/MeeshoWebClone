$(document).ready(function () {
    // Show dropdown on focus
    $("#searchInput").on("focus", function () {
        $("#searchDropdown").fadeIn();
    });

    // Hide dropdown when clicking outside
    $(document).on("click", function (event) {
        if (!$(event.target).closest(".search-container").length) {
            $("#searchDropdown").fadeOut();
        }
    });

    // Fill input when clicking a pill
    $(".search-pill").on("click", function () {
        $("#searchInput").val($(this).text());
        $("#searchDropdown").fadeOut();
    });

    document.querySelectorAll(".mega-dropdown").forEach((dropdown) => {
        dropdown.addEventListener("mouseenter", function () {
            this.querySelector(".mega-menu").style.display = "block";
        });

        dropdown.addEventListener("mouseleave", function () {
            this.querySelector(".mega-menu").style.display = "none";
        });

        dropdown.addEventListener("click", function () {
            let menu = this.querySelector(".mega-menu");
            if (menu.style.display === "block") {
                menu.style.display = "none";
            } else {
                menu.style.display = "block";
            }
        });
    });
});
