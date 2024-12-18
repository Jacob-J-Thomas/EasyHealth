import axios from 'axios';

class ApiClient {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
        this.client = axios.create({
            baseURL: baseUrl,
        });
    }

    // generate a new hangmanword
    async getHangmanWord() {
        try {
            const response = await this.client.get('/api/generateWord'); // Ensure '/start/hangman' matches your controller route
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

    // Method to call the StartHangman endpoint
    async startHangmanGame(word) {
        try {
            const response = await this.client.get(`/api/start/${word}`); // Ensure '/start/hangman' matches your controller route
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