import React, { Component } from 'react';
import './Message.css';
import ReactMarkdown from 'react-markdown';
import rehypeRaw from 'rehype-raw';

export class Message extends Component {
    static displayName = Message.name;

    render() {
        const { author, content, alignRight } = this.props;
        const alignmentClass = alignRight ? 'align-left' : 'align-right';
        const messageStyle = {
            backgroundColor: alignRight ? '#ebebeb' : '#007bff' 
        };

        return (
            <div className={`message-container ${alignmentClass}`}>
                <p className={`author ${alignmentClass}`}>{author}</p>
                <div className="message" style={messageStyle}>
                    <ReactMarkdown className="message-body" rehypePlugins={[rehypeRaw]}>
                        {content}
                    </ReactMarkdown>
                </div>
            </div>
        );
    }
}
