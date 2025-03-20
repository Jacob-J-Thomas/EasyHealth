import React, { useEffect, useState } from 'react';
import SplitPane from 'react-split-pane';
import { useAuth0 } from '@auth0/auth0-react';
import { debounce } from 'lodash';
import ChatWindow from './ChatWindow';
import ContentWindow from './ContentWindow';
import FooterSection from './FooterSection';
import ApiClient from '../api/ApiClient';
import { NavMenu } from './NavMenu';
import './WindowWrapper.css';
import authConfig from '../auth_config.json';

const InnapropriateRequestErrorMessage = "Your last message was flagged as unrelated to SQL. Please check your input.";

const WindowWrapper = () => {
    const { isAuthenticated, getAccessTokenSilently, user } = useAuth0();
    const [state, setState] = useState({
        tableDefinitions: '',
        size: window.innerWidth < 1200 ? '50%' : '50%',
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
    });

    // Initialize API client
    const apiClient = new ApiClient(authConfig.ApiUri, getAccessTokenSilently);

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
                    console.error("Error fetching access token");
                }

                try {
                    const sqlData = await apiClient.getSQLData(user.email);
                    setState((prevState) => ({
                        ...prevState,
                        tableDefinitions: sqlData,
                    }));
                } catch (error) {
                    console.error('Error fetching SQL data:', error.message);
                }
            }
        };
        initialize();
    }, [isAuthenticated, getAccessTokenSilently]);

    const requestSQLConversion = async (inputMessage) => {
        if (!user || !user.email) {
            console.error('User is not authenticated or username is missing');
            return;
        }

        const username = user.email;
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
            const response = await apiClient.requestSQLConversion(username, updatedMessages, state.tableDefinitions);
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
            console.error('Error during SQL conversion request:', error.message);
        }
    };

    const handleAssistantSend = async (assistantInput) => {
        if (!user || !user.email) {
            console.error('User is not authenticated or email is missing');
            return;
        }

        const username = user.email;

        try {
            const response = await apiClient.requestSQLDataHelp(username, assistantInput, state.tableDefinitions);
            if (response.inappropriate) {
                const errorMessage = {
                    role: 'Assistant',
                    content: InnapropriateRequestErrorMessage
                };
                setState((prevState) => ({
                    ...prevState,
                    messages: [...prevState.messages, errorMessage],
                }));
            } else {
                setState((prevState) => ({
                    ...prevState,
                    tableDefinitions: response.response,
                }));
            }
        } catch (error) {
            console.error('Error during SQL data help request:', error.message);
        }
    };

    const handleSave = async (tableDefinitions) => {
        if (!user || !user.email) {
            console.error('User is not authenticated or email is missing');
            return;
        }

        const username = user.email;

        try {
            await apiClient.saveSQLData(username, tableDefinitions);
        } catch (error) {
            console.error('Error saving SQL data:', error.message);
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
            <SplitPane
                style={{ position: 'relative', width: '100%', height: '100%' }}
                split={state.split}
                minSize={state.minSize}
                maxSize={state.maxSize}
                size={state.size}
                primary="second"
                onDragFinished={debounce(
                    (newSize) =>
                        setState((prevState) => ({
                            ...prevState,
                            size: newSize,
                        })),
                    50
                )}
            >
                <div className="pane">
                    <ContentWindow
                        tableDefinitions={state.tableDefinitions}
                        onAssistantSend={handleAssistantSend}
                        onSave={handleSave}
                    />
                </div>
                <div className="pane">
                    <label className="chatwindow-label">SQL Query Converter</label>
                    <ChatWindow
                        toggleSplit={() =>
                            setState((prevState) => ({
                                ...prevState,
                                split: prevState.split === 'vertical' ? 'horizontal' : 'vertical',
                                size: '50%',
                            }))
                        }
                        isSmallViewport={state.isSmallViewport}
                        messages={state.messages}
                        sendMessage={(inputMessage) => {
                            requestSQLConversion(inputMessage);
                        }}
                        clearMessages={clearMessages}
                    />
                    <div ref={React.createRef()} />
                </div>
            </SplitPane>
            <FooterSection />
        </div>
    );
};

export default WindowWrapper;
