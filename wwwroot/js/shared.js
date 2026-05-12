/* ============================================
   FUREVER HOME - SHARED JS
   ============================================ */

// Hamburger menu toggle
const hamburger = document.getElementById('hamburgerBtn');
const navLinks = document.getElementById('navLinks');

if (hamburger && navLinks) {
    hamburger.addEventListener('click', () => {
        navLinks.classList.toggle('open');
        const isOpen = navLinks.classList.contains('open');
        hamburger.setAttribute('aria-expanded', isOpen);
    });

    // Close on outside click
    document.addEventListener('click', (e) => {
        if (!hamburger.contains(e.target) && !navLinks.contains(e.target)) {
            navLinks.classList.remove('open');
        }
    });
}

// Navbar scroll shadow
const navbar = document.getElementById('mainNavbar');
if (navbar) {
    window.addEventListener('scroll', () => {
        if (window.scrollY > 10) {
            navbar.style.boxShadow = '0 4px 24px rgba(47,106,53,0.14)';
        } else {
            navbar.style.boxShadow = '0 2px 16px rgba(47,106,53,0.08)';
        }
    });
}

// Filter pills
document.querySelectorAll('.filter-pill').forEach(pill => {
    pill.addEventListener('click', function () {
        const group = this.closest('.filter-bar');
        group.querySelectorAll('.filter-pill').forEach(p => p.classList.remove('active'));
        this.classList.add('active');
    });
});

// Password toggle
document.querySelectorAll('[data-toggle-password]').forEach(btn => {
    btn.addEventListener('click', function () {
        const targetId = this.dataset.togglePassword;
        const input = document.getElementById(targetId);
        if (!input) return;
        if (input.type === 'password') {
            input.type = 'text';
            this.textContent = '🙈';
        } else {
            input.type = 'password';
            this.textContent = '👁️';
        }
    });
});

// Image upload preview
document.querySelectorAll('[data-upload-preview]').forEach(input => {
    input.addEventListener('change', function () {
        const previewId = this.dataset.uploadPreview;
        const preview = document.getElementById(previewId);
        if (!preview || !this.files[0]) return;
        const reader = new FileReader();
        reader.onload = e => {
            preview.src = e.target.result;
            preview.style.display = 'block';
        };
        reader.readAsDataURL(this.files[0]);
    });
});

// Drag & drop upload area
document.querySelectorAll('.upload-area').forEach(area => {
    area.addEventListener('dragover', e => {
        e.preventDefault();
        area.style.borderColor = 'var(--green-primary)';
        area.style.background = 'var(--green-light)';
    });
    area.addEventListener('dragleave', () => {
        area.style.borderColor = '';
        area.style.background = '';
    });
    area.addEventListener('drop', e => {
        e.preventDefault();
        area.style.borderColor = '';
        area.style.background = '';
        const files = e.dataTransfer.files;
        if (files.length) {
            const fileInput = area.querySelector('input[type="file"]');
            if (fileInput) {
                const dt = new DataTransfer();
                dt.items.add(files[0]);
                fileInput.files = dt.files;
                fileInput.dispatchEvent(new Event('change'));
            }
        }
    });
});

// Number counter animation
function animateCounter(el) {
    const target = parseInt(el.dataset.target || el.textContent, 10);
    const duration = 1500;
    const start = performance.now();
    const initial = 0;

    function update(now) {
        const elapsed = now - start;
        const progress = Math.min(elapsed / duration, 1);
        const eased = 1 - Math.pow(1 - progress, 3);
        el.textContent = Math.round(initial + (target - initial) * eased) + (el.dataset.suffix || '');
        if (progress < 1) requestAnimationFrame(update);
    }
    requestAnimationFrame(update);
}

const counterObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting && !entry.target.dataset.animated) {
            entry.target.dataset.animated = 'true';
            animateCounter(entry.target);
        }
    });
}, { threshold: 0.5 });

document.querySelectorAll('[data-counter]').forEach(el => counterObserver.observe(el));