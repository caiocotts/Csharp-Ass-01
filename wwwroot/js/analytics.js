/**
 * Analytics Dashboard Charts
 * Requires Chart.js to be loaded
 */
document.addEventListener('DOMContentLoaded', function () {
    initTicketSalesChart();
    initMonthlyRevenueChart();
});

/**
 * Initializes the Ticket Sales by Category polar area chart
 */
function initTicketSalesChart() {
    fetch('/EventManager/GetTicketData')
        .then(res => res.json())
        .then(events => {
            const labels = events.map(e => e.name);
            const values = events.map(e => e.sold);

            const data = {
                labels: labels,
                datasets: [{
                    label: 'Tickets Sold',
                    data: values
                }]
            };

            const config = {
                type: 'polarArea',
                data: data,
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                            labels: {
                                color: '#e0e0e0',
                            }
                        },
                        title: {
                            display: true,
                            text: 'Ticket Sales Per Category',
                            font: {
                                size: 24,
                                weight: 'bold',
                            },
                            color: '#ffffff'
                        },
                    },
                }
            };

            const ctx = document.getElementById('ticketSalesByCategory').getContext('2d');
            new Chart(ctx, config);
        });
}

/**
 * Initializes the Monthly Revenue line chart
 */
function initMonthlyRevenueChart() {
    fetch('/EventManager/GetMonthlyRevenueData/')
        .then(res => res.json())
        .then(events => {
            if (!events || events.length === 0) {
                events = [
                    { month: "November 2025", revenue: 120 },
                    { month: "October 2025", revenue: 95 },
                    { month: "September 2025", revenue: 60 },
                ];
            }

            const labels = events.map(e => e.month);
            const values = events.map(e => e.revenue);

            const data = {
                labels: labels,
                datasets: [{
                    label: 'Revenue',
                    data: values,
                    borderColor: 'rgba(75, 192, 192, 1)',
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    pointStyle: 'circle',
                    pointRadius: 6,
                    pointHoverRadius: 10
                }]
            };

            const config = {
                type: 'line',
                data: data,
                options: {
                    responsive: true,
                    plugins: {
                        legend: { labels: { color: '#e0e0e0' } },
                        title: {
                            display: true,
                            text: 'Monthly Revenue',
                            font: { size: 24, weight: 'bold' },
                            color: '#ffffff'
                        }
                    },
                    scales: {
                        y: {
                            ticks: {
                                color: '#e0e0e0',
                                callback: function (value) {
                                    return '$' + value.toLocaleString();
                                }
                            },
                            grid: {
                                color: 'rgba(255, 255, 255, 0.1)'
                            }
                        },
                        x: {
                            ticks: {
                                color: '#e0e0e0',
                            },
                            grid: {
                                color: 'rgba(255, 255, 255, 0.1)'
                            }
                        }
                    }
                }
            };

            const ctx = document.getElementById('monthlyRevenue').getContext('2d');
            new Chart(ctx, config);
        });
}
