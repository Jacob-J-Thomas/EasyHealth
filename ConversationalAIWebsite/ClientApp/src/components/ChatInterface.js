import React, { Component } from "react";
import { ReceivedMessage } from "./ReceivedMessage";
import { SentMessage } from "./SentMessage";
import { ChatInput } from "./ChatInput";
import "./ChatInterface.css";
import { HubConnectionBuilder } from '@microsoft/signalr';

export class ChatInterface extends Component {
    static displayName = ChatInterface.name;
    conversationId = this.generateGUID();

    constructor(props) {
        super(props);
        this.state = {
            messages: [
                { type: "received", author: "AI Assistant", message: "Hello and welcome! Please feel free to ask me anything!", timestamp: new Date().toLocaleTimeString() }
            ],
            hubConnection: null
        };
    }

    componentDidMount() {
        const hubConnection = new HubConnectionBuilder()
            .withUrl("https://localhost:7228/aiHub") // Replace with the actual URL of your SignalR hub
            .build();

    

        this.setState({ hubConnection }, () => {
            this.state.hubConnection
                .start()
                .then(() => {
                    console.log("Connection started");

                    // Listen for broadcastMessage from AIHub
                    this.state.hubConnection.on("broadcastMessage", (author, message) => {
                        // Handle received messages
                        console.log(`Received message from AIHub: ${author}: ${message}`);
                        this.handleReceivedMessage(author, message, new Date().toLocaleTimeString());
                    });
                })
                .catch(err => console.log("Error while establishing connection: " + err));
        });
    }

    addMessage = (type, message, timestamp) => {
        this.setState(prevState =>
        ({
            messages: [...prevState.messages, { type, message, timestamp }]
        }));
    }

    handleSentMessage = (message, timestamp) => {
        this.addMessage("sent", message, timestamp);
    }

    handleReceivedMessage = (author, message, timestamp) => {
        if (message !== null) {
            this.setState(prevState => {
                const messages = prevState.messages;
                const updatedMessages = [...messages]; // Create a copy of messages array
                const lastMessage = messages[messages.length - 1];
                
                if (lastMessage && lastMessage.type !== "sent") {
                    // Ensure lastMessage.message is initialized as a string
                    var updatedMessage = (lastMessage.message || "") + message;

                    if (author === "Generate_Single_Shot_Sample") {
                        updatedMessage = "Please wait a moment while I process your request...";
                    }
                    updatedMessages[messages.length - 1] = {
                        ...lastMessage,
                        author: author,
                        message: updatedMessage,
                        timestamp: timestamp
                    };

                    return { messages: updatedMessages };
                    
                } else {
                    // Add a new message to the state
                    return { messages: [...messages, { type: "received", author, message, timestamp }] };
                }

                
            });
        }
    }

    generateGUID() {
        return this.generateGuidSubstring() + this.generateGuidSubstring() + '-' + this.generateGuidSubstring() + '-' + this.generateGuidSubstring() + '-'
            + this.generateGuidSubstring() + '-' + this.generateGuidSubstring() + this.generateGuidSubstring() + this.generateGuidSubstring();
    }

    generateGuidSubstring() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }

    render() {
        return (
            <div className="chat-interface">
                <div className="chat-box px-4 py-5">
                    {this.state.messages.map((msg, index) => (
                        msg.type === "received" ?
                            <ReceivedMessage key={index} author={msg.author} message={msg.message} timestamp={msg.timestamp} /> :
                            <SentMessage key={index} author={msg.author} message={msg.message} timestamp={msg.timestamp} />
                    ))}
                </div>
                <ChatInput onSendMessage={this.handleSentMessage} hubConnection={this.state.hubConnection} conversationId={this.conversationId} />
            </div>
        );
    }
}