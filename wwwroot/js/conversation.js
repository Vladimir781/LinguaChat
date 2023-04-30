var chatBox = document.querySelector('.chat_box');
let dotsInterval;

// Функция для создания сообщений на странице
function createMessageElement(sender, message) {
    const chatMessages = document.querySelector('.chat__messages');
    const messageElem = document.createElement('div');
    messageElem.classList.add('chat__message');
    messageElem.classList.add(`chat__message--${sender}`);
    const senderName = sender === "ai" ? "Assistant" : "User";
    //messageElem.innerHTML = `<span>${senderName}</span><p>${message}</p>`;
    if (message.startsWith('[') && message.endsWith(']')) {
        // Remove the brackets from the message
        message = message.slice(1, -1);
    }
    messageElem.innerHTML = `<span>${senderName}</span>`;
    messageElem.innerHTML += sender === "ai" ? `<p>${message}</p>` : `<p></p><p>${message}</p><p></p>`;
    chatMessages.appendChild(messageElem);
    return messageElem;
}


const sendButton = document.querySelector('.chat__button--send');

// создаем новый обработчик событий
const sendButtonClickHandler = (event) => {
    event.preventDefault(); // отменяем стандартное поведение формы

    // получаем данные формы
    const messageInput = document.querySelector('.chat__input');
    const message = messageInput.value.trim();

    if (message !== '') {
        // создаем сообщение от пользователя на странице
        createMessageElement('user', message);
        chatBox.scrollTop = chatBox.scrollHeight;
        // очищаем поле ввода
        messageInput.value = '';
        // блокируем форму и выводим надпись "Ожидание ответа"
        disableForm();

        // отправляем данные формы на сервер
        const xhr = new XMLHttpRequest();
        xhr.open('POST', '/Conversation/SetTopic');
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.onload = () => {
            if (xhr.status === 200) {
                try {
                    // обрабатываем ответ сервера
                    const response = xhr.responseText;
                    console.log('Server response:', response);
                    // выводим ответ сервера на странице в виде сообщения от ИИ
                    createMessageElement('ai', response);
                    chatBox.scrollTop = chatBox.scrollHeight;
                    // удаляем текущий обработчик событий
                    sendButton.removeEventListener('click', sendButtonClickHandler);
                    // добавляем новый обработчик событий
                    sendButton.addEventListener('click', newSendButtonClickHandler);
                } catch (error) {
                    console.error('Failed to parse server response:', error);
                }

            } else {
                console.error(xhr.statusText);
            }
            // возвращаем прежнее состояние формы
            // отображаем кнопки
            enableForm();
            // скрываем кнопки
            //document.querySelector('.chat__buttons').style.display = 'none';
        };
        xhr.onerror = () => console.error(xhr.statusText);
        xhr.send(JSON.stringify({ message: message }));
    }
};
const newSendButtonClickHandler = (event) => {
    event.preventDefault(); // отменяем стандартное поведение формы

    // получаем данные формы
    const messageInput = document.querySelector('.chat__input');
    const message = messageInput.value.trim();

    if (message !== '') {
        // создаем сообщение от пользователя на странице
        createMessageElement('user', message);
        chatBox.scrollTop = chatBox.scrollHeight;
        // очищаем поле ввода
        messageInput.value = '';
        // блокируем форму и выводим надпись "Ожидание ответа"
        disableForm();



        // отправляем данные формы на сервер
        const xhr = new XMLHttpRequest();
        xhr.open('POST', '/Conversation/SendMessage');
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.onload = () => {
            if (xhr.status === 200) {
                try {
                    // обрабатываем ответ сервера
                    const response = xhr.responseText;
                    console.log('Server response:', response);
                    // выводим ответ сервера на странице в виде сообщения от ИИ
                    createMessageElement('ai', response);
                    chatBox.scrollTop = chatBox.scrollHeight;


                } catch (error) {
                    console.error('Failed to parse server response:', error);
                }

            } else {
                console.error(xhr.statusText);
            }
            // возвращаем прежнее состояние формы
            // отображаем кнопки
            enableForm();
            // скрываем кнопки
            //document.querySelector('.chat__buttons').style.display = 'none';
        };
        xhr.onerror = () => console.error(xhr.statusText);
        xhr.send(JSON.stringify({ message: message }));
    }
};



// добавляем новый обработчик событий
sendButton.addEventListener('click', sendButtonClickHandler);







document.querySelector('.chat__button--clear').addEventListener('click', (event) => {
    event.preventDefault(); // отменяем станд
    // обновляем страницу
    location.reload();
});

// Функция для блокировки формы и вывода сообщения "Ожидание ответа"
function disableForm() {
    const messageInput = document.querySelector('.chat__input');
    const sendMessageBtn = document.querySelector('.chat__send-message-btn');


    messageInput.disabled = true;

    // Блокируем кнопку отправки
    sendMessageBtn.disabled = true;
    sendMessageBtn.classList.add('disabled');

    // добавляем сообщение ожидания в поле ввода
    messageInput.value = 'Waiting...';
    // создаем элемент для анимации точек
    dotsElem = document.createElement('span');
    dotsElem.textContent = '...';
    // добавляем элемент анимации к сообщению ожидания
    messageInput.insertAdjacentElement('beforeend', dotsElem);

    let dots = '';
    dotsInterval = setInterval(() => {
        if (dots.length < 3) {
            dots += '.';
        } else {
            dots = '';
        }
        messageInput.value = `Typing${dots}`;
    }, 500);
}



function enableForm() {
    const messageInput = document.querySelector('.chat__input');
    const sendMessageBtn = document.querySelector('.chat__send-message-btn');

    // удаляем сообщение ожидания и элемент анимации
    if (messageInput.lastChild.classList.contains('chat__dots')) {
        messageInput.value = '';
        messageInput.removeChild(messageInput.lastChild);
        // останавливаем анимацию
        clearInterval(dotsInterval);
    }

    // разблокируем поле ввода и кнопку отправки
    messageInput.disabled = false;
    sendMessageBtn.disabled = false;
    sendMessageBtn.classList.remove('disabled');

    // удаляем классы анимации из поля ввода
    messageInput.classList.remove('chat__input--loading');
    messageInput.classList.remove('chat__input--disabled');
    messageInput.value = '';
    // останавливаем анимацию точек
    clearInterval(dotsInterval);
}

