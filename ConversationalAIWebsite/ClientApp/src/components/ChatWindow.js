import React, { useEffect, useState, useRef } from 'react';
import './ChatWindow.css';
import { Message } from './Message';

function ChatWindow({ toggleSplit, isSmallViewport, messages, sendMessage }) {
    const [inputMessage, setInputMessage] = useState('');
    const chatEndRef = useRef(null);

    useEffect(() => {
        chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    const handleSendMessage = () => {
        sendMessage(inputMessage);
        setInputMessage('');
    };

    return (
        <div className="chat-window">
            <div className="message-list">
                {messages.map((msg, index) => (
                    <Message
                        key={index}
                        author={msg.user}
                        message={msg.message}
                        timestamp={msg.timestamp}
                        alignRight={msg.user === "Assistant"}
                    />
                ))}
                <div ref={chatEndRef} />
            </div>
            <div className="chat-input">
                <input
                    type="text"
                    value={inputMessage}
                    onChange={(e) => setInputMessage(e.target.value)}
                    placeholder="Type your message..."
                    onKeyDown={(e) => e.key === 'Enter' && handleSendMessage()}
                />
                <button onClick={handleSendMessage}>Send</button>
                <button
                    className="toggleSplit"
                    onClick={toggleSplit}
                    style={{ display: isSmallViewport ? 'none' : 'block' }}
                >
                    Toggle Split
                </button>
            </div>
        </div>
    );
}

export default ChatWindow;
