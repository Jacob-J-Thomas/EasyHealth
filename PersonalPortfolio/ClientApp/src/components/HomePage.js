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
                    <strong>Follow me on X:</strong> <a href="https://x.com/JacobThoma38517">@JacobThoma38517</a>
                </p>
                <p>
                    <strong>LLC Website:</strong> <a href="https://appliedai-org.github.io/homepage/">Applied AI Landing Page</a>
                </p>
                <p>
                    <strong>LLC Email:</strong> <a href="mailto:jacob.thomas@applied-ai-org.com">jacob.thomas@applied-ai-org.com</a>
                </p>
                <p>
                    You can also check out my work on{' '}
                    <a
                        href="https://github.com/AppliedAI-Org"
                        target="_blank"
                        rel="noopener noreferrer"
                    >
                        My Startup's GitHub
                    </a> and{' '}
                    <a
                        href="https://github.com/Jacob-J-Thomas"
                        target="_blank"
                        rel="noopener noreferrer"
                    >
                        My Personal GitHub
                    </a>.
                </p>
            </section>

            <FooterSection />
        </div>
    );
}

export default HomePage;
