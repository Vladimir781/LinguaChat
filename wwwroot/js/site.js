// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
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


// обработчик клика на кнопку отправки сообщения
document.querySelector('.chat__send-message-btn').addEventListener('click', (event) => {
    event.preventDefault(); // отменяем стандартное поведение формы

    // получаем данные формы
    const messageInput = document.querySelector('.chat__input');
    const message = messageInput.value.trim();

    if (message !== '') {
        // создаем сообщение от пользователя на странице
        createMessageElement('user', message);
        // очищаем поле ввода
        messageInput.value = '';
        // блокируем форму и выводим надпись "Ожидание ответа"
        disableForm();



        // отправляем данные формы на сервер
        const xhr = new XMLHttpRequest();
        xhr.open('POST', '/Grammar/SendMessage');
        xhr.setRequestHeader('Content-Type', 'application/json');

        xhr.onload = () => {
            if (xhr.status === 200) {
                try {
                    // обрабатываем ответ сервера
                    const response = xhr.responseText;
                    console.log('Server response:', response);
                    // выводим ответ сервера на странице в виде сообщения от ИИ
                    createMessageElement('ai', response);

                } catch (error) {
                    console.error('Failed to parse server response:', error);
                }

            } else {
                console.error(xhr.statusText);
            }
            // возвращаем прежнее состояние формы
            // отображаем кнопки
            document.querySelector('.chat__buttons').style.display = 'block';
            enableForm();
            // скрываем кнопки
            //document.querySelector('.chat__buttons').style.display = 'none';
        };
        xhr.onerror = () => console.error(xhr.statusText);
        xhr.send(JSON.stringify({ message: message }));
    }
});

// обработчик клика на кнопку отправки сообщения
document.querySelector('.chat__button--rules').addEventListener('click', (event) => {
    event.preventDefault(); // отменяем стандартное поведение формы

    // получаем данные формы
    //const messageInput = document.querySelector('.chat__input');
    //const message = messageInput.value.trim();
    disableForm();



    // отправляем данные формы на сервер
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Grammar/GetRules');
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onload = () => {
        if (xhr.status === 200) {
            try {
                // обрабатываем ответ сервера
                const response = xhr.responseText;
                console.log('Server response:', response);
                // выводим ответ сервера на странице в виде сообщения от ИИ
                createMessageElement('ai', response);

            } catch (error) {
                console.error('Failed to parse server response:', error);
            }

        } else {
            console.error(xhr.statusText);
        }
        // возвращаем прежнее состояние формы
        // отображаем кнопки
        document.querySelector('.chat__buttons').style.display = 'block';
        enableForm();
        // скрываем кнопки
        //document.querySelector('.chat__buttons').style.display = 'none';
    }
    xhr.send();
});

// обработчик клика на кнопку отправки сообщения
document.querySelector('.chat__button--examples').addEventListener('click', (event) => {
    event.preventDefault(); // отменяем стандартное поведение формы

    // получаем данные формы
    //const messageInput = document.querySelector('.chat__input');
    //const message = messageInput.value.trim();
    disableForm();



    // отправляем данные формы на сервер
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Grammar/GetExamples');
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onload = () => {
        if (xhr.status === 200) {
            try {
                // обрабатываем ответ сервера
                const response = xhr.responseText;
                console.log('Server response:', response);
                // выводим ответ сервера на странице в виде сообщения от ИИ
                createMessageElement('ai', response);

            } catch (error) {
                console.error('Failed to parse server response:', error);
            }

        } else {
            console.error(xhr.statusText);
        }
        // возвращаем прежнее состояние формы
        // отображаем кнопки
        document.querySelector('.chat__buttons').style.display = 'block';
        enableForm();
        // скрываем кнопки
        //document.querySelector('.chat__buttons').style.display = 'none';
    }
    xhr.send();
});


let dotsInterval;

// Функция для блокировки формы и вывода сообщения "Ожидание ответа"
function disableForm() {
    const messageInput = document.querySelector('.chat__input');
    const sendMessageBtn = document.querySelector('.chat__send-message-btn');

    const buttons = document.querySelectorAll('.chat__button--rules, .chat__button--examples');
    buttons.forEach(button => {
        button.classList.add('disabled');
        button.disabled = true;
    });


    // добавляем сообщение ожидания в поле ввода
    messageInput.value = 'Waiting for a response...';
    // создаем элемент для анимации точек
    const dotsElem = document.createElement('span');
    dotsElem.textContent = '...';
    // добавляем элемент анимации к сообщению ожидания
    messageInput.insertAdjacentElement('beforeend', dotsElem);
    // запускаем анимацию

    let dots = '';
    dotsInterval = setInterval(() => {
        if (dots.length < 3) {
            dots += '.';
        } else {
            dots = '';
        }
        messageInput.value = `Waiting for a response${dots}`;
    }, 500);

    // блокируем поле ввода и кнопку отправки
    messageInput.disabled = true;
    sendMessageBtn.disabled = true;
    sendMessageBtn.classList.add('disabled');
}

// Функция для разблокировки формы и скрытия сообщения "Ожидание ответа"
function enableForm() {
    const messageInput = document.querySelector('.chat__input');
    const sendMessageBtn = document.querySelector('.chat__send-message-btn');


    const buttons = document.querySelectorAll('.chat__button--rules, .chat__button--examples');
    buttons.forEach(button => {
        button.classList.remove('disabled');
        button.disabled = false;
    });


    // удаляем сообщение ожидания и элемент анимации
    messageInput.value = '';
    messageInput.removeChild(messageInput.lastChild);
    // останавливаем анимацию
    clearInterval(dotsInterval);

    // разблокируем поле ввода и кнопку отправки
    messageInput.disabled = false;
    sendMessageBtn.disabled = false;
    sendMessageBtn.classList.remove('disabled');
}

