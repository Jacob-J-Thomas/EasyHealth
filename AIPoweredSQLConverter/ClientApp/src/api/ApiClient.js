import axios from 'axios';

class ApiClient {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
        this.client = axios.create({
            baseURL: baseUrl,
        });
    }

    async getNewAPIKey(username) {
        try {
            const response = await this.client.get('/api/get/newAPIKey/' + username);
            if (response.status === 200) {
                return response.data;
            } else {
                console.error('Failed to start Hangman game:', response.statusText);
                throw new Error(response.statusText);
            }
        } catch (error) {
            console.error('Error during Hangman game start:', error.message);
            throw new Error('Something went wrong when starting the Hangman game');
        }
    }

    async saveSQLData(username, sqlDefinitionsString) {
        try {
            var body = {
                User: username,
                SqlData: sqlDefinitionsString
            }

            const response = await this.client.post('/api/post/sqlData', body);
            if (response.status === 200) {
                return response.data;
            } else {
                console.error('Failed to start Hangman game:', response.statusText);
                throw new Error(response.statusText);
            }
        } catch (error) {
            console.error('Error during Hangman game start:', error.message);
            throw new Error('Something went wrong when starting the Hangman game');
        }
    }

    async requestSQLDataHelp(username, assistanceQuery, currentSQLDefinitionsString) {
        try {
            var body = {
                User: username,
                Query: assistanceQuery,
                SqlData: currentSQLDefinitionsString
            }

            const response = await this.client.post('/api/post/sqlHelp', body); 
            if (response.status === 200) {
                return response.data;
            } else {
                console.error('Failed to start Hangman game:', response.statusText);
                throw new Error(response.statusText);
            }
        } catch (error) {
            console.error('Error during Hangman game start:', error.message);
            throw new Error('Something went wrong when starting the Hangman game');
        }
    }

    async requestSQLConversion(username, conversionString, conversationId) {
        try {
            var body = {
                User: username,
                Query: conversionString,
                ConversationId: conversationId
            }

            const response = await this.client.post('/api/post/convertQuery', body);
            if (response.status === 200) {
                return response.data;
            } else {
                console.error('Failed to start Hangman game:', response.statusText);
                throw new Error(response.statusText);
            }
        } catch (error) {
            console.error('Error during Hangman game start:', error.message);
            throw new Error('Something went wrong when starting the Hangman game');
        }
    }
}

export default ApiClient;