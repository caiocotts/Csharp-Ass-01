/**
 * Purchase Confirmation - Quantity Controls
 * Handles the +/- buttons and price calculation
 */

/**
 * Initializes the quantity controls for ticket purchasing
 * @param {number} pricePerTicket - The price per ticket
 */
function initQuantityControls(pricePerTicket) {
    const quantityInput = document.getElementById('quantity');
    if (!quantityInput) return;

    const priceDisplay = document.getElementById('price');

    const updatePrice = () => {
        const quantity = parseInt(quantityInput.value, 10);
        const total = (quantity * pricePerTicket).toFixed(2);
        priceDisplay.textContent = `Total Price: $${total}`;
    };

    document.querySelectorAll('[data-adjust]').forEach(button => {
        button.addEventListener('click', () => {
            const delta = parseInt(button.getAttribute('data-adjust'), 10);
            const min = parseInt(quantityInput.min, 10);
            const max = parseInt(quantityInput.max, 10);
            const nextValue = parseInt(quantityInput.value, 10) + delta;

            if (nextValue >= min && nextValue <= max) {
                quantityInput.value = nextValue;
                updatePrice();
            }
        });
    });

    updatePrice();
}
