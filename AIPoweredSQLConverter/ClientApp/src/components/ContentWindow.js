import React, { useState } from 'react';
import './ContentWindow.css';

function ContentWindow({ stage, onTableDefinitionsChange }) {
    const [tableDefinitions, setTableDefinitions] = useState('');

    const handleInputChange = (event) => {
        const value = event.target.value;
        setTableDefinitions(value);
    };

    const handleSave = () => {
        onTableDefinitionsChange(tableDefinitions);
    };

    return (
        <div className="pane">
            <h1>Table Definitions</h1>
            <p>Enter your table definitions below:</p>
            <textarea
                className="table-definitions-input"
                value={tableDefinitions}
                onChange={handleInputChange}
                rows="10"
                cols="50"
            />
            <button className="save-button" onClick={handleSave}>Save</button>
        </div>
    );
}

export default ContentWindow;