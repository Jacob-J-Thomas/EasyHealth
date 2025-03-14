import React from 'react';
import { withAuthenticationRequired } from '@auth0/auth0-react';
import WindowWrapper from './WindowWrapper';

const ProtectedWindowWrapper = withAuthenticationRequired(WindowWrapper, {
    onRedirecting: () => <div>Loading...</div>,
});

export default ProtectedWindowWrapper;
