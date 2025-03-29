import React, { useEffect } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import ApiClient from '../api/ApiClient';
import authConfig from '../auth_config.json';

const StripePortalRedirect = () => {
    const { user, loginWithRedirect, getAccessTokenSilently } = useAuth0();

    useEffect(() => {
        const apiClient = new ApiClient(authConfig.ApiUri, getAccessTokenSilently);

        async function createPortalSession() {
            if (!user || !user.sub) {
                loginWithRedirect({
                    authorizationParams: {
                        audience: authConfig.ApiUri,
                        scope: authConfig.scope,
                    },
                });
                return;
            }

            try {
                const data = await apiClient.createPortalSession(user.sub);
                if (data) {
                    if (data.url) {
                        window.location.replace(data.url); // Redirect to portal
                    } else if (data.sessionId) {
                        await apiClient.redirectToStripeCheckout(user.sub, data.sessionId); // Redirect to checkout
                    } else {
                        console.error("Invalid response from createPortalSession:", data);
                    }
                } else {
                    console.error("Failed to create portal session: No data returned.");
                }
            } catch (error) {
                console.error('Error creating portal session:', error);
            }
        }

        createPortalSession();
    }, [user, getAccessTokenSilently, loginWithRedirect]);

    return <div>Redirecting to your subscription management page...</div>;
};

export default StripePortalRedirect;