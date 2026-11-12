using System;
using System.Collections.Generic;
using System.Linq;

public class QuestionAnswering
{
    private readonly Dictionary<string, Func<string, string>> _answers;

    public QuestionAnswering()
    {
        _answers = new Dictionary<string, Func<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            // Why do you want to work here?
            { "why", company =>
                $"I'm excited about the opportunity to contribute to a team like {company}. " +
                "Your focus on innovation, growth, and building meaningful solutions aligns with my career goals " +
                "and the type of environment where I do my best work."
            },

            // Relocation
            { "relocate", _ =>
                "I am open to relocation if the role is a strong fit and offers long-term growth opportunities."
            },

            // Sponsorship
            { "sponsorship", _ =>
                "I do not require sponsorship now or in the future."
            },

            // Expected salary
            { "salary", _ =>
                "My expected salary is flexible and depends on the overall compensation package, benefits, and growth opportunities."
            },

            // Tell us about yourself
            { "yourself", _ =>
                "I'm a motivated software developer with hands-on experience in C#, .NET, SQL, and building real-world applications. " +
                "I enjoy solving problems, learning new technologies, and contributing to meaningful projects."
            },

            // C# experience
            { "c#", _ =>
                "I have experience building applications using C#, .NET, and ASP.NET. " +
                "I've worked with APIs, backend logic, and real-world project structures."
            },

            // React experience
            { "react", _ =>
                "I have experience building front-end components with React, including state management, hooks, and API integration."
            },

            // SQL experience
            { "sql", _ =>
                "I have experience writing SQL queries, joins, stored procedures, and working with relational databases."
            }
        };
    }

    public string GenerateAnswer(string questionText, string companyName)
    {
        questionText = questionText.ToLower();

        foreach (var key in _answers.Keys)
        {
            if (questionText.Contains(key))
            {
                return _answers[key](companyName);
            }
        }

        // Fallback generic answer
        return "I am confident that my skills and experience make me a strong fit for this role.";
    }
}

