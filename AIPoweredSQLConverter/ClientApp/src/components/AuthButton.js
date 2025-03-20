// AuthButton.js
import React from 'react';
import { useAuth0 } from "@auth0/auth0-react";
import './AuthButton.css';

const AuthButton = () => {
    const { isAuthenticated, loginWithRedirect, logout } = useAuth0();

    return isAuthenticated ? (
        <button
            onClick={() =>
                logout({ logoutParams: { returnTo: window.location.origin } })
            }
            className="logout-button"
        >
            Log Out
        </button>
    ) : (
        <button
            onClick={() =>
                loginWithRedirect({
                    authorizationParams: {
                        audience: 'https://localhost:7228', // Your API identifier
                        scope: 'email user:all',                   // The scope your API requires
                    },
                })
            }
            className="logout-button"
        >
            Log In
        </button>
    );
};

export default AuthButton;
