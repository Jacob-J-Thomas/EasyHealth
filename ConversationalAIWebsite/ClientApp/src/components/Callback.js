import React, { useEffect } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { useNavigate } from 'react-router-dom';

const Callback = () => {
    const { handleRedirectCallback } = useAuth0();
    const navigate = useNavigate();

    useEffect(() => {
        const processCallback = async () => {
            await handleRedirectCallback();
            // Remove the query parameters so that reloading doesn't trigger another callback processing
            window.history.replaceState({}, document.title, window.location.pathname);
            navigate('/');
        };
        processCallback();
    }, [handleRedirectCallback, navigate]);

    return <div>Loading...</div>;
};

export default Callback;
