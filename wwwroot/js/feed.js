document.addEventListener('DOMContentLoaded', function () {
    const scrollingWrappers = document.querySelectorAll('.scrolling-wrapper');

    scrollingWrappers.forEach(wrapper => {
        wrapper.addEventListener('wheel', function (event) {
            if (event.deltaY !== 0) {
                event.preventDefault();
                wrapper.scrollLeft += event.deltaY;
            }
        });
    });
});
