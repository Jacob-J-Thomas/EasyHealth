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
                    Welcome to the NLSequel API documentation. This API enables you to define, save, and retrieve your SQL schema, as well as convert natural language queries into SQL statements. All endpoints require a valid API key.
                </p>
                <p>
                    Use the "Generate API Key" option below to obtain your API key. Note that generating a new key will invalidate any previously generated key, and each key is valid for 6 months.
                </p>
                <GenerateApiKey />
            </div>

            <div className="section-bubble">
                <h2>Usage</h2>
                <p>
                    The standard flow is simple: first, update your SQL data by saving or updating your schema using the provided endpoint; then, make a conversion request to transform your natural language query into a SQL statement.
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
                    <h3>1. Save SQL Data</h3>
                    <p><strong>Endpoint:</strong> <code>POST /api/post/sqlData</code></p>
                    <p><strong>Description:</strong> Save or update your SQL schema and data. The schema is defined by a SQL script that creates two tables: one for user details (authentication, sign-in, and metadata) and one for sales (purchase details and metadata).</p>
                    <p>NOTE: SQL schemas work the best with the API, but technically any data format is acceptable, as long as it accuratley describes the SQL table in a clear and easy to understand way.</p>
                    <h4>Request:</h4>
                    <pre>
                        <code>
                            {`POST /api/post/sqlData
x-api-key: your-api-key
Content-Type: application/json

{
    "SqlData": "CREATE TABLE Users (
                Id INT PRIMARY KEY,
                Username VARCHAR(50),
                PasswordHash VARCHAR(255),
                LastSignIn DATETIME,
                Metadata JSON
            );
            
            CREATE TABLE Sales (
                SaleId INT PRIMARY KEY,
                UserId INT,
                Amount DECIMAL(10,2),
                SaleDate DATETIME,
                Metadata JSON,
                FOREIGN KEY (UserId) 
                REFERENCES Users(Id)
            );"
}`}
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
                    <h3>2. Retrieve SQL Data</h3>
                    <p><strong>Endpoint:</strong> <code>GET /api/get/sqlData</code></p>
                    <p><strong>Description:</strong> Retrieve your saved SQL schema and data.</p>
                    <h4>Request:</h4>
                    <pre>
                        <code>
                            {`GET /api/get/sqlData
x-api-key: your-api-key`}
                        </code>
                    </pre>
                    <h4>Response:</h4>
                    <pre>
                        <code>
                            {`{
  "Success": true,
  "Data": "CREATE TABLE Users (
                Id INT PRIMARY KEY,
                Username VARCHAR(50),
                PasswordHash VARCHAR(255),
                LastSignIn DATETIME,
                Metadata JSON
            );
            
            CREATE TABLE Sales (
                SaleId INT PRIMARY KEY,
                UserId INT,
                Amount DECIMAL(10,2),
                SaleDate DATETIME,
                Metadata JSON,
                FOREIGN KEY (UserId) 
                REFERENCES Users(Id)
            );"
}`}
                        </code>
                    </pre>
                </div>

                <div className="api-endpoint">
                    <h3>3. Convert Natural Language to SQL</h3>
                    <p><strong>Endpoint:</strong> <code>POST /api/post/convertQuery</code></p>
                    <p>
                        <strong>Description:</strong> Convert a natural language query into a SQL query. This example converts a query into a JOIN statement that retrieves user and sales details for sales over $100.
                    </p>
                    <p>
                        NOTE: The SQL data parameter can also be attached here, in which case this data will be used to inform the conversion rather than the existing data in the database.
                    </p>
                    <h4>Request:</h4>
                    <pre>
                        <code>
                            {`POST /api/post/convertQuery
x-api-key: your-api-key
Content-Type: application/json

{
  "Query": "Retrieve the username, last sign-in, sale amount, and sale date for users who made purchases over $100"
}`}
                        </code>
                    </pre>
                    <h4>Response:</h4>
                    <pre>
                        <code>
                            {`{
  "Success": true,
  "Data": "SELECT U.Username, U.LastSignIn, S.Amount, S.SaleDate FROM Users U JOIN Sales S ON U.Id = S.UserId WHERE S.Amount > 100;",
  "RemainingFreeRequests": 4
}`}
                        </code>
                    </pre>
                    <p>
                        If you exceed your daily request quota, you will receive a '429 Too Many Requests' response.
                    </p>
                </div>
            </div>
            <FooterSection />
        </div>
    );
}

export default ApiReference;

