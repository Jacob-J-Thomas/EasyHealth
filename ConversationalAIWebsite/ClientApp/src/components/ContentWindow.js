import React, { useEffect, useState } from 'react';
import ApiClient from '../api/ApiClient';
import './ContentWindow.css';
import { useAuth0 } from '@auth0/auth0-react';
import authConfig from '../auth_config.json';

function ContentWindow() {
    const [profileData, setProfileData] = useState(null);
    const [loading, setLoading] = useState(true);
    const { getAccessTokenSilently } = useAuth0();
    
    useEffect(() => {
        const apiClient = new ApiClient(authConfig.ApiUri, getAccessTokenSilently);
        async function fetchProfileData() {
            const data = await apiClient.getProfileData('test');
            if (data === "Something went wrong...") {
                const profile = {
                    Name: 'test',
                    Model: 'gpt-4o',
                    Host: 'Azure'
                };
                await apiClient.upsertProfileData(profile);
                setProfileData(profile);
            } else {
                setProfileData(data);
            }
            setLoading(false);
        }
        fetchProfileData();
    }, [getAccessTokenSilently]);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <div className="component-container">
            <label className="table-definitions-label">Content Tab</label>
            <textarea
                className="table-definitions-input"
                rows="10"
                cols="50"
                value={JSON.stringify(profileData, null, 2)}
                readOnly
            />
        </div>
    );
}

export default ContentWindow;
