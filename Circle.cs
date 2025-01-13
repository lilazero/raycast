using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace csharp;

public class Circle
{
    public Vector2 Position { get; set; }
    public float Radius { get; set; }
    public int NumberOfRays { get; set; }

    public Circle(Vector2 position, float radius, int numberOfRays)
    {
        Position = position;
        Radius = radius;
        NumberOfRays = numberOfRays;
    }

    public void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
    {
        const int segments = 32;
        VertexPositionColor[] vertices = new VertexPositionColor[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = MathHelper.TwoPi * i / segments;
            Vector2 point =
                Position
                + new Vector2((float)Math.Cos(angle) * Radius, (float)Math.Sin(angle) * Radius);
            vertices[i] = new VertexPositionColor(new Vector3(point, 0), Color.Red); // Changed to red
        }

        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, segments);
        }
    }
}
