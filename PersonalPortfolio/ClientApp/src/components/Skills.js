import React from 'react';
import './Skills.css';

const Skills = () => {
    const skillsList = [  
                 "C#",  
                 ".NET 8",  
                 "React",  
                 "JavaScript",  
                 "HTML & CSS",  
                 "SQL",  
                 "Entity Framework",  
                 "Azure",  
                 "Azure AI Services",  
                 "Azure OpenAI",  
                 "Git & GitHub",  
                 "AI/ML Development",  
                 "PyTorch",  
                 "Python",  
                 "Prompt Engineering",  
                 "API Development",  
                 "ASP.NET",  
                 "OpenAI",  
                 "Resilient Design & Disaster Recovery"  
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
