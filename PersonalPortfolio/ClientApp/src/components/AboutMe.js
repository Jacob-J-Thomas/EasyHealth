import React from 'react';
import './AboutMe.css';

const AboutMe = () => {
    return (
        <div className="about-me-container">
            <div className="about-me-photo-container">
                <img
                    src="/headshot.png"
                    alt="Profile"
                    className="about-me-photo"
                    style={{
                        width: '12rem',
                        height: '12rem',
                        borderRadius: '50%',
                        objectFit: 'contain'
                    }}
                />
            </div>
            <div className="about-me-text-bubble">
                <h1 className="about-me-title">About Me</h1>
                <p className="about-me-text">
                    I’m Jacob Thomas, a .NET AI engineer with 2+ years at Convergint where I built and scaled the "Convergint Virtual Assistant," boosting reliability and response relevancy. In 2024 I founded Applied AI and created The Intelligence Hub, an open-source API with RAG, agentic workflows, vision support, and much more.
                </p>
                <p className="about-me-text">
                    To get a better idea of my expertise and accomplishments, feel free to keep scrolling, download my resume from below, or have a discussion with my personal virtual assistant, StackSherpa.
                </p>
                <a
                    href="/resume.pdf"
                    download="Jacob_Thomas_Resume.pdf"
                    className="download-resume-button"
                >
                    Download My Resume
                </a>
            </div>
        </div>
    );
};

export default AboutMe;
