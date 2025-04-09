import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import './index.css';
import App from './App';
import * as serviceWorkerRegistration from './serviceWorkerRegistration';
import reportWebVitals from './reportWebVitals';
import { Auth0Provider } from '@auth0/auth0-react';
import authConfig from './auth_config.json';

const onRedirectCallback = (appState) => {
    window.history.replaceState(
        {},
        document.title,
        appState?.returnTo || window.location.pathname
    );
    // Clear messages from local storage on sign out
    if (!appState || appState.returnTo === '/logout') {
        localStorage.removeItem('messages');
    }
};

const rootElement = document.getElementById('root');
const root = createRoot(rootElement);

root.render(
    <Auth0Provider
        domain={authConfig.domain}
        clientId={authConfig.clientId}
        authorizationParams={{
            redirect_uri: authConfig.redirectUri,
            audience: authConfig.audience, // Use a consistent audience here
            scope: authConfig.scope,
        }}
        onRedirectCallback={onRedirectCallback}
    >
        <BrowserRouter>
            <App />
        </BrowserRouter>
    </Auth0Provider>
);

serviceWorkerRegistration.unregister();

reportWebVitals(metric => {
    console.log("App Insight Metric Written:\n", { name: metric.name, average: metric.value });
});
