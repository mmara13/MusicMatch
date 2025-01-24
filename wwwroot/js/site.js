// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('navbarSearch');
    const searchResults = document.createElement('div');
    searchResults.className = 'absolute w-full bg-white mt-1 rounded-lg shadow-lg z-50';
    searchInput.parentElement.appendChild(searchResults);

    let debounceTimeout;

    searchInput.addEventListener('input', function () {
        clearTimeout(debounceTimeout);
        const query = this.value;

        debounceTimeout = setTimeout(async () => {
            try {
                const response = await fetch(`/Profiles/SearchPartial?query=${encodeURIComponent(query)}`);
                if (response.ok) {
                    const html = await response.text();
                    searchResults.innerHTML = html;
                    searchResults.style.display = query ? 'block' : 'none';
                } else {
                    throw new Error('Search failed');
                }
            } catch (error) {
                searchResults.innerHTML = '<p class="text-red-500 p-4">An error occurred while searching. Please try again.</p>';
                searchResults.style.display = 'block';
            }
        }, 300);
    });

    // Close search results when clicking outside
    document.addEventListener('click', function (e) {
        if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
            searchResults.style.display = 'none';
        }
    });
});