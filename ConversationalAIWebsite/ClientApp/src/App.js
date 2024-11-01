import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import SplitPane from 'react-split-pane';
import { debounce } from 'lodash';
import ChatWindow from './components/ChatWindow';
import ContentWindow from './components/ContentWindow';
import './App.css';

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
        this.state = {
            size: '30%',  // Default size to 30%
            isSmallViewport: window.innerWidth < 1200, // Add state for isSmallViewport
            split: window.innerWidth < 1200 ? 'horizontal' : 'vertical',  // Default split to horizontal if isSmallViewport is true
            minSize: 0,
            maxSize: 0
        };
        this.chatEndRef = React.createRef();
        this.updateSizes = this.updateSizes.bind(this);
        this.checkViewportSize = this.checkViewportSize.bind(this);
        this.toggleSplit = this.toggleSplit.bind(this);
        this.scrollToBottom = this.scrollToBottom.bind(this);
        this.handleResize = debounce(this.handleResize.bind(this), 50); // Adjust delay if needed
    }

    componentDidMount() {
        this.updateSizes();
        this.checkViewportSize(); // Initial check
        window.addEventListener('resize', this.updateSizes);
        window.addEventListener('resize', this.checkViewportSize);
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.updateSizes);
        window.removeEventListener('resize', this.checkViewportSize);
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

    render() {
        return (
            <Layout>
                <SplitPane
                    split={this.state.split}
                    minSize={this.state.minSize}
                    maxSize={this.state.maxSize}
                    size={this.state.size}    // Use controlled size
                    primary="second" // set the primary to the chat window to ensure proper resizing
                    onDragFinished={this.handleResize}  // Use handleResize for debounced updates
                >
                    <div className="pane">
                        <ContentWindow />
                    </div>
                    <div className="pane">
                        <ChatWindow
                            chatEndRef={this.chatEndRef}
                            toggleSplit={this.toggleSplit}
                            isSmallViewport={this.state.isSmallViewport} // Pass isSmallViewport to ChatWindow
                        />
                        <div ref={this.chatEndRef} />
                    </div>
                </SplitPane>
                <Routes>
                    {AppRoutes.map((route, index) => {
                        const { element, ...rest } = route;
                        return <Route key={index} {...rest} element={element} />;
                    })}
                </Routes>
            </Layout>
        );
    }
}
