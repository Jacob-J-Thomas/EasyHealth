import React, { Component } from 'react';
import './Message.css';
import ReactMarkdown from 'react-markdown';
import rehypeRaw from 'rehype-raw'; // <-- Import rehype-raw to process raw HTML

export class Message extends Component {
    static displayName = Message.name;

    render() {
        const { author, content, timestamp, alignRight } = this.props;
        const alignmentClass = alignRight ? 'align-right' : 'align-left';
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
                <p className={`time-stamp ${alignmentClass}`}>{timestamp}</p>
            </div>
        );
    }
}
