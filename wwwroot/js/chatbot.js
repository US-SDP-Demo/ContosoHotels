// Contoso Chatbot Frontend Logic
$(function () {
    var $chatbot = $('#contoso-chatbot');
    var $toggle = $('#contoso-chatbot-toggle');
    var $close = $('#contoso-chatbot-close');
    var $messages = $('#contoso-chatbot-messages');
    var $form = $('#contoso-chatbot-input');
    var $input = $('#contoso-chatbot-prompt');

    function appendMessage(text, sender) {
        var msgHtml = '<div class="contoso-chatbot-message ' + sender + '"><div class="contoso-chatbot-bubble">' + $('<div>').text(text).html() + '</div></div>';
        $messages.append(msgHtml);
        $messages.scrollTop($messages[0].scrollHeight);
    }

    $toggle.on('click', function () {
        $chatbot.fadeIn(200);
        $toggle.hide();
        $input.focus();
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
        // Simulate async bot response (replace with real API call if needed)
        setTimeout(function () {
            $messages.find('.contoso-chatbot-message.bot:last .contoso-chatbot-bubble').text('This is a sample response to: ' + prompt);
        }, 900);
    });

    // Optional: greet on open
    $toggle.one('click', function () {
        setTimeout(function () {
            appendMessage('Hello! How can I help you today at Contoso Hotels?', 'bot');
        }, 400);
    });
});
