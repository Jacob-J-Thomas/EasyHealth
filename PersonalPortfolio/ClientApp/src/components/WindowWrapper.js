import React, { useEffect, useState } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import ChatWindow from './ChatWindow';
import ContentWindow from './ContentWindow';
import FooterSection from './FooterSection';
import ApiClient from '../api/ApiClient';
import { NavMenu } from './NavMenu';
import './WindowWrapper.css';
import authConfig from '../auth_config.json';

const WindowWrapper = () => {
    const { isAuthenticated, getAccessTokenSilently, user } = useAuth0();
    const [state, setState] = useState({
        tableDefinitions: '',
        isSmallViewport: window.innerWidth < 1200,
        stage: 0,
        conversationId: '',
        gameStartMessage: '',
        accessToken: '',
        messages: [],
        connectionError: false,
    });

    // Initialize API client


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

    useEffect(() => {
        const apiClient = new ApiClient(authConfig.ApiUri, getAccessTokenSilently);
        const initialize = async () => {
            if (isAuthenticated) {
                try {
                    const token = await getAccessTokenSilently({
                        audience: authConfig.audience,
                    });
                    setState((prevState) => ({
                        ...prevState,
                        accessToken: token,
                    }));
                } catch (error) {
                    
                }

                try {
                    const sqlData = await apiClient.getProfile(user.sub);
                    setState((prevState) => ({
                        ...prevState,
                        tableDefinitions: sqlData,
                    }));
                } catch (error) {
                    
                }
            }
        };
        initialize();
    }, [isAuthenticated, getAccessTokenSilently, user.sub]);

    useEffect(() => {
        const apiClient = new ApiClient(authConfig.ApiUri, getAccessTokenSilently);
        const handleStripeCheckoutClick = (event) => {
            if (event.target && event.target.id === 'stripe-checkout-link') {
                event.preventDefault();
                apiClient.redirectToStripeCheckout(user.sub);
            }
        };

        document.addEventListener('click', handleStripeCheckoutClick);

        return () => {
            document.removeEventListener('click', handleStripeCheckoutClick);
        };
    }, [getAccessTokenSilently, user.sub]);

    const requestSQLConversion = async (inputMessage) => {
        if (!user || !user.sub) {
            return;
        }

        const apiClient = new ApiClient(authConfig.ApiUri, getAccessTokenSilently);

        const userId = user.sub;
        const newMessage = {
            role: 'User',
            content: inputMessage,
        };
        const updatedMessages = [...state.messages, newMessage];

        setState((prevState) => ({
            ...prevState,
            messages: updatedMessages,
        }));

        try {
            const response = await apiClient.requestSQLConversion(userId, updatedMessages, state.tableDefinitions);

            if (response) {
                const responseMessage = {
                    role: 'Assistant',
                    content: response,
                };
                setState((prevState) => ({
                    ...prevState,
                    messages: [...prevState.messages, responseMessage],
                }));
            }
        } catch (error) {
            
        }
    };

    // clearMessages resets the messages state and localStorage entry
    const clearMessages = () => {
        setState((prevState) => ({
            ...prevState,
            messages: []
        }));
        localStorage.removeItem('messages');
    };

    return (
        <div className="window-wrapper-container">
            <NavMenu />
            <div className="fixed-pane-container">
                <div className="pane">
                    <ContentWindow/>
                </div>
                <div className="pane">
                    <label className="chatwindow-label">Chat Window</label>
                    <ChatWindow
                        isSmallViewport={state.isSmallViewport}
                        messages={state.messages}
                        sendMessage={(inputMessage) => {
                            requestSQLConversion(inputMessage);
                        }}
                        clearMessages={clearMessages}
                    />
                </div>
            </div>
            <FooterSection />
        </div>
    );
};

export default WindowWrapper;
