import React, { useEffect, useState, useRef } from 'react';
import './ChatWindow.css';
import { Message } from './Message';
import authConfig from '../auth_config.json';
import ApiClient from '../api/ApiClient';

function ChatWindow({ clearMessages }) {
    const [inputMessage, setInputMessage] = useState('');
    const [messages, setMessages] = useState([
        {
            role: 'Assistant',
            content: `Hi there! I'm StackSherpa, your assistant in summitting Jacob Thomas's technical skills, projects, and experience. \n\nAre you looking to explore Jacob's experience with AI, .NET, or Python, see if he might be a good fit for your team, or make an inquiry about his startup, Applied AI, or one of its products? \n\nBy the way, please remember that as an AI, I can make mistakes from time to time, so be sure to verify anything important with Jacob himself.`,
            timestamp: new Date().toISOString(),
        },
    ]);
    const [connection, setConnection] = useState(null);
    const chatEndRef = useRef(null);
    const profileName = 'portfolio-assistant';

    useEffect(() => {
        const setupSignalRConnection = async () => {
            const apiClient = new ApiClient(authConfig.ApiUri);
            const token = await apiClient.getStreamKey();

            if (token) {
                const newConnection = new window.signalR.HubConnectionBuilder()
                    .withUrl(authConfig.signalRHubUrl, {
                        accessTokenFactory: () => token
                    })
                    .withAutomaticReconnect()
                    .build();

                newConnection.on('broadcastMessage', (chunk) => {
                    console.log('Received chunk:', chunk); // Log the chunk data for debugging
                    setMessages((prevMessages) => {
                        const lastMessage = prevMessages[prevMessages.length - 1];
                        if (lastMessage && lastMessage.role === 'Assistant') {
                            if (!lastMessage.content) lastMessage.content = "";
                            const updatedMessage = {
                                ...lastMessage,
                                content: lastMessage.content + (chunk.completionUpdate)
                            };
                            console.log('Updated message:', updatedMessage); // Log the updated message for debugging
                            return [...prevMessages.slice(0, -1), updatedMessage];
                        } else {
                            const newMessage = {
                                role: 'Assistant',
                                content: chunk.CompletionUpdate || chunk.ErrorMessage,
                                timestamp: new Date().toISOString()
                            };
                            console.log('New message:', newMessage); // Log the new message for debugging
                            return [...prevMessages, newMessage];
                        }
                    });
                });

                try {
                    await newConnection.start();
                    setConnection(newConnection);
                } catch (error) {
                    console.error('SignalR Connection Error: ', error);
                }
            }
        };

        if (!connection) {
            setupSignalRConnection();
        }

        return () => {
            if (connection) {
                connection.stop().then(() => setConnection(null));
            }
        };
    }, [connection]);

    useEffect(() => {
        const messageList = chatEndRef.current?.parentNode; // Get the parent container of the chatEndRef
        if (messageList) {
            messageList.scrollTop = messageList.scrollHeight; // Scroll to the bottom of the message list
        }
    }, [messages]);

    const handleSendMessage = async () => {
        if (!inputMessage.trim()) {
            return;
        }

        if (connection) {
            const newMessage = {
                role: 'User',
                content: inputMessage,
                timestamp: new Date().toISOString()
            };
            const updatedMessages = [...messages, newMessage];
            setMessages(updatedMessages);

            const completionRequest = {
                ConversationId: null,
                ProfileOptions: {
                    Name: profileName
                },
                Messages: updatedMessages.map(msg => ({
                    Role: msg.role,
                    Content: msg.content,
                    Base64Image: msg.base64Image || null,
                    TimeStamp: msg.timestamp
                }))
            };
            setInputMessage('');
            await connection.invoke('Send', completionRequest);
        }
    };

    const handleClearMessages = () => {
        setMessages([]); // Clear the messages state
        if (clearMessages) {
            clearMessages(); // Call the parent-provided clearMessages function if it exists
        }
    };

    return (
        <div className="chat-window">
            <div className="message-list">
                {messages.map((msg, index) => (
                    <Message
                        key={index}
                        author={msg.role === "Assistant" ? "StackSherpa" : msg.role}
                        content={msg.content}
                        timestamp={msg.timestamp}
                        alignRight={msg.role === "Assistant"}
                    />
                ))}
                <div ref={chatEndRef} />
            </div>
            <div className="chat-input">
                <div className="chat-input-input-container">
                    <input
                        type="text"
                        value={inputMessage}
                        onChange={(e) => setInputMessage(e.target.value)}
                        placeholder="Ask a question..."
                        onKeyDown={(e) => e.key === 'Enter' && handleSendMessage()}
                    />
                </div>
                <button className="toolbar-button" onClick={handleSendMessage}>Send</button>
                <button className="toolbar-button" onClick={handleClearMessages}>Reset</button>
            </div>
        </div>
    );
}

export default ChatWindow;

