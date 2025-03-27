import React from 'react';
import FooterSection from './FooterSection';
import { NavMenu } from './NavMenu';
import GenerateApiKey from './GenerateApiKey';
import './ApiReference.css';

function ApiReference() {
    return (
        <div className="api-reference-container">
            <NavMenu />
            <div className="api-reference-content">
                <h1>API Reference</h1>
                <p>Welcome to the API reference for NLSequel. Below you will find details about the available endpoints and how to use them.</p>
                <p>Below you can generate an API key to access these endpoints. Keys remain valid for 6 months following their creation.
                    NOTE: Generating this key will invalidate any previous key you've created.</p>
                <GenerateApiKey />
                <div className="api-endpoint">
                    <h2>Convert SQL</h2>
                    <p>Endpoint: <code>POST /api/convert</code></p>
                    <h3>Request</h3>
                    <pre>
                        <code>
                            {`{
                                "query": "SELECT * FROM users WHERE age > 30"
                            }`}
                        </code>
                    </pre>
                    <h3>Response</h3>
                    <pre>
                        <code>
                            {`{
                                "sql": "SELECT * FROM users WHERE age > 30"
                            }`}
                        </code>
                    </pre>
                </div>

                <div className="api-endpoint">
                    <h2>API Reference</h2>
                    <p>Endpoint: <code>GET /api/reference</code></p>
                    <h3>Request</h3>
                    <pre>
                        <code>
                            {`{
                                "apiKey": "your-api-key"
                            }`}
                        </code>
                    </pre>
                    <h3>Response</h3>
                    <pre>
                        <code>
                            {`{
                                "endpoints": [
                                    {
                                        "name": "Convert SQL",
                                        "method": "POST",
                                        "path": "/api/convert"
                                    },
                                    {
                                        "name": "API Reference",
                                        "method": "GET",
                                        "path": "/api/reference"
                                    }
                                ]
                            }`}
                        </code>
                    </pre>
                </div>
            </div>
            <FooterSection />
        </div>
    );
}

export default ApiReference;
