// wwwroot/js/search.js
document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('navbarSearch');
    const searchResultsDropdown = document.getElementById('searchResultsDropdown');
    let searchTimeout;

    if (!searchInput || !searchResultsDropdown) {
        console.error('Search elements not found!');
        return;
    }

    searchInput.addEventListener('input', handleSearch);
    searchInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            handleSearch(e);
        }
    });

    function handleSearch(e) {
        clearTimeout(searchTimeout);
        const query = searchInput.value.trim();

        console.log('Search query:', query); // Debug log

        if (query.length === 0) {
            searchResultsDropdown.classList.add('hidden');
            return;
        }

        searchTimeout = setTimeout(() => {
            searchResultsDropdown.classList.remove('hidden');
            searchResultsDropdown.innerHTML = '<div class="p-4 text-center text-gray-500">Searching...</div>';

            fetch(`/Profiles/SearchPartial?query=${encodeURIComponent(query)}`)
                .then(async response => {
                    if (!response.ok) {
                        const text = await response.text();
                        throw new Error(text);
                    }
                    return response.text();
                })
                .then(html => {
                    console.log('Search results received'); // Debug log
                    if (html.includes('error')) {
                        const errorData = JSON.parse(html);
                        throw new Error(errorData.error);
                    }
                    searchResultsDropdown.innerHTML = html;
                })
                .catch(error => {
                    console.error('Search error:', error);
                    searchResultsDropdown.innerHTML = `
                    <div class="p-4 text-center text-red-500">
                        ${error.message || 'An error occurred while searching. Please try again.'}
                    </div>
                `;
                });
        }, 300);
    }

    // Close dropdown when clicking outside
    document.addEventListener('click', function (e) {
        if (!searchInput.contains(e.target) && !searchResultsDropdown.contains(e.target)) {
            searchResultsDropdown.classList.add('hidden');
        }
    });
});