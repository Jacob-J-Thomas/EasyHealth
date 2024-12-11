import React, { useEffect, useState, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { v4 as uuidv4 } from 'uuid';
import AuthClient from '../api/AuthClient';
import ApiClient from '../api/ApiClient';
import './ChatWindow.css';
import { Message } from './Message';

const CHAT_API_URL = 'https://localhost:53337/chatstream';
const CONNECTION_LOST_MESSAGE = "Oops! It seems I lost my connection. Could you please repeat your last message? If this issue persists, please contact support at applied.ai.help@gmail.com.";
const CONNECTION_FAILED_MESSAGE = "It looks like I'm having trouble connecting to the server. Please ensure your internet connection is stable and try again, or contact support at applied.ai.help@gmail.com if this issue persists.";
const authClient = new AuthClient("https://localhost:7228"); // Initialize AuthClient
const apiClient = new ApiClient("https://localhost:7228"); // Initialize AuthClient
let conversationId = "";

function ChatWindow({ toggleSplit, isSmallViewport }) {
    const [messages, setMessages] = useState([]);
    const [inputMessage, setInputMessage] = useState('');
    const chatEndRef = useRef(null);
    const [connection, setConnection] = useState(null);
    
    const [connectionError, setConnectionError] = useState(false);
    const hubConnectionErrorMessageSent = useRef(false);
    

    useEffect(() => {
        const establishConnection = async () => {
            try {
                // Fetch the auth token
                const tokenResponse = await authClient.authorize();
                const accessToken = tokenResponse.access_token;

                // Start a new Hangman Session
                var gameData = await apiClient.startHangmanGame();

                conversationId = gameData.conversationId;
                const isFailedGuess = gameData.increment;
                const gameStartMessage = gameData.message;

                // Update messages with the Hangman game start response
                setMessages((prevMessages) => [
                    ...prevMessages,
                    {
                        user: "Assistant",
                        message: gameStartMessage,
                        timestamp: new Date().toLocaleTimeString(),
                    },
                ]);

                // Establish SignalR connection with the access token
                const newConnection = new signalR.HubConnectionBuilder()
                    .withUrl(CHAT_API_URL, {
                        accessTokenFactory: () => accessToken, // Use the access token for authentication
                    })
                    .withAutomaticReconnect() // Automatically reconnect
                    .build();

                newConnection.on('broadcastMessage', (response) => {
                    const author = response.role;
                    const messageChunk = response.completionUpdate;
                    const timestamp = new Date().toLocaleTimeString();

                    setMessages((prevMessages) => {
                        const updatedMessages = [...prevMessages];
                        const lastMessage = prevMessages[prevMessages.length - 1];

                        if (lastMessage && lastMessage.user === author) {
                            const updatedMessage = (lastMessage.message || "") + messageChunk;
                            updatedMessages[prevMessages.length - 1] = {
                                ...lastMessage,
                                author: author,
                                message: updatedMessage,
                                timestamp: timestamp,
                            };
                        } else if (messageChunk && messageChunk.trim() !== "") {
                            updatedMessages.push({
                                user: author,
                                message: messageChunk,
                                timestamp: timestamp,
                            });
                        }

                        return updatedMessages;
                    });

                    console.log(`Received chunk from AIHub: ${author}: ${messageChunk}`);
                });

                newConnection.onclose(() => {
                    if (!hubConnectionErrorMessageSent.current) {
                        setMessages((prevMessages) => [
                            ...prevMessages,
                            {
                                user: "Assistant",
                                message: CONNECTION_LOST_MESSAGE,
                                timestamp: new Date().toLocaleTimeString(),
                            },
                        ]);
                        hubConnectionErrorMessageSent.current = true;
                    }
                    setConnectionError(true);
                });

                await newConnection.start();
                console.log('Connected to SignalR hub');
                setConnection(newConnection);
                setConnectionError(false);
                hubConnectionErrorMessageSent.current = false; // Reset the flag when connection is successful
            } catch (error) {
                console.error('Connection failed: ', error);
                if (!hubConnectionErrorMessageSent.current) {
                    setMessages((prevMessages) => [
                        ...prevMessages,
                        {
                            user: "Assistant",
                            message: CONNECTION_FAILED_MESSAGE,
                            timestamp: new Date().toLocaleTimeString(),
                        },
                    ]);
                    hubConnectionErrorMessageSent.current = true;
                }
                setConnectionError(true);
            }
        };

        establishConnection();
    }, [authClient]); // Added authClient to the dependency array

    useEffect(() => {
        chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    const sendMessage = async () => {
        if (inputMessage.trim() && connection) {
            const timestamp = new Date().toLocaleTimeString();
            setMessages((prevMessages) => [...prevMessages, { user: "User", message: inputMessage, timestamp }]);
            setInputMessage('');
            const request = {
                conversationId: conversationId,
                profileOptions: {
                    name: "HangmanPlayer", // default profile name
                },
                messages: [
                    {
                        role: "User",
                        content: inputMessage,
                    },
                ],
            };

            try {
                await connection.invoke('Send', request);
            } catch (error) {
                console.error('Error sending message: ', error);
                setMessages((prevMessages) => [
                    ...prevMessages,
                    {
                        user: "Assistant",
                        message: CONNECTION_LOST_MESSAGE,
                        timestamp: new Date().toLocaleTimeString(),
                    },
                ]);
            }
        }
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
                    onKeyDown={(e) => e.key === 'Enter' && sendMessage()}
                />
                <button onClick={sendMessage}>Send</button>
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
