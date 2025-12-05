/**
 * Shopping Cart - Badge Update
 * Updates the cart badge count in the navbar
 */

/**
 * Updates the cart badge with the current item count
 * @param {string} getCartCountUrl - The URL to fetch the cart count
 */
function updateCartBadge(getCartCountUrl) {
    $.get(getCartCountUrl, function (data) {
        var badge = $('#cart-badge');
        if (data.count > 0) {
            badge.text(data.count).show();
        } else {
            badge.hide();
        }
    });
}
