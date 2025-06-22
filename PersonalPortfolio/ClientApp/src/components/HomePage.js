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
                <AboutMe />
                <WindowWrapper />
            </section>

            <section id="projects-section" className="projects-section">
                <Projects />
            </section>

            <section id="skills-section" className="skills-section">
                <Skills />
            </section>

            <section id="contact-section" className="contact-section">
                <h2 className="section-title">Contact Me</h2>
                <p>
                    <strong>Phone:</strong> <a href="tel:6308802110">630-880-2110</a>
                </p>
                <p>
                    <strong>Email:</strong> <a href="mailto:JacobJonThomas@gmail.com">JacobJonThomas@gmail.com</a>
                </p>
                <p>
                    <strong>LinkedIn:</strong> <a href="https://www.linkedin.com/in/jacob-thomas-546268131/">My LinkedIn Profile</a>
                </p>
                <p>
                    <strong>GitHub:</strong> <a href="https://github.com/Jacob-J-Thomas">My GitHub Profile</a>
                </p>
                <p>
                    <strong>Applied AI Website:</strong> <a href="https://appliedai-org.github.io/homepage/">Applied AI Landing Page</a>
                </p>
                <p>
                    <strong>Applied AI Email:</strong> <a href="mailto:jacob.thomas@applied-ai-org.com">jacob.thomas@applied-ai-org.com</a>
                </p>
                <p>
                    <strong>Applied AI GitHub:</strong> <a href="https://github.com/AppliedAI-Org">jacob.thomas@applied-ai-org.com</a>
                </p>
            </section>

            <FooterSection />
        </div>
    );
}

export default HomePage;
