using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace csharp;

public class Triangle
{
    public Vector2 Position { get; set; }
    public float Size { get; set; }

    public Triangle(Vector2 position, float size)
    {
        Position = position;
        Size = size;
    }

    public bool IsHitByRay(Vector2 rayStart, Vector2 rayEnd)
    {
        Vector2[] vertices = GetVertices();
        for (int i = 0; i < 3; i++)
        {
            Vector2 start = vertices[i];
            Vector2 end = vertices[(i + 1) % 3];
            if (LineIntersectsLine(rayStart, rayEnd, start, end, out _))
                return true;
        }
        return false;
    }

    private Vector2[] GetVertices()
    {
        return new Vector2[]
        {
            Position + new Vector2(0, -Size),
            Position + new Vector2(-Size * 0.866f, Size * 0.5f),
            Position + new Vector2(Size * 0.866f, Size * 0.5f)
        };
    }

    public void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect, Color color)
    {
        Vector2[] vertices = GetVertices();
        VertexPositionColor[] vpc = new VertexPositionColor[4];
        
        for (int i = 0; i < 3; i++)
        {
            vpc[i] = new VertexPositionColor(new Vector3(vertices[i], 0), color);
        }
        vpc[3] = vpc[0];

        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vpc, 0, 3);
        }
    }

    private bool LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
    {
        intersection = Vector2.Zero;
        Vector2 b = a2 - a1;
        Vector2 d = b2 - b1;
        float bDotDPerp = b.X * d.Y - b.Y * d.X;
        if (bDotDPerp == 0) return false;
        Vector2 c = b1 - a1;
        float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
        if (t < 0 || t > 1) return false;
        float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
        if (u < 0 || u > 1) return false;
        intersection = a1 + t * b;
        return true;
    }
}
