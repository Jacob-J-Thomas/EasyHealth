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
                    I am a .NET AI Engineer with over two years of experience at Convergint, where I developed a variety of AI solutions. More recently, I've been working on some products I host through my start up, <a href="https://appliedai-org.github.io/homepage/">Applied AI</a>.
                </p>
                <p className="about-me-text">
                    I've worked across the stack, implementing anything from the front end for natural language filters, to RAG vector pipelines for enterprise grade chatbots.
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
