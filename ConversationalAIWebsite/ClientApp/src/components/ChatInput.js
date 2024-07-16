import React, { Component } from 'react';
import './ChatInput.css';
import { HubConnectionBuilder } from '@microsoft/signalr';

export class ChatInput extends Component {
    static displayName = ChatInput.name;
    constructor(props) {
        super(props);
        this.state = {
            message: ''
        };
    }

    handleChange = (event) => {
        this.setState({ message: event.target.value });
    }

    handleSend = async (event) => {
        event.preventDefault();
        const { message } = this.state;
        if (message.trim()) {
            // Clear the input field
            this.setState({ message: '' });
            const { conversationId, hubConnection, onSendMessage } = this.props;

            const timestamp = new Date().toLocaleTimeString();

            // Send the message to the parent component
            this.props.onSendMessage(message, timestamp);

            // build request data:
            var request = {
                "ProfileName": "string",
                "ConversationId": conversationId,
                "Username": "AiWebsiteTemplate",
                "Message": message
            }

            // Send the message to the SignalR hub
            try {
                await this.props.hubConnection.invoke("SendMessageToAIAPI", request);
            } catch (error) {
                console.error('Error sending message:', error);
            }
        }
    }

    render() {
        return (
            <form action="#" className="bg-light" onSubmit={this.handleSend}>
                <input
                    type="text"
                    placeholder="Ask a question"
                    aria-describedby="button-addon2"
                    className="form-control rounded-0 border-0 py-4 bg-light"
                    value={this.state.message}
                    onChange={this.handleChange}
                    maxLength="280"
                />
            </form>
        );
    }
}