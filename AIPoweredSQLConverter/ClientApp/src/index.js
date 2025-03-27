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
            redirect_uri: authConfig.redirectUri, // Ensure this matches auth_config.json
            audience: authConfig.audience,
            scope: authConfig.scope,
        }}
        onRedirectCallback={onRedirectCallback}
    >
        <BrowserRouter>
            <App />
        </BrowserRouter>
    </Auth0Provider>
);

// Initialize Application Insights
//function initializeAppInsights() {
//    const appInsights = new ApplicationInsights({
//        config: {
//            instrumentationKey: 'YOUR_INSTRUMENTATION_KEY' // Replace with your actual instrumentation key
//        }
//    });
//    appInsights.loadAppInsights();
//    return appInsights;
//}

//const appInsights = initializeAppInsights();

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
serviceWorkerRegistration.unregister();

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals(metric => {
    //appInsights.trackMetric({ name: metric.name, average: metric.value });
    console.log("App Insight Metric Written:\n", { name: metric.name, average: metric.value });
});

