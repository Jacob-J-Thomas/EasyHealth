import React from 'react';
import { NavMenu } from './NavMenu';
import './HomePage.css';
import WindowWrapper from './WindowWrapper';

function HomePage() {
    return (
        <div className="home-page-container">
            <NavMenu />
            <section id="about-and-window-wrapper" className="about-and-window-wrapper">
                <WindowWrapper />
            </section>
        </div>
    );
}

export default HomePage;
