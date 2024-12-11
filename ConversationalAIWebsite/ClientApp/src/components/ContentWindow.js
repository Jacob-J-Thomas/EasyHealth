import React from 'react';
import { ReactSVG } from 'react-svg';
import './ContentWindow.css';

function ContentWindow({ stage }) {
    const stages = [
        'stage0.svg',
        'stage1.svg',
        'stage2.svg',
        'stage3.svg',
        'stage4.svg',
        'stage5.svg',
        'stage6.svg',
    ];

    return (
        <div className="pane">
            <h1>Hangman Game</h1>
            <ReactSVG src={`/assets/hangman/stage6.svg`} />
        </div>
    );
}

export default ContentWindow;