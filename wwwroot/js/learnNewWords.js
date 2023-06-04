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

document.querySelector('.chat__button--topic').addEventListener('click', (event) => {
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
        xhr.open('POST', '/LearnNewWords/SelectTopic');
        xhr.setRequestHeader('Content-Type', 'application/json');

        xhr.onload = () => {
            if (xhr.status === 200) {
                try {
                    // обрабатываем ответ сервера
                    const response = xhr.responseText;
                    console.log('Server response:', response);
                    // выводим ответ сервера на странице в виде сообщения от ИИ
                    createMessageElement('ai', response);
                    // получаем список элементов списка
                    const listItems = document.querySelectorAll('.chat_box ul li, .chat_box ol li');


                    // добавляем обработчик клика на каждый элемент списка
                    // добавляем обработчик клика на каждый элемент списка
                    listItems.forEach((item) => {
                        item.addEventListener('click', (event) => {
                            event.preventDefault(); // отменяем стандартное поведение ссылки
                            disableForm();
                            // получаем значение элемента списка
                            const listItemValue = item.textContent.trim();

                            // отправляем данные на сервер
                            const xhr = new XMLHttpRequest();
                            xhr.open('POST', '/LearnNewWords/GetWord');
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
                                enableForm();
                            };
                            xhr.onerror = () => console.error(xhr.statusText);
                            console.dir(listItemValue);
                            xhr.send(JSON.stringify({ message: listItemValue }));
                        });
                    });




                    chatBox.scrollTop = chatBox.scrollHeight;


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



document.querySelector('.chat__button--ask').addEventListener('click', (event) => {
    event.preventDefault(); // отменяем стандартное поведение формы

    // получаем ссылку на кнопку "Ask question"
    const questionButton = document.querySelector('.chat__button--ask');

    // скрываем кнопку "Ask question"
    questionButton.style.display = 'none';

    // создаем элемент формы
    const formElement = document.createElement('form');
    formElement.classList.add('chat__form--ask');
    formElement.classList.add('mx-auto'); 
    formElement.innerHTML = '<input class="chat__input--ask" type="text" placeholder="Write your question...">' +
        '<button class="chat__button chat__button--ask">Отправить</button>';

    // вставляем форму после кнопки "Ask question"
    questionButton.parentElement.insertBefore(formElement, questionButton.nextSibling);

    // добавляем обработчик событий для формы
    formElement.addEventListener('submit', (event) => {
        event.preventDefault(); // отменяем стандартное поведение формы

        // получаем данные формы
        const messageInput = formElement.querySelector('.chat__input--ask');
        const message = messageInput.value.trim();

        if (message !== '') {
            messageInput.value = '';
            disableForm();
            createMessageElement('user', message);
            chatBox.scrollTop = chatBox.scrollHeight;
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
                        chatBox.scrollTop = chatBox.scrollHeight;

                    } catch (error) {
                        console.error('Failed to parse server response:', error);
                    }

                } else {
                    console.error(xhr.statusText);
                }
                // возвращаем прежнее состояние формы
                enableForm();
                // удаляем форму
                formElement.remove();
                // показываем кнопку "Ask question"
                questionButton.style.display = 'block';
            }
            xhr.send(JSON.stringify({ message: message }));
        }
    });
});

document.querySelector('.chat__button--clear').addEventListener('click', (event) => {
    event.preventDefault(); // отменяем станд
    // обновляем страницу
    location.reload();
});

// Функция для блокировки формы и вывода сообщения "Ожидание ответа"
function disableForm() {
    const messageInput = document.querySelector('.chat__input');
    const messageInputAsk = document.querySelector('.chat__input--ask');
    const sendMessageBtn = document.querySelector('.chat__send-message-btn');

    // Блокируем поля ввода перед добавлением сообщения ожидания
    if (messageInputAsk) {
        messageInputAsk.disabled = true;
    }

    messageInput.disabled = true;

    // Блокируем кнопку отправки
    sendMessageBtn.disabled = true;
    sendMessageBtn.classList.add('disabled');

    const buttons = document.querySelectorAll('.chat__button--ask');
    buttons.forEach(button => {
        button.classList.add('disabled');
        button.disabled = true;
    });
    if (messageInputAsk) {
        // добавляем сообщение ожидания в поле ввода
        messageInputAsk.value = 'Waiting...';
        // создаем элемент для анимации точек
        let dotsElem = document.createElement('span');
        dotsElem.textContent = '...';
        // добавляем элемент анимации к сообщению ожидания
        messageInputAsk.insertAdjacentElement('beforeend', dotsElem);
    }
    // добавляем сообщение ожидания в поле ввода
    messageInput.value = 'Typing...';
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
        if (messageInputAsk) {
            messageInputAsk.value = `Waiting${dots}`;
        }
        messageInput.value = `Typing${dots}`;
    }, 500);
}



function enableForm() {
    const messageInput = document.querySelector('.chat__input');
    const sendMessageBtn = document.querySelector('.chat__send-message-btn');

    const buttons = document.querySelectorAll('.chat__button--ask');
    buttons.forEach(button => {
        button.classList.remove('disabled');
        button.disabled = false;
    });

    // удаляем сообщение ожидания и элемент анимации
    if (messageInput.lastChild.classList.contains('chat__dots')) {
        messageInput.value = '';
        messageInput.removeChild(messageInput.lastChild);
        // останавливаем анимацию
        clearInterval(dotsInterval);
    }
    const askInput = document.querySelector('.chat__input--ask');
    if (askInput) {
        askInput.disabled = false;
        askInput.value = '';
        askInput.classList.remove('disabled');
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

