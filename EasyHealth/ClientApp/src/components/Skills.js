import React from 'react';
import './Skills.css';

const Skills = () => {
    const skillsList = [  
                 ".NET (C#)",  
                 "JavaScript",  
                 "Python",
                 "SQL",
                 "React",
                 "Azure",  
                 "Azure AI Services", 
                 "Azure OpenAI",
                 "Git", 
                 "HTML",
                 "CSS",
                 "PyTorch",  
                 "Prompt Engineering" 
             ];

    return (
        <div className="skills-container">
            <h1 className="skills-title">Skills</h1>
            <div className="skills-list">
                {skillsList.map((skill, index) => (
                    <div key={index} className="skill-card">
                        {skill}
                    </div>
                ))}
            </div>
        </div>
    );
};

export default Skills;
