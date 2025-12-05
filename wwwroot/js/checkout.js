/**
 * Checkout Page - Success Modal with Confetti
 * Requires particles.js to be loaded
 */

/**
 * Initializes the checkout success modal with confetti effect
 * @param {boolean} showModal - Whether to show the success modal
 */
function initCheckoutSuccess(showModal) {
    if (!showModal) return;

    const particlesContainer = document.getElementById('particles-js');
    if (!particlesContainer) return;

    particlesContainer.style.display = 'block';

    particlesJS('particles-js', {
        "particles": {
            "number": {
                "value": 150,
                "density": {
                    "enable": true,
                    "value_area": 800
                }
            },
            "color": {
                "value": ["#ff0000", "#00ff00", "#0000ff", "#ffff00", "#ff00ff", "#00ffff", "#ffa500", "#ff1493"]
            },
            "shape": {
                "type": ["circle", "triangle", "polygon"],
                "polygon": {
                    "nb_sides": 6
                }
            },
            "opacity": {
                "value": 0.9,
                "random": true,
                "anim": {
                    "enable": true,
                    "speed": 1,
                    "opacity_min": 0.1,
                    "sync": false
                }
            },
            "size": {
                "value": 8,
                "random": true,
                "anim": {
                    "enable": false
                }
            },
            "line_linked": {
                "enable": false
            },
            "move": {
                "enable": true,
                "speed": 8,
                "direction": "bottom",
                "random": true,
                "straight": false,
                "out_mode": "out",
                "bounce": false,
                "attract": {
                    "enable": false
                }
            }
        },
        "interactivity": {
            "detect_on": "canvas",
            "events": {
                "onhover": {
                    "enable": false
                },
                "onclick": {
                    "enable": false
                },
                "resize": true
            }
        },
        "retina_detect": true
    });

    const successModal = new bootstrap.Modal(document.getElementById('successModal'));
    successModal.show();

    setTimeout(function () {
        $('#particles-js').fadeOut(1000);
    }, 5000);
}
