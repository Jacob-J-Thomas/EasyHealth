import React from 'react';
import FooterSection from './FooterSection';
import { NavMenu } from './NavMenu';
import GenerateApiKey from './GenerateApiKey';
import './ApiReference.css';

function ApiReference() {
    return (
        <div className="api-reference-container">
            <NavMenu />
            <div className="intro-bubble">
                <h1>API Reference</h1>
                <p>
                    Welcome to the API documentation. This API enables you to perform various operations. All endpoints require a valid API key.
                </p>
                <p>
                    Use the "Generate API Key" option below to obtain your API key. Note that generating a new key will invalidate any previously generated key, and each key is valid for a specified period.
                </p>
                <GenerateApiKey />
            </div>

            <div className="section-bubble">
                <h2>Usage</h2>
                <p>
                    The standard flow is simple: first, perform the necessary setup using the provided endpoint; then, make a request to execute the desired operation.
                </p>
            </div>

            <div className="section-bubble">
                <h2>Authentication</h2>
                <p>
                    Every API request must include your API key in the header using the key <code>x-api-key</code>. For example:
                </p>
                <pre>
                    <code>
                        {`x-api-key: your-api-key`}
                    </code>
                </pre>
                <p>
                    The backend middleware validates your API key and will return a 401 Unauthorized response if the key is missing, invalid, or expired.
                </p>
            </div>

            <div className="section-bubble">
                <h2>Endpoints</h2>

                <div className="api-endpoint">
                    <h3>1. Example Endpoint</h3>
                    <p><strong>Endpoint:</strong> <code>POST /api/example</code></p>
                    <p><strong>Description:</strong> This endpoint allows you to perform a specific operation. The request body should contain the necessary data in a specified format.</p>
                    <h4>Request:</h4>
                    <pre>
                        <code>
                            {`POST /api/example
x-api-key: your-api-key
Content-Type: application/json

[request body goes here]`}
                        </code>
                    </pre>
                    <h4>Response:</h4>
                    <pre>
                        <code>
                            {`204 No Content (on success)`}
                        </code>
                    </pre>
                </div>

                <div className="api-endpoint">
                    <h3>2. Another Example Endpoint</h3>
                    <p><strong>Endpoint:</strong> <code>GET /api/another-example</code></p>
                    <p><strong>Description:</strong> This endpoint allows you to retrieve specific data.</p>
                    <h4>Request:</h4>
                    <pre>
                        <code>
                            {`GET /api/another-example
x-api-key: your-api-key`}
                        </code>
                    </pre>
                    <h4>Response:</h4>
                    <pre>
                        <code>
                            {`[Response body goes here]`}
                        </code>
                    </pre>
                </div>
            </div>
            <FooterSection />
        </div>
    );
}

export default ApiReference;
