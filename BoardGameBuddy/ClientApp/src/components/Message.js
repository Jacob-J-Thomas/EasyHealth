import React, { Component } from 'react';
import './Message.css';
import ReactMarkdown from 'react-markdown';

export class Message extends Component {
    static displayName = Message.name;

    render() {
        const { author, message, timestamp, alignRight } = this.props; // Added alignRight prop
        const alignmentClass = alignRight ? 'align-right' : 'align-left'; // Determine alignment class

        // Define the background color based on alignment
        const messageStyle = {
            backgroundColor: alignRight ? '#f0f0f0' : '#007bff' 
        };

        return (
            <div className={`message-container ${alignmentClass}`}> {/* Apply alignment class */}
                <p className={`author ${alignmentClass}`}>{author}</p>
                <div className="message" style={messageStyle}> {/* Apply dynamic style */}
                    <ReactMarkdown className="message-body">{message}</ReactMarkdown>
                </div>
                <p className={`time-stamp ${alignmentClass}`}>{timestamp}</p>
            </div>
        );
    }
}
