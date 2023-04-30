const chatInput = document.querySelector('.chat__input');
if (chatInput) {
    chatInput.addEventListener('keydown', (event) => {
        if (event.key === 'Enter') {
            event.preventDefault();
            document.querySelector('.chat__send-message-btn').click();
        }
    });
}
