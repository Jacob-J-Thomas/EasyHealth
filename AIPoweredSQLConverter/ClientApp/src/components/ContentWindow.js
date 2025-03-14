import React, { useState } from 'react';
import './ContentWindow.css';

function ContentWindow({ stage, onTableDefinitionsChange }) {
    const [tableDefinitions, setTableDefinitions] = useState('');
    const [assistantInput, setAssistantInput] = useState('');

    const handleInputChange = (event) => {
        const value = event.target.value;
        setTableDefinitions(value);
    };

    const handleAssistantInputChange = (event) => {
        const value = event.target.value;
        setAssistantInput(value);
    };

    const handleSave = () => {
        onTableDefinitionsChange(tableDefinitions);
    };

    const handleSend = () => {

    };

    return (
        <div className="component-container">
            <div className="pane">
                <label className="table-definitions-label">SQL Table Definition(s)</label>
                <textarea
                    className="table-definitions-input"
                    value={tableDefinitions}
                    onChange={handleInputChange}
                    rows="10"
                    cols="50"
                />
                <div className="table-definitions-toolbar">
                    <div className="assistant-container">
                        <label className="assistant-label">
                            SQL Table Construction Assistant
                        </label>
                        <input
                            className="assistant-input"
                            type="text"
                            placeholder="Describe your table(s) here to generate a template, or get help modifying an existing one..."
                            value={assistantInput}
                            onChange={handleAssistantInputChange}
                        />
                    </div>
                    <button className="toolbar-button" /*onClick={handleSend}*/>Send</button>
                    <button className="toolbar-button" /*onClick={handleSend}*/>Save</button>
                </div>
            </div>
        </div>
    );
}

export default ContentWindow;
