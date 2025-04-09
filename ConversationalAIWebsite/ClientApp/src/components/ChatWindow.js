import React, { useEffect, useState, useRef } from 'react';
import './ChatWindow.css';
import { Message } from './Message';
import authConfig from '../auth_config.json';
import ApiClient from '../api/ApiClient';
import { useAuth0 } from '@auth0/auth0-react';

function ChatWindow({ clearMessages }) {
    const [inputMessage, setInputMessage] = useState('');
    const [messages, setMessages] = useState([]);
    const [connection, setConnection] = useState(null);
    const chatEndRef = useRef(null);
    const { getAccessTokenSilently, user } = useAuth0();
    const testProfile = 'test';

    useEffect(() => {
        const setupSignalRConnection = async () => {
            const apiClient = new ApiClient(authConfig.ApiUri, getAccessTokenSilently);
            const token = await apiClient.getStreamKey(user.sub);

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

        if (user && !connection) {
            setupSignalRConnection();
        }

        return () => {
            if (connection) {
                connection.stop().then(() => setConnection(null));
            }
        };
    }, [connection, getAccessTokenSilently, user]);

    useEffect(() => {
        chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    const handleSendMessage = async () => {
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
                    Name: testProfile
                },
                Messages: updatedMessages.map(msg => ({
                    Role: msg.role,
                    User: user.sub,
                    Content: msg.content,
                    Base64Image: msg.base64Image || null,
                    TimeStamp: msg.timestamp
                }))
            };
            setInputMessage('');
            await connection.invoke('Send', completionRequest);
        }
    };

    return (
        <div className="chat-window">
            <div className="message-list">
                {messages.map((msg, index) => (
                    <Message
                        key={index}
                        author={msg.role}
                        content={msg.content}
                        timestamp={msg.timestamp}
                        alignRight={msg.role === "Assistant"}
                    />
                ))}
                <div ref={chatEndRef} />
            </div>
            <div className="chat-input">
                <div className="chat-input-input-container">
                    <label>Chat Input</label>
                    <input
                        type="text"
                        value={inputMessage}
                        onChange={(e) => setInputMessage(e.target.value)}
                        placeholder="Start chatting..."
                        onKeyDown={(e) => e.key === 'Enter' && handleSendMessage()}
                    />
                </div>
                <button className="toolbar-button" onClick={handleSendMessage}>Send</button>
                <button className="toolbar-button" onClick={clearMessages}>Reset</button>
            </div>
        </div>
    );
}

export default ChatWindow;
