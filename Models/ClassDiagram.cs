using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;

namespace PathFindingClassDiagram.Models
{
    public class ClassDiagram
    {
        /// <summary>
        /// The name of the class
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// List of class attributes
        /// </summary>
        public List<string> Attributes { get; set; }

        /// <summary>
        /// List of class methods
        /// </summary>
        public List<string> Methods { get; set; }

        /// <summary>
        /// List of relationships with other classes
        /// </summary>
        public List<Relationship> Relationships { get; set; }

        /// <summary>
        /// List of diagram connection points
        /// </summary>
        public List<ClassDiagramPoint> Points { get; set; }

        /// <summary>
        /// Initializes a new instance of the ClassDiagram class
        /// </summary>
        public ClassDiagram(string className, List<string> attributes, List<string> methods, List<Relationship> relationships)
        {
            ClassName = className;
            Attributes = attributes ?? new List<string>();
            Methods = methods ?? new List<string>();
            Relationships = relationships ?? new List<Relationship>();
            Points = new List<ClassDiagramPoint>();
        }

        /// <summary>
        /// Draws the class diagram on the specified graphics object
        /// </summary>
        public void Draw(Graphics g, float x, float y, List<ClassDiagram> allDiagrams)
        {
            if (g == null)
                return;

            float textMargin = 10f;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float attributesHeight = CalculateTextBlockHeight(g, Attributes);
            float methodsHeight = CalculateTextBlockHeight(g, Methods);

            float rectangleWidth = Math.Max(
                g.MeasureString(ClassName, new Font("Arial", 12f)).Width + 12f * textMargin,
                Math.Max(
                    (Attributes.Count > 0) ? Attributes.Max(attr => g.MeasureString(attr, new Font("Arial", 10f)).Width) : 0f,
                    (Methods.Count > 0) ? Methods.Max(method => g.MeasureString(method, new Font("Arial", 10f)).Width + 15f) : 0f
                )
            );

            float rectangleHeight = textMargin + attributesHeight + textMargin + methodsHeight + textMargin;

            Points = new List<ClassDiagramPoint>
            {
                new ClassDiagramPoint(
                    ClassName,
                    new PointF(x, y),
                    new PointF(x + rectangleWidth, y),
                    new PointF(x, y + rectangleHeight),
                    new PointF(x + rectangleWidth, y + rectangleHeight)
                )
            };

            Rectangle classRect = new Rectangle((int)x, (int)y, (int)rectangleWidth, (int)rectangleHeight);

            // Draw class name
            if (!string.IsNullOrEmpty(ClassName))
            {
                g.DrawRectangle(Pens.Black, classRect);
                float textX = classRect.X + (classRect.Width - g.MeasureString(ClassName, new Font("Arial", 12f)).Width) / 2f;
                g.DrawString(ClassName, new Font("Arial", 12f), Brushes.Black, textX, classRect.Y + textMargin);
                g.DrawLine(Pens.Black, classRect.X, classRect.Y + 30, classRect.Right, classRect.Y + 30);
            }

            // Draw attributes
            if (Attributes.Count > 0)
            {
                DrawTextBlock(g, classRect.Left + 10, classRect.Y + 40, Attributes);
                g.DrawLine(Pens.Black, classRect.X, (float)(classRect.Y + 40) + attributesHeight, classRect.Right, (float)(classRect.Y + 40) + attributesHeight);
            }

            // Draw methods
            if (Methods.Count > 0)
            {
                float methodsY = (float)(classRect.Y + 40) + attributesHeight + ((Attributes.Count > 0) ? textMargin : 0f);
                DrawTextBlock(g, classRect.Left + 10, methodsY, Methods);
            }
        }

        /// <summary>
        /// Calculates the height needed to display a block of text
        /// </summary>
        public float CalculateTextBlockHeight(Graphics g, List<string> textBlock)
        {
            if (g == null || textBlock == null || textBlock.Count == 0)
            {
                return 0f;
            }

            float totalHeight = 0f;
            foreach (string text in textBlock)
            {
                totalHeight += g.MeasureString(text, new Font("Arial", 10f)).Height + 20f;
            }

            return totalHeight;
        }

        /// <summary>
        /// Draws a block of text at the specified location
        /// </summary>
        public void DrawTextBlock(Graphics g, float x, float y, List<string> textBlock)
        {
            if (g == null || textBlock == null || textBlock.Count == 0)
                return;

            for (int i = 0; i < textBlock.Count; i++)
            {
                g.DrawString(textBlock[i], new Font("Arial", 10f), Brushes.Black, x, y);
                y += g.MeasureString(textBlock[i], new Font("Arial", 10f)).Height;

                if (i < textBlock.Count - 1)
                {
                    y += 15f;
                }
            }
        }

        /// <summary>
        /// Calculates the total height of the class diagram
        /// </summary>
        public float CalculateTotalHeight(Graphics g)
        {
            float textMargin = 10f;
            float attributesHeight = CalculateTextBlockHeight(g, Attributes);
            float methodsHeight = CalculateTextBlockHeight(g, Methods);

            return textMargin + attributesHeight + textMargin + methodsHeight + textMargin;
        }

        /// <summary>
        /// Calculates the total width of the class diagram
        /// </summary>
        public float CalculateTotalWidth(Graphics g)
        {
            if (g == null)
            {
                return 0f;
            }

            float textMargin = 10f;
            float classNameWidth = g.MeasureString(ClassName, new Font("Arial", 12f)).Width;
            float maxAttributeWidth = (Attributes.Any() ? Attributes.Max(attr => g.MeasureString(attr, new Font("Arial", 10f)).Width) : 0f);
            float maxMethodWidth = (Methods.Any() ? Methods.Max(method => g.MeasureString(method, new Font("Arial", 10f)).Width) : 0f);

            return Math.Max(classNameWidth + 12f * textMargin, Math.Max(maxAttributeWidth, maxMethodWidth));
        }

        /// <summary>
        /// Draws an arrow between two points
        /// </summary>
        public void DrawArrow(Graphics g, Pen pen, float sourceX, float sourceY, float targetX, float targetY)
        {
            g.DrawLine(pen, sourceX, sourceY, targetX, targetY);

            float angle = (float)Math.Atan2(targetY - sourceY, targetX - sourceX);
            float arrowSize = 10f;

            PointF arrow1 = new PointF(
                targetX - arrowSize * (float)Math.Cos(angle - Math.PI / 6.0),
                targetY - arrowSize * (float)Math.Sin(angle - Math.PI / 6.0)
            );

            PointF arrow2 = new PointF(
                targetX - arrowSize * (float)Math.Cos(angle + Math.PI / 6.0),
                targetY - arrowSize * (float)Math.Sin(angle + Math.PI / 6.0)
            );

            g.DrawLine(pen, targetX, targetY, arrow1.X, arrow1.Y);
            g.DrawLine(pen, targetX, targetY, arrow2.X, arrow2.Y);
        }
    }
}
