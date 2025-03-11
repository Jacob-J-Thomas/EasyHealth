import React, { Component } from 'react';
import SplitPane from 'react-split-pane';
import { debounce } from 'lodash';
import * as signalR from '@microsoft/signalr';
import ChatWindow from './ChatWindow';
import ContentWindow from './ContentWindow';
import AuthClient from '../api/AuthClient';
import ApiClient from '../api/ApiClient';
import './WindowWrapper.css';

const authClient = new AuthClient("https://localhost:7228");
const apiClient = new ApiClient("https://localhost:7228");

const CHAT_API_URL = 'https://localhost:53337/chatstream';
const CONNECTION_LOST_MESSAGE = "Oops! It seems I lost my connection. Could you please repeat your last message? If this issue persists, please contact support at applied.ai.help@gmail.com.";
const CONNECTION_FAILED_MESSAGE = "It looks like I'm having trouble connecting to the server. Please ensure your internet connection is stable and try again, or contact support at applied.ai.help@gmail.com if this issue persists.";

let hangmanWord = '';

export default class WindowWrapper extends Component {
    constructor(props) {
        super(props);
        this.state = {
            size: '30%',  // Default size to 30%
            isSmallViewport: window.innerWidth < 1200, // Add state for isSmallViewport
            split: window.innerWidth < 1200 ? 'horizontal' : 'vertical',  // Default split to horizontal if isSmallViewport is true
            minSize: 0,
            maxSize: 0,
            stage: 0,
            conversationId: '',
            gameStartMessage: '',
            accessToken: '', // Add state for accessToken
            messages: [], // Add state for messages
            connectionError: false, // Add state for connection error
        };
        this.chatEndRef = React.createRef();
        this.updateSizes = this.updateSizes.bind(this);
        this.checkViewportSize = this.checkViewportSize.bind(this);
        this.toggleSplit = this.toggleSplit.bind(this);
        this.scrollToBottom = this.scrollToBottom.bind(this);
        this.handleResize = debounce(this.handleResize.bind(this), 50); // Adjust delay if needed
        this.setStage = this.setStage.bind(this);
        this.startHangmanGame = this.startHangmanGame.bind(this);
        this.establishConnection = this.establishConnection.bind(this);
        this.addMessageToUI = this.addMessageToUI.bind(this);
        this.sendMessageToBackend = this.sendMessageToBackend.bind(this);
        this.sendMessage = this.sendMessage.bind(this); // Bind sendMessage method
    }

    async componentDidMount() {
        this.updateSizes(); 
        this.checkViewportSize(); // Initial check
        window.addEventListener('resize', this.updateSizes);
        window.addEventListener('resize', this.checkViewportSize);
        await this.startHangmanGame(); // Start the hangman game
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.updateSizes);
        window.removeEventListener('resize', this.checkViewportSize);
        if (this.state.connection) {
            this.state.connection.stop();
        }
    }

    async startHangmanGame() {
        try {
            const tokenResponse = await authClient.authorize();
            const accessToken = tokenResponse.access_token;

            hangmanWord = await apiClient.getHangmanWord();
            const gameData = await apiClient.startHangmanGame(hangmanWord);
            const conversationId = gameData.conversationId;
            const messageJsonData = gameData.message;

            // Parse the messageJsonData JSON string
            const parsedMessageData = JSON.parse(messageJsonData);
            const gameStartMessage = parsedMessageData.message;
            const isFailedGuess = parsedMessageData.increment;

            this.setState((prevState) => ({
                conversationId,
                gameStartMessage,
                stage: isFailedGuess ? prevState.stage + 1 : prevState.stage, // Increment stage if isFailedGuess is true
                accessToken, // Set accessToken in state
            }), () => {
                this.addMessageToUI(gameStartMessage, "Assistant"); // Add the initial message without sending to backend
                this.establishConnection(); // Establish connection after setting state
            });
        } catch (error) {
            console.error('Failed to start hangman game: ', error);
        }
    }

    async establishConnection() {
        try {
            const { accessToken } = this.state;

            // Establish SignalR connection with the access token
            const newConnection = new signalR.HubConnectionBuilder()
                .withUrl(CHAT_API_URL, {
                    accessTokenFactory: () => accessToken, // Use the access token for authentication
                })
                .withAutomaticReconnect() // Automatically reconnect
                .build();

            let messageBuffer = '';

            newConnection.on('broadcastMessage', (response) => {
                const author = response.role;
                const messageChunk = response.completionUpdate;
                const timestamp = new Date().toLocaleTimeString();

                // Accumulate message chunks
                messageBuffer += messageChunk;

                // Check if the buffer contains a complete JSON object
                try {
                    const parsedMessageData = JSON.parse(messageBuffer);
                    const message = parsedMessageData.message;
                    const isFailedGuess = parsedMessageData.increment;

                    this.setState((prevState) => {
                        const updatedMessages = [...prevState.messages];
                        const lastMessage = prevState.messages[prevState.messages.length - 1];

                        // Ensure stage does not exceed 6
                        const newStage = isFailedGuess ? Math.min(prevState.stage + 1, 6) : prevState.stage;

                        if (lastMessage && lastMessage.user === author) {
                            const updatedMessage = (lastMessage.message || "") + message;
                            updatedMessages[prevState.messages.length - 1] = {
                                ...lastMessage,
                                author: author,
                                message: updatedMessage,
                                timestamp: timestamp,
                            };
                        } else if (newStage >= 6) {
                            updatedMessages.push({
                                user: author,
                                message: `Oh no, it looks like you ran out of guesses! The word was '${hangmanWord}'. Please refresh the page to play again.`, // This refresh could be triggered via a tool call
                                timestamp: timestamp,
                            });
                        } else if (message && message.trim() !== "") {
                            updatedMessages.push({
                                user: author,
                                message: message,
                                timestamp: timestamp,
                            });
                        }

                        return {
                            messages: updatedMessages,
                            stage: newStage,
                        }
                    });

                    console.log(`Received chunk from AIHub: ${author}: ${message}`);
                    messageBuffer = ''; // Clear the buffer after successful parsing
                } catch (e) {
                    // If JSON.parse fails, it means the buffer does not contain a complete JSON object yet
                    console.log('Waiting for more message chunks...');
                }
            });

            newConnection.onclose(() => {
                if (!this.hubConnectionErrorMessageSent) {
                    this.setState((prevState) => ({
                        messages: [
                            ...prevState.messages,
                            {
                                user: "Assistant",
                                message: CONNECTION_LOST_MESSAGE,
                                timestamp: new Date().toLocaleTimeString(),
                            },
                        ],
                    }));
                    this.hubConnectionErrorMessageSent = true;
                }
                this.setState({ connectionError: true });
            });

            await newConnection.start();
            console.log('Connected to SignalR hub');
            this.setState({ connection: newConnection, connectionError: false });
            this.hubConnectionErrorMessageSent = false; // Reset the flag when connection is successful
        } catch (error) {
            console.error('Connection failed: ', error);
            if (!this.hubConnectionErrorMessageSent) {
                this.setState((prevState) => ({
                    messages: [
                        ...prevState.messages,
                        {
                            user: "Assistant",
                            message: CONNECTION_FAILED_MESSAGE,
                            timestamp: new Date().toLocaleTimeString(),
                        },
                    ],
                }));
                this.hubConnectionErrorMessageSent = true;
            }
            this.setState({ connectionError: true });
        }
    }

    addMessageToUI(message, user) {
        const timestamp = new Date().toLocaleTimeString();
        const newMessage = { user, message, timestamp };

        this.setState((prevState) => ({
            messages: [...prevState.messages, newMessage],
        }));
    }

    async sendMessageToBackend(inputMessage) {
        const { connection, conversationId } = this.state;
        if (inputMessage.trim() && connection) {
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
                this.addMessageToUI(CONNECTION_LOST_MESSAGE, "Assistant");
            }
        }
    }

    sendMessage(inputMessage) {
        this.addMessageToUI(inputMessage, "User");
        this.sendMessageToBackend(inputMessage);
    }

    updateSizes() {
        const { split } = this.state; // Use the latest split value
        const viewportSize = split === 'vertical' ? window.innerWidth : window.innerHeight;

        this.setState({
            minSize: viewportSize * 0.25,
            maxSize: viewportSize * 0.75,
        });
    }

    checkViewportSize() {
        if (window.innerWidth < 1200) {
            this.setState((prevState) => ({
                split: 'horizontal',
                size: prevState.isSmallViewport ? prevState.size : '50%', // Only update size if isSmallViewport is false
                isSmallViewport: true
            }));
        } else {
            this.setState({
                isSmallViewport: false
            });
        }
    }

    toggleSplit() {
        this.setState(
            (prevState) => ({
                split: prevState.split === 'vertical' ? 'horizontal' : 'vertical',
                size: '30%', // Reset size when toggling
            }),
            this.updateSizes // Ensure sizes are recalculated with the new split state
        );
    }

    scrollToBottom() {
        this.chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }

    handleResize(newSize) {
        this.setState({ size: newSize });
    }

    setStage(stage) {
        this.setState({ stage });
    }

    render() {
        return (
            <SplitPane
                split={this.state.split}
                minSize={this.state.minSize}
                maxSize={this.state.maxSize}
                size={this.state.size}    // Use controlled size
                primary="second" // set the primary to the chat window to ensure proper resizing
                onDragFinished={this.handleResize}  // Use handleResize for debounced updates
            >
                <div className="pane">
                    <ContentWindow stage={this.state.stage} />
                </div>
                <div className="pane">
                    <ChatWindow
                        toggleSplit={this.toggleSplit}
                        isSmallViewport={this.state.isSmallViewport} // Pass isSmallViewport to ChatWindow
                        messages={this.state.messages} // Pass messages to ChatWindow
                        sendMessage={this.sendMessage} // Pass sendMessage to ChatWindow
                    />
                    <div ref={this.chatEndRef} />
                </div>
            </SplitPane>
        );
    }
}