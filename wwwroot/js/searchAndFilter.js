$(document).ready(function () {
    let debounceTimer;

    
    $('input[name="searchString"]').on('input', function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(fetchFilteredEvents, 300); // debounce of 300ms for search input
    });
    
    $('select[name="CategoryFilter"], select[name="AvailabilityFilter"], input[name="startDate"], input[name="endDate"]').on('change', function () {
        fetchFilteredEvents();
    });

    function fetchFilteredEvents() {
        const formData = $('form').serialize();

        $.ajax({
            url: '/EventManager/GetFilteredEvents',
            type: 'GET',
            data: formData,
            dataType: 'json',
            success: function (events) {
                updateTable(events);
            },
            error: function (xhr, status, error) {
                console.error('Error fetching events:', error);
            }
        });
    }

    function updateTable(events) {
        const $tbody = $('table.table tbody');
        if ($tbody.length === 0) return;

        if (events.length === 0) {
            $tbody.html('<tr><td colspan="7" class="text-center">No events found.</td></tr>');
            return;
        }

        const rows = events.map(function (event) {
            return `
                <tr class="main-row">
                    <td>${event.id}</td>
                    <td>${escapeHtml(event.title)}</td>
                    <td>${escapeHtml(event.category)}</td>
                    <td>${event.eventDate}</td>
                    <td>$${event.pricePerTicket.toFixed(2)}</td>
                    <td>${formatAvailability(event.availableTickets)}</td>
                    <td>
                        <a href="/EventManager/Edit/${event.id}" class="btn btn-sm btn-warning me-2">Edit</a>
                        <a href="/EventManager/DeleteConfirmation/${event.id}" class="btn btn-danger">Delete</a>
                    </td>
                </tr> `;
        }).join('');

        $tbody.html(rows);
    }

    function formatAvailability(tickets) {
        if (tickets <= 0) return '<span class="text-danger">Sold Out</span>';
        if (tickets <= 10) return '<span class="text-warning">' + tickets + '</span>';
        return tickets.toString();
    }

    function escapeHtml(text) {
        if (!text) return '';
        return $('<div>').text(text).html(); // escaping magic
    }
});