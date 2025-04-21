import axios from 'axios';
import authConfig from '../auth_config.json';
import { loadStripe } from '@stripe/stripe-js';

// Initialize Stripe with your publishable key
const stripePromise = loadStripe(authConfig.stripePublishableKey);

const DefaultErrorMessage = "Something went wrong. If you continue to receive this error, please request support at applied.ai.help@gmail.com.";
const profileName = 'test';

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

                }
                return config;
            },
            (error) => Promise.reject(error)
        );

        // Bind methods
        this.redirectToStripeCheckout = this.redirectToStripeCheckout.bind(this);
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

    async saveUser(sub) {
        try {
            const response = await this.client.post(`/promptflow/post/SaveUser/${sub}`);
            if (response.status === 200) {
                return response.data;
            } else {
                return DefaultErrorMessage;
            }
        } catch (error) {
            return DefaultErrorMessage;
        }
    }

    async getStreamKey(username) {
        try {
            const response = await this.client.get(`/promptflow/get/newBearerKey/${username}`);
            if (response.status === 200) {
                return response.data;
            } else {
                return DefaultErrorMessage;
            }
        } catch (error) {
            return DefaultErrorMessage;
        }
    }

    async getNewAPIKey(username) {
        try {
            const response = await this.client.get(`/promptflow/get/newAPIKey/${username}`);
            if (response.status === 200) {
                return response.data;
            } else {
                return DefaultErrorMessage;
            }
        } catch (error) {
            return DefaultErrorMessage;
        }
    }

    async upsertProfileData(body) {
        try {
            const response = await this.client.post('/promptflow/post/profile/', body);
            if (response.status === 200) {
                return response.data;
            } else {
                return DefaultErrorMessage;
            }
        } catch (error) {
            if (error.status === 404) return;
            return DefaultErrorMessage;
        }
    }
    
    async getProfileData() {
        try {
            const response = await this.client.get('/promptflow/get/profile/' + profileName);
            if (response.status === 200) {
                return response.data;
            } else {
                return DefaultErrorMessage;
            }
        } catch (error) {
            if (error.status === 404) return;
            return DefaultErrorMessage;
        }
    }

    async getUserData(username) {
        try {
            const response = await this.client.get('/promptflow/get/UserData/' + username);
            if (response.status === 200) {
                return response.data;
            } else {
                return DefaultErrorMessage;
            }
        } catch (error) {
            if (error.status === 404) return;
            return DefaultErrorMessage;
        }
    }

    async saveUserData(username, sqlDefinitionsString) {
        try {
            const body = {
                Username: username,
                SqlData: sqlDefinitionsString,
            };

            const response = await this.client.post('/promptflow/post/sqlData', body);
            if (response.status === 204) {
                return response.data;
            } else {
                return DefaultErrorMessage;
            }
        } catch (error) {
            return DefaultErrorMessage;
        }
    }

    async createPortalSession(sub) {
        try {
            const response = await this.client.post(`/webhook/create-portal-session/${sub}`);
            if (response.status === 200) {
                if (response.data) return response.data;
            } else {
                return null;
            }
        } catch (error) {
            return null;
        }
    }

    async createCheckoutSession(sub) {
        try {
            const response = await this.client.post(`/webhook/create-checkout-session/${sub}`);
            if (response.status === 200) {
                return response.data.sessionId;
            } else {
                return null;
            }
        } catch (error) {
            return null;
        }
    }

    async redirectToStripeCheckout(sub, sessionId = null) {
        const stripe = await stripePromise;

        if (!sessionId) sessionId = await this.createCheckoutSession(sub);

        if (sessionId) {
            await stripe.redirectToCheckout({ sessionId });
        }
    }

    appendCheckoutLink(message) {
        return `${message} Please <a href="#" id="stripe-checkout-link">click here</a> to proceed to the Stripe Checkout.`;
    }
}

export default ApiClient;



