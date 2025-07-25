import React from 'react';
import FooterSection from './FooterSection';
import { NavMenu } from './NavMenu';
import './HomePage.css';
import WindowWrapper from './WindowWrapper';
import AboutMe from './AboutMe';
import Projects from './Projects';
import Skills from './Skills';

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
