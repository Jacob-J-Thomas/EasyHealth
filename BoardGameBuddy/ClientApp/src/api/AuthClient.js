import axios from 'axios';

class AuthClient {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
        this.client = axios.create({
            baseURL: baseUrl,
        });

        this.authTokenResponse = null; // To store the token response
        this.tokenRefreshThreshold = 300; // Refresh the token 300 seconds before expiration
    }

    // Method to call the 'Authorize' endpoint
    async authorize() {
        try {
            const response = await this.client.get('/api/auth'); // Ensure '/auth' matches your controller route
            if (response.status === 200) {
                this.authTokenResponse = response.data; // Store the token response
                return this.authTokenResponse;
            } else {
                console.error('Failed to authorize:', response.statusText);
                throw new Error(response.statusText);
            }
        } catch (error) {
            console.error('Error during authorization:', error.message);
            throw new Error('Something went wrong when authenticating');
        }
    }

    // Method to get the access token
    getAccessToken() {
        if (!this.authTokenResponse) {
            throw new Error('No auth token available. Please authorize first.');
        }

        const { access_token, expires_in } = this.authTokenResponse;
        const expiryTime = Date.now() + expires_in * 1000; // Convert to milliseconds

        // Check if token is about to expire
        if (Date.now() > expiryTime - this.tokenRefreshThreshold * 1000) {
            console.log('Token is about to expire, refreshing...');
            return this.refreshToken(); // Call refreshToken if token is close to expiring
        }

        return access_token; // Return the existing access token
    }

    // Method to refresh the token
    async refreshToken() {
        try {
            const response = await this.authorize(); // Re-authorize to get a new token
            this.authTokenResponse = response; // Update stored token response
            return this.authTokenResponse.access_token; // Return the new access token
        } catch (error) {
            console.error('Error refreshing token:', error.message);
            throw new Error('Could not refresh token');
        }
    }
}

export default AuthClient;
