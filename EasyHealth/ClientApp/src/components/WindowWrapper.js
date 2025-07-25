import React, { useEffect, useState } from 'react';
import ChatWindow from './ChatWindow';
import './WindowWrapper.css';

const WindowWrapper = () => {
    const [state, setState] = useState({
        tableDefinitions: '',
        isSmallViewport: window.innerWidth < 1200,
        stage: 0,
        conversationId: '',
        gameStartMessage: '',
        accessToken: '',
        messages: [
            {
                role: 'Assistant',
                content: "Hi there! I’m StackSherpa, your guide to Jacob Thomas’s technical skills, projects, and experience. How can I help you today? Looking to explore his AI work, discuss his experience in .NET and Python, or dive into one of his personal projects? ??",
                timestamp: new Date().toISOString(),
            },
        ],
        connectionError: false,
    });

    // Load messages from localStorage on mount
    useEffect(() => {
        const savedMessages = localStorage.getItem('messages');
        if (savedMessages) {
            setState((prevState) => ({
                ...prevState,
                messages: JSON.parse(savedMessages),
            }));
        }
    }, []);

    // Save messages to localStorage whenever they change
    useEffect(() => {
        localStorage.setItem('messages', JSON.stringify(state.messages));
    }, [state.messages]);

    const updateConversation = async (inputMessage) => {
    if (!inputMessage.trim()) {
        return;
    }

    const newMessage = {
        role: 'User',
        content: inputMessage,
    };
    const updatedMessages = [...state.messages, newMessage];

    setState((prevState) => ({
        ...prevState,
        messages: updatedMessages,
    }));
};


    const clearMessages = () => {
        setState((prevState) => ({
            ...prevState,
            messages: [],
        }));
        localStorage.removeItem('messages');
    };

    return (
        <div className="window-wrapper-container">
            <div className="fixed-pane-container">
                <div className="pane">
                    <ChatWindow
                        isSmallViewport={state.isSmallViewport}
                        messages={state.messages}
                        sendMessage={(inputMessage) => {
                            updateConversation(inputMessage);
                        }}
                        clearMessages={clearMessages}
                    />
                </div>
            </div>
        </div>
    );
};

export default WindowWrapper;
