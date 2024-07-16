import React, { Component } from 'react';
import './ReceivedMessage.css';
import ReactMarkdown from 'react-markdown';
export class ReceivedMessage extends Component {
    static displayName = ReceivedMessage.name;

  render() {
    const { author, message, timestamp } = this.props;
      return (
        <div className="received-message-container">
            <p className="received-author">{author}</p>
            <div className="received-message">
            <ReactMarkdown className="received-message-body">{message}</ReactMarkdown>
            </div>
            <p className="received-time-stamp">{timestamp}</p>
        </div>
    );
  }
}