function updateUnreadCount() {
    fetch('/Notifications/GetUnreadCount')
        .then(response => response.json())
        .then(data => {
            const unreadCountElement = document.getElementById('unread-count');
            if (unreadCountElement) {
                const count = Number(data.count) || 0;

                if (count > 0) {
                    unreadCountElement.textContent = count.toString();
                    unreadCountElement.style.display = 'flex';
                } else {
                    unreadCountElement.style.display = 'none';
                }
            }
        })
        .catch(error => {
            console.error('Error updating unread count:', error);
            const unreadCountElement = document.getElementById('unread-count');
            if (unreadCountElement) {
                unreadCountElement.style.display = 'none';
            }
        });
}

function toggleNotificationsDropdown() {
    const dropdown = document.getElementById('notifications-dropdown');
    const notificationsList = document.getElementById('notifications-list');

    // Toggle dropdown visibility
    dropdown.classList.toggle('hidden');

    // If dropdown is now visible, load notifications
    if (!dropdown.classList.contains('hidden')) {
        fetch('/Notifications/GetLatest')
            .then(response => response.text())
            .then(html => {
                // If no notifications, show "No notifications" message
                notificationsList.innerHTML = html || `
                    <div class="p-4 text-center text-gray-500">
                        No unread notifications
                    </div>
                `;
            })
            .catch(error => {
                console.error('Error loading notifications:', error);
                notificationsList.innerHTML = `
                    <div class="p-4 text-center text-gray-500">
                        Unable to load notifications
                    </div>
                `;
            });
    }
}

function markAsRead(notificationId) {
    fetch(`/Notifications/MarkAsRead/${notificationId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                // Reload notifications and update count
                fetch('/Notifications/GetLatest')
                    .then(response => response.text())
                    .then(html => {
                        const notificationsList = document.getElementById('notifications-list');
                        notificationsList.innerHTML = html || `
                        <div class="p-4 text-center text-gray-500">
                            No unread notifications
                        </div>
                    `;
                        updateUnreadCount();
                    });
            }
        })
        .catch(error => {
            console.error('Error marking notification as read:', error);
        });
}

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    updateUnreadCount(); // Initial update

    // Close dropdown when clicking outside
    document.addEventListener('click', function (event) {
        const bell = document.getElementById('notification-bell');
        const dropdown = document.getElementById('notifications-dropdown');

        if (bell && dropdown &&
            !bell.contains(event.target) &&
            !dropdown.contains(event.target)) {
            dropdown.classList.add('hidden');
        }
    });
});