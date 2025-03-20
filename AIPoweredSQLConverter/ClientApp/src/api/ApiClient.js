import axios from 'axios';
import authConfig from '../auth_config.json';

const InnapropriateRequestErrorMessage = "Your last message was flagged as unrelated to SQL. Please check your input."
const DefaultErrorMessage = "Something went wrong. If you continue to receive this error, please request support at applied.ai.help@gmail.com."
const NullTableErrorMessage = "Please ensure you've attached sql table definition(s) to the left.";

class ApiClient {
    constructor(baseUrl, getAccessTokenSilently) {
        this.baseUrl = baseUrl;
        this.getAccessTokenSilently = getAccessTokenSilently;
        this.client = axios.create({
            baseURL: baseUrl,
        });

        // Add a request interceptor to automatically attach the access token.
        this.client.interceptors.request.use(
            async (config) => {
                try {
                    // Request a token with the proper audience and scope
                    const token = await this.getAccessTokenSilently({
                        authorizationParams: {
                            audience: authConfig.audience,
                            scope: authConfig.scope,
                        },
                    });
                    if (token) {
                        config.headers.Authorization = `Bearer ${token}`;
                    }
                } catch (error) {
                    console.error('Error fetching access token:', error);
                    return "Please ensure you've attached a Table Definition. If you continue to receive this error, please request support at applied.ai.help@gmail.com.";
                }
                return config;
            },
            (error) => Promise.reject(error)
        );
    }

    async getNewAPIKey(username) {
        try {
            const response = await this.client.get(`/api/get/newAPIKey/${username}`);
            if (response.status === 200) {
                return response.data;
            } else {
                console.error('Failed to get new API key:', response.statusText);
                return DefaultErrorMessage;
            }
        } catch (error) {
            console.error('Error during getting new API key:', error.message);
            return DefaultErrorMessage;
        }
    }

    async getSQLData(username) {
        try {
            const response = await this.client.get('/api/get/sqlData/' + username);
            if (response.status === 200) {
                return response.data;
            } else {
                console.error('Failed to save SQL data:', response.statusText);
                return DefaultErrorMessage;
            }
        } catch (error) {
            console.error('Error during saving SQL data:', error.message);
            return DefaultErrorMessage;
        }
    }

    async saveSQLData(username, sqlDefinitionsString) {
        try {
            const body = {
                Username: username,
                SqlData: sqlDefinitionsString,
            };

            const response = await this.client.post('/api/post/sqlData', body);
            if (response.status === 204) {
                return response.data;
            } else {
                console.error('Failed to save SQL data:', response.statusText);
                return DefaultErrorMessage;
            }
        } catch (error) {
            console.error('Error during saving SQL data:', error.message);
            return DefaultErrorMessage;
        }
    }

    async requestSQLDataHelp(username, assistanceQuery, tableDefinitions) {
        try {
            const body = {
                Username: username,
                Query: assistanceQuery,
                SqlData: tableDefinitions
            };

            const response = await this.client.post('/api/post/sqlHelp', body);
            if (response.status === 200) {
                return response.data;
            } else {
                console.error('Failed to request SQL data help:', response.statusText);
                return DefaultErrorMessage;
            }
        } catch (error) {
            console.error('Error during requesting SQL data help:', error.message);
            return DefaultErrorMessage;
        }
    }

    async requestSQLConversion(username, messages, tableDefinitions) {
        try {
            if (tableDefinitions === null || tableDefinitions.trim() === "") return NullTableErrorMessage;

            const body = {
                Username: username,
                Messages: messages,
                SqlData: tableDefinitions
            };

            const response = await this.client.post('/api/post/convertQuery', body);
            if (response.status === 200) {
                return response.data;
            } else {
                console.error('Failed to convert query to SQL:', response.statusText);
                return DefaultErrorMessage;
            }
        } catch (error) {
            console.error('Error during SQL conversion request:', error.message);
            return DefaultErrorMessage;
        }
    }
}

export default ApiClient;
