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
                audience: authConfig.audience, // Consistent audience
                scope: authConfig.scope,
            },
        });
    };

    return (
        <div className="home-page-container">
            <NavMenu />
            <header className="hero-section">
                <div className="hero-content">
                    <h1>Subtitle Goes Here</h1>
                    <p>
                        We can place a small little blurb here giving some additional data about the product, how it can be used, and 
                        more data that's useful. Really sell the tool, product or service.
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
                                <td>Paid Tier</td>
                                <td>Paid requests amount number</td>
                                <td>✔️</td>
                                <td>$200</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div className="features">
                    <h2>Why Should You Use This Project?</h2>
                    <ul>
                        <li>
                            <strong>Reason 1:</strong> This is the first reason.
                        </li>
                        <li>
                            <strong>Reason 2:</strong> This is the second reason.
                        </li>
                        <li>
                            <strong>Reason 3:</strong> This is the third reason.
                        </li>
                        <li>
                            <strong>Reason 4:</strong> This is the fourth reason.
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
