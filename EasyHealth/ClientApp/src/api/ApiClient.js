import axios from 'axios';

const DefaultErrorMessage = "Something went wrong. If you continue to receive this error, please request support at jacob.thomas@applied-ai-org.com.";

class ApiClient {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
        this.client = axios.create({
            baseURL: baseUrl,
        });
    }

    // Helper method to retry a request
    async retryRequest(fn, retries = 6) {
        for (let attempt = 0; attempt < retries; attempt++) {
            try {
                return await fn();
            } catch (error) {
                if (error.response && [404, 401].includes(error.response.status)) {
                    return; // Do not retry 404 or 401 errors
                }

                if (attempt < retries - 1) {
                    await new Promise(resolve => setTimeout(resolve, 100 * (attempt + 1))); // Exponential backoff
                } else {
                    return;
                }
            }
        }
    }

    async getStreamKey() {
        try {
            const response = await this.client.get(`/promptflow/get/newBearerKey`);
            if (response.status === 200) {
                return response.data;
            } else {
                return DefaultErrorMessage;
            }
        } catch (error) {
            return DefaultErrorMessage;
        }
    }
}

export default ApiClient;



