// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
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

// обработчик нажатия клавиши Enter в поле ввода
document.querySelector('.chat__input').addEventListener('keydown', (event) => {
    if (event.key === 'Enter') {
        event.preventDefault(); // отменяем стандартное поведение формы
        document.querySelector('.chat__send-message-btn').click(); // имитируем нажатие на кнопку отправки сообщения
    }
});

const correctButton = document.querySelector('.chat__button--correct');
if (correctButton) {
    document.querySelector('.chat__button--correct').addEventListener('click', (event) => {
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
            xhr.open('POST', '/Grammar/GetCorrects');
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
                document.querySelector('.chat__buttons').style.display = 'block';
                enableForm();
                // скрываем кнопки
                //document.querySelector('.chat__buttons').style.display = 'none';
            };
            xhr.onerror = () => console.error(xhr.statusText);
            xhr.send(JSON.stringify({ message: message }));
        }
    });
}

const topicButton = document.querySelector('.chat__button--topic');
if (topicButton) {
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
}

const rulesButton = document.querySelector('.chat__button--rules');
if (rulesButton) {
    // обработчик клика на кнопку отправки сообщения
    document.querySelector(rulesButton).addEventListener('click', (event) => {
        event.preventDefault(); // отменяем стандартное поведение формы

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
        }
        xhr.send();
    });
}

const examplesButton = document.querySelector('.chat__button--examples');
if (examplesButton) {
    // обработчик клика на кнопку отправки сообщения
    document.querySelector(examplesButton).addEventListener('click', (event) => {
        event.preventDefault(); // отменяем стандартное поведение формы

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
                    var chatBox = document.querySelector('.chat_box');
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
        }
        xhr.send();
    });
}
const askButton = document.querySelector('.chat__button--ask');
if (askButton) {
    // добавляем обработчик событий для кнопки "Ask question"
    document.querySelector(askButton).addEventListener('click', (event) => {
        event.preventDefault(); // отменяем стандартное поведение формы

        // получаем ссылку на кнопку "Ask question"
        const questionButton = document.querySelector('.chat__button--ask');

        // скрываем кнопку "Ask question"
        questionButton.style.display = 'none';

        // создаем элемент формы
        const formElement = document.createElement('form');
        formElement.classList.add('chat__form--ask');
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
                    /*// удаляем форму
                    formElement.remove();
                    // показываем кнопку "Ask question"
                    questionButton.style.display = 'block';*/
                }
                xhr.send(JSON.stringify({ message: message }));
            }
        });
    });
}

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

    const buttons = document.querySelectorAll('.chat__button--rules, .chat__button--examples, .chat__button--correct, .chat__button--ask, .chat__button--topic');
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
    messageInput.value = 'Correcting your text...';
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
        messageInput.value = `Correcting your text${dots}`;
    }, 500);
}


// Функция для разблокировки формы и скрытия сообщения "Ожидание ответа"
function enableForm() {
    const messageInput = document.querySelector('.chat__input');
    const sendMessageBtn = document.querySelector('.chat__send-message-btn');
    const messageInputAsk = document.querySelector('.chat__input--ask');

    const buttons = document.querySelectorAll('.chat__button--rules, .chat__button--examples, .chat__button--correct, .chat__button--ask, .chat__button--topic');
    buttons.forEach(button => {
        button.classList.remove('disabled');
        button.disabled = false;
    });


    // удаляем сообщение ожидания и элемент анимации
    if (messageInputAsk) {
        messageInputAsk.value = '';
    }
    messageInput.value = '';
    messageInput.removeChild(messageInput.lastChild);
    // останавливаем анимацию
    clearInterval(dotsInterval);

    // разблокируем поле ввода и кнопку отправки
    if (messageInputAsk) {
        messageInputAsk.disabled = false;
    }
    messageInput.disabled = false;
    sendMessageBtn.disabled = false;
    sendMessageBtn.classList.remove('disabled');
}


