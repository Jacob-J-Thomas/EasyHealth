import React from 'react';
import FooterSection from './FooterSection';
import { NavMenu } from './NavMenu';
import { useAuth0 } from "@auth0/auth0-react";
import authConfig from '../auth_config.json';
import './HomePage.css';

function HomePage() {
    const { loginWithRedirect } = useAuth0();

    const handleSignUp = () => {
        loginWithRedirect({
            authorizationParams: {
                audience: authConfig.ApiUri,
                scope: authConfig.scope,
            },
        });
    };

    return (
        <div className="home-page-container">
            <NavMenu />
            <header className="hero-section">
                <div className="hero-content">
                    <h1>Empower Your Data with NLSequel</h1>
                    <p>
                        Transform natural language into secure SQL queries in seconds. Enjoy a free-to-use API
                        that never directly connects to your database—ensuring safety, transparency, and peace of mind.
                    </p>
                    <button className="signup-button" onClick={handleSignUp}>
                        Sign Up Now
                    </button>
                </div>
            </header>

            <section className="info-section">
                <div className="pricing-table">
                    <h2>Pricing</h2>
                    <table>
                        <thead>
                            <tr>
                                <th>Plan</th>
                                <th>Free Requests</th>
                                <th>API Access</th>
                                <th>Cost per Request</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>Free</td>
                                <td>20/day</td>
                                <td>✔️</td>
                                <td>$0.00</td>
                            </tr>
                            <tr>
                                <td>Pay-as-You-Go</td>
                                <td>20 free/day + Unlimited extra</td>
                                <td>✔️</td>
                                <td>$0.02</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div className="features">
                    <h2>Why NLSequel?</h2>
                    <ul>
                        <li>
                            <strong>Secure:</strong> No direct database connections mean your data is safe.
                        </li>
                        <li>
                            <strong>Efficient:</strong> Get SQL queries from natural language in seconds.
                        </li>
                        <li>
                            <strong>Flexible:</strong> Use our API or UI at the same transparent pricing.
                        </li>
                        <li>
                            <strong>Transparent:</strong> Only pay for what you use.
                        </li>
                    </ul>
                </div>
            </section>

            <section className="contact-section">
                <h2>Contact Us</h2>
                <p>
                    Checkout other projects by us at{' '}
                    <a href="https://appliedai-org.github.io/homepage/">
                        appliedai-org.github.io/homepage
                    </a>.
                </p>
                <p>
                    If you have any questions, feedback, or need support, please reach out to us at{' '}
                    <a href="mailto:applied.ai.help@gmail.com">
                        applied.ai.help@gmail.com
                    </a>, or submit an issue on{' '}
                    <a
                        href="https://github.com/AppliedAI-Org"
                        target="_blank"
                        rel="noopener noreferrer"
                    >
                        our GitHub page
                    </a>.
                </p>
            </section>

            <FooterSection />
        </div>
    );
}

export default HomePage;
