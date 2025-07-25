import React, { Component } from 'react';
import './Message.css';
import ReactMarkdown from 'react-markdown';
import rehypeRaw from 'rehype-raw';
import { FeedbackButtons } from './FeedbackButtons';

export class Message extends Component {
    static displayName = Message.name;

    handleFeedback = (type) => {
        if (this.props.onFeedback) {
            this.props.onFeedback(type, this.props.content);
        }
    };

    render() {
        const { author, content, alignRight } = this.props;
        const alignmentClass = alignRight ? 'align-left' : 'align-right';
        const messageStyle = {
            backgroundColor: alignRight ? '#ebebeb' : '#014575' 
        };

        return (
            <div className={`message-container ${alignmentClass}`}>
                <p className={`author ${alignmentClass}`}>{author}</p>
                <div className="message" style={messageStyle}>
                    <ReactMarkdown className="message-body" rehypePlugins={[rehypeRaw]}>
                        {content}
                    </ReactMarkdown>
                    {author === "StackSherpa" && (
                        <FeedbackButtons onFeedback={this.handleFeedback} />
                    )}
                </div>
            </div>
        );
    }
}
