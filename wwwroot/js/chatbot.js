// Contoso Chatbot Frontend Logic
$(function () {
    var $chatbot = $('#contoso-chatbot');
    var $toggle = $('#contoso-chatbot-toggle');
    var $close = $('#contoso-chatbot-close');
    var $messages = $('#contoso-chatbot-messages');
    var $form = $('#contoso-chatbot-input');
    var $input = $('#contoso-chatbot-prompt');

    var userInfo = null; // Store user information

    function appendMessage(text, sender) {
        var msgHtml = '<div class="contoso-chatbot-message ' + sender + '"><div class="contoso-chatbot-bubble">' + $('<div>').text(text).html() + '</div></div>';
        $messages.append(msgHtml);
        $messages.scrollTop($messages[0].scrollHeight);
    }

    function fetchUserInfo() {
        $.ajax({
            url: '/api/CustomerInfo/customer/david.allen59@email.com',
            method: 'GET',
            success: function (response) {
                userInfo = response;
                console.log('User info loaded:', userInfo);
            },
            error: function (xhr, status, error) {
                console.error('Failed to load user info:', error);
            }
        });
    }

    $toggle.on('click', function () {
        $chatbot.fadeIn(200);
        $toggle.hide();
        $input.focus();
        
        // Fetch user info when chatbot is first opened
        if (!userInfo) {
            fetchUserInfo();
        }
    });
    $close.on('click', function () {
        $chatbot.fadeOut(200);
        $toggle.show();
    });

    $form.on('submit', function (e) {
        e.preventDefault();
        var prompt = $input.val().trim();
        if (!prompt) return;
        
        appendMessage(prompt, 'user');
        $input.val('');
        appendMessage('Let me think...', 'bot');
        
        // Send to backend instead of simulated response
        $.ajax({
            url: '/api/Chatbot/message',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(
                { 
                    message: prompt,
                    guestName: userInfo.firstName + ' ' + userInfo.lastName,
                    guestId: userInfo.customerId
                }
            ),
            success: function (response) {
                // Replace the "thinking" message with actual response
                $messages.find('.contoso-chatbot-message.bot:last .contoso-chatbot-bubble').text(response.message);
            },
            error: function (xhr, status, error) {
                console.error('Chatbot error:', error);
                $messages.find('.contoso-chatbot-message.bot:last .contoso-chatbot-bubble').text('Sorry, I\'m having trouble responding right now. Please try again later.');
            }
        });
    });

    // Optional: greet on open
    $toggle.one('click', function () {
        setTimeout(function () {
            appendMessage('Hello! How can I help you today at Contoso Hotels?', 'bot');
        }, 400);
    });
});
