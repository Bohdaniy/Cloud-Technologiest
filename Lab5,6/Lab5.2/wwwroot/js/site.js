// Function to handle sample text selection
document.addEventListener('DOMContentLoaded', function () {
    // Highlight selected sample text
    const sampleCards = document.querySelectorAll('.sample-text-card');
    sampleCards.forEach(card => {
        card.addEventListener('click', function () {
            sampleCards.forEach(c => c.classList.remove('border-primary'));
            this.classList.add('border-primary');
            document.getElementById('SelectedTextId').value = this.dataset.id;
            document.getElementById('SelectedText').value = this.querySelector('.card-text').innerText;
        });
    });

    // Copy text to clipboard functionality
    document.querySelectorAll('.copy-text-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const textToCopy = this.dataset.text;
            navigator.clipboard.writeText(textToCopy).then(() => {
                const tooltip = new bootstrap.Tooltip(this, {
                    title: 'Copied!',
                    trigger: 'manual'
                });
                tooltip.show();
                setTimeout(() => tooltip.hide(), 1000);
            });
        });
    });
});