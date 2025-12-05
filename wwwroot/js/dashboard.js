/**
 * Dashboard Tab Management
 * Handles AJAX loading of tab content and modal initialization
 */
document.addEventListener('DOMContentLoaded', function () {
    const tabButtons = document.querySelectorAll('#dashboardTabs button[data-bs-toggle="tab"]');
    const loadedTabs = new Set();

    /**
     * Loads content for a tab via AJAX
     * @param {HTMLElement} tabButton - The tab button element
     */
    function loadTabContent(tabButton) {
        const targetId = tabButton.getAttribute('data-bs-target');
        const url = tabButton.getAttribute('data-url');
        const targetPane = document.querySelector(targetId);

        // Skip if already loaded
        if (loadedTabs.has(targetId)) return;

        fetch(url)
            .then(response => response.text())
            .then(html => {
                targetPane.innerHTML = html;
                loadedTabs.add(targetId);
                initializeModals(targetPane);
            })
            .catch(error => {
                targetPane.innerHTML = '<div class="alert alert-danger">Failed to load content.</div>';
                console.error('Error loading tab:', error);
            });
    }

    /**
     * Initializes Bootstrap modals within dynamically loaded content
     * @param {HTMLElement} container - The container with the loaded content
     */
    function initializeModals(container) {
        // Rating modal handler
        const ratingModal = container.querySelector('#ratingModal');
        if (ratingModal) {
            ratingModal.addEventListener('show.bs.modal', function (event) {
                const button = event.relatedTarget;
                const id = button.getAttribute('data-id');
                const modalInput = ratingModal.querySelector('#ratingPurchaseId');
                modalInput.value = id;
            });
        }

        // QR code modal handler
        const qrModal = container.querySelector('#qrModal');
        if (qrModal) {
            qrModal.addEventListener('show.bs.modal', function (event) {
                const button = event.relatedTarget;
                const purchaseId = button.getAttribute('data-qr-id');
                const modalImage = qrModal.querySelector('#modalQrImage');
                modalImage.src = '/Dashboard/GenerateQr?id=' + purchaseId;
            });

            qrModal.addEventListener('hidden.bs.modal', function () {
                const modalImage = qrModal.querySelector('#modalQrImage');
                modalImage.src = '';
            });
        }
    }

    // Listen for tab changes
    tabButtons.forEach(button => {
        button.addEventListener('shown.bs.tab', function () {
            loadTabContent(this);
        });
    });

    // Load initial tab content
    const activeTab = document.querySelector('#dashboardTabs button.active');
    if (activeTab) {
        loadTabContent(activeTab);
    }
});
