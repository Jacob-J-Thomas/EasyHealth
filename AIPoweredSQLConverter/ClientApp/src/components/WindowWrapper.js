import React, { Component } from 'react';
import SplitPane from 'react-split-pane';
import { useAuth0 } from '@auth0/auth0-react';
import { debounce } from 'lodash';
import ChatWindow from './ChatWindow';
import ContentWindow from './ContentWindow';
import FooterSection from './FooterSection';
import ApiClient from '../api/ApiClient';
import { NavMenu } from './NavMenu'; // Imported NavMenu
import './WindowWrapper.css';

const domain = 'https://localhost:7228';
const apiClient = new ApiClient(domain);

const CONNECTION_LOST_MESSAGE = "Oops! It seems I lost my connection. Could you please repeat your last message? If this issue persists, please contact support at applied.ai.help@gmail.com.";
const CONNECTION_FAILED_MESSAGE = "It looks like I'm having trouble connecting to the server. Please ensure your internet connection is stable and try again, or contact support at applied.ai.help@gmail.com if this issue persists.";

const WindowWrapper = () => {
    const { isAuthenticated, getAccessTokenSilently } = useAuth0();
    const [state, setState] = React.useState({
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

    React.useEffect(() => {
        const initialize = async () => {
            if (isAuthenticated) {
                //const token = await getAccessTokenSilently();
                //setState((prevState) => ({
                //    ...prevState,
                //    accessToken: token,
                //}));
                //await startHangmanGame(token);
            }
        };
        initialize();
    }, [isAuthenticated, getAccessTokenSilently]);

    //const startHangmanGame = async (token) => {
    //    try {
    //        const hangmanWord = await apiClient.getHangmanWord();
    //        const gameData = await apiClient.startHangmanGame(hangmanWord, token);
    //        const conversationId = gameData.conversationId;
    //        const messageJsonData = gameData.message;

    //        const parsedMessageData = JSON.parse(messageJsonData);
    //        const gameStartMessage = parsedMessageData.message;
    //        const isFailedGuess = parsedMessageData.increment;

    //        setState((prevState) => ({
    //            ...prevState,
    //            conversationId,
    //            gameStartMessage,
    //            stage: isFailedGuess ? prevState.stage + 1 : prevState.stage,
    //        }));
    //    } catch (error) {
    //        console.error('Failed to start hangman game: ', error);
    //    }
    //};

    // Other methods and render function remain the same

    return (
        <div className="window-wrapper-container">
            <NavMenu></NavMenu>
            <SplitPane
                style={{ position: 'relative', width: '100%', height: '100%' }}
                split={state.split}
                minSize={state.minSize}
                maxSize={state.maxSize}
                size={state.size}
                primary="second"
                onDragFinished={debounce((newSize) => setState({ size: newSize }), 50)}
            >
                <div className="pane">
                    <ContentWindow stage={state.stage} />
                </div>
                <div className="pane">
                    <label className="chatwindow-label">SQL Query Converter</label>
                    <ChatWindow
                        toggleSplit={() => setState((prevState) => ({
                            split: prevState.split === 'vertical' ? 'horizontal' : 'vertical',
                            size: '50%',
                        }))}
                        isSmallViewport={state.isSmallViewport}
                        messages={state.messages}
                        sendMessage={(inputMessage) => {
                            setState((prevState) => ({
                                messages: [...prevState.messages, { user: 'User', message: inputMessage, timestamp: new Date().toLocaleTimeString() }],
                            }));
                            // sendMessageToBackend(inputMessage);
                        }}
                    />
                    <div ref={React.createRef()} />
                </div>
            </SplitPane>
            <FooterSection></FooterSection>
        </div>
    );
};

export default WindowWrapper;