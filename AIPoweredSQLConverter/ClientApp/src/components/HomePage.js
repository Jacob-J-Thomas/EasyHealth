import React from 'react';
import FooterSection from './FooterSection';
import { NavMenu } from './NavMenu';
import './HomePage.css';

function HomePage() {
    return (
        <div className="home-page-container">
            <NavMenu />
            <div className="home-page-content">
                <h1>Contact Us</h1>
                <p>If you have any questions, feedback, or concerns, please do not hesitate to reach out to us. We value your input and are committed to improving our service.</p>    
                <p>Any issues can be logged at the GitHub URL, or sent to us directly at the email address provided below.</p>
                <div className="contact-details">
                    <h2>Contact Information</h2>
                    <p>Email: <a href="mailto:applied.ai.help@gmail.com">applied.ai.help@gmail.com</a></p>
                    <p>Github: <a href="https://github.com/AppliedAI-Org">https://github.com/AppliedAI-Org</a></p>
                </div>
                <div className="usage-info">
                    <h2>Usage Information</h2>
                    <p>NLSequel allows you to convert natural language to SQL with a limit of 10 free requests per day. Additional requests follow a "pay as you go" model. For enterprise subscriptions, please contact our sales team directly.</p>
                    <p>To get an API keys and directions, see the API Reference tab.</p>
                </div>
            </div>
            <FooterSection />
        </div>
    );
}

export default HomePage;
