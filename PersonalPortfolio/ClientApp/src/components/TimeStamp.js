import React, { Component } from 'react';
import './TimeStamp.css';
export class TimeStamp extends Component {
  static displayName = TimeStamp.name;

  render() {
    return (
        <p className="time-stamp"></p>
    );
  }
}