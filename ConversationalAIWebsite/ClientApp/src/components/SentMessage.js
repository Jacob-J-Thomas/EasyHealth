import React, { Component } from 'react';
import './SentMessage.css';
export class SentMessage extends Component {
  static displayName = SentMessage.name;

  render() {
    const { message, timestamp } = this.props;
    return (
        <div className="sent-message-container">
            <div className="sent-message-body">
                <p className="text-small mb-0 text-white">{message}</p>
            </div>
            <p className="sent-time-stamp">{timestamp}</p>
        </div>
    );
  }
}