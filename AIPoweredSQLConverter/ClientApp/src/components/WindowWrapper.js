import React, { Component } from 'react';
import SplitPane from 'react-split-pane';
import { debounce } from 'lodash';
import * as signalR from '@microsoft/signalr';
import ChatWindow from './ChatWindow';
import ContentWindow from './ContentWindow';
import AuthClient from '../api/AuthClient';
import ApiClient from '../api/ApiClient';
import { NavMenu } from './NavMenu'; // Imported NavMenu
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
            size: window.innerWidth < 1200 ? '25%' : '50%',
            isSmallViewport: window.innerWidth < 1200,
            split: window.innerWidth < 1200 ? 'horizontal' : 'vertical',
            minSize: 0,
            maxSize: 0,
            stage: 0,
            conversationId: '',
            gameStartMessage: '',
            accessToken: '',
            messages: [],
            connectionError: false,
        };
        this.chatEndRef = React.createRef();
        this.updateSizes = this.updateSizes.bind(this);
        this.checkViewportSize = this.checkViewportSize.bind(this);
        this.toggleSplit = this.toggleSplit.bind(this);
        //this.scrollToBottom = this.scrollToBottom.bind(this);
        this.handleResize = debounce(this.handleResize.bind(this), 50);
        this.setStage = this.setStage.bind(this);
        this.startHangmanGame = this.startHangmanGame.bind(this);
        this.establishConnection = this.establishConnection.bind(this);
        this.addMessageToUI = this.addMessageToUI.bind(this);
        this.sendMessageToBackend = this.sendMessageToBackend.bind(this);
        this.sendMessage = this.sendMessage.bind(this);
    }

    async componentDidMount() {
        this.updateSizes();
        this.checkViewportSize();
        window.addEventListener('resize', this.updateSizes);
        window.addEventListener('resize', this.checkViewportSize);
        await this.startHangmanGame();
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

            const parsedMessageData = JSON.parse(messageJsonData);
            const gameStartMessage = parsedMessageData.message;
            const isFailedGuess = parsedMessageData.increment;

            this.setState((prevState) => ({
                conversationId,
                gameStartMessage,
                stage: isFailedGuess ? prevState.stage + 1 : prevState.stage,
                accessToken,
            }), () => {
                this.addMessageToUI(gameStartMessage, "Assistant");
                this.establishConnection();
            });
        } catch (error) {
            console.error('Failed to start hangman game: ', error);
        }
    }

    async establishConnection() {
        try {
            const { accessToken } = this.state;

            const newConnection = new signalR.HubConnectionBuilder()
                .withUrl(CHAT_API_URL, {
                    accessTokenFactory: () => accessToken,
                })
                .withAutomaticReconnect()
                .build();

            let messageBuffer = '';

            newConnection.on('broadcastMessage', (response) => {
                const author = response.role;
                const messageChunk = response.completionUpdate;
                const timestamp = new Date().toLocaleTimeString();

                messageBuffer += messageChunk;

                try {
                    const parsedMessageData = JSON.parse(messageBuffer);
                    const message = parsedMessageData.message;
                    const isFailedGuess = parsedMessageData.increment;

                    this.setState((prevState) => {
                        const updatedMessages = [...prevState.messages];
                        const lastMessage = prevState.messages[prevState.messages.length - 1];
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
                                message: `Oh no, it looks like you ran out of guesses! The word was '${hangmanWord}'. Please refresh the page to play again.`,
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
                    messageBuffer = '';
                } catch (e) {
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
            this.hubConnectionErrorMessageSent = false;
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
                    name: "HangmanPlayer",
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
        const { split } = this.state;
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
                size: prevState.isSmallViewport ? prevState.size : '50%',
                isSmallViewport: true
            }));
        } else {
            this.setState((prevState) => ({
                split: 'vertical',
                size: prevState.isSmallViewport ? '50%' : prevState.size,
                isSmallViewport: false
            }));
        }
    }

    toggleSplit() {
        this.setState(
            (prevState) => ({
                split: prevState.split === 'vertical' ? 'horizontal' : 'vertical',
                size: '30%',
            }),
            this.updateSizes
        );
    }

    //scrollToBottom() {
    //    this.chatEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    //}

    handleResize(newSize) {
        this.setState({ size: newSize });
    }

    setStage(stage) {
        this.setState({ stage });
    }

    render() {
        return (
            <div className="window-wrapper-container">
                <NavMenu></NavMenu>
                <SplitPane
                    style={{ position: 'relative', width: '100%', height: '100%' }}
                    split={this.state.split}
                    minSize={this.state.minSize}
                    maxSize={this.state.maxSize}
                    size={this.state.size}
                    primary="second"
                    onDragFinished={this.handleResize}
                >
                    <div className="pane">
                        <ContentWindow stage={this.state.stage} />
                    </div>
                    <div className="pane">
                        <label className="chatwindow-label">SQL Query Converter</label>
                        <ChatWindow
                            toggleSplit={this.toggleSplit}
                            isSmallViewport={this.state.isSmallViewport}
                            messages={this.state.messages}
                            sendMessage={this.sendMessage}
                        />
                        <div ref={this.chatEndRef} />
                    </div>
                </SplitPane>
                <div className="footer">
                    <p>&copy; 2025 Applied AI Inc. All rights reserved.</p>
                </div>
            </div>
        );
    }
}
