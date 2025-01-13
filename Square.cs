using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace csharp;

public class Square
{
    public Vector2 Position { get; set; }
    public float Size { get; set; }

    public Square(Vector2 position, float size)
    {
        Position = position;
        Size = size;
    }

    public bool Intersects(Vector2 start, Vector2 end, out Vector2 intersection)
    {
        intersection = Vector2.Zero;
        float left = Position.X - Size / 2;
        float right = Position.X + Size / 2;
        float top = Position.Y - Size / 2;
        float bottom = Position.Y + Size / 2;

        // Check each edge of the square for intersection
        if (LineIntersectsLine(start, end, new Vector2(left, top), new Vector2(right, top), out intersection) ||
            LineIntersectsLine(start, end, new Vector2(right, top), new Vector2(right, bottom), out intersection) ||
            LineIntersectsLine(start, end, new Vector2(right, bottom), new Vector2(left, bottom), out intersection) ||
            LineIntersectsLine(start, end, new Vector2(left, bottom), new Vector2(left, top), out intersection))
        {
            return true;
        }
        return false;
    }

    private bool LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
    {
        intersection = Vector2.Zero;

        Vector2 b = a2 - a1;
        Vector2 d = b2 - b1;
        float bDotDPerp = b.X * d.Y - b.Y * d.X;

        if (bDotDPerp == 0)
            return false;

        Vector2 c = b1 - a1;
        float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
        if (t < 0 || t > 1)
            return false;

        float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
        if (u < 0 || u > 1)
            return false;

        intersection = a1 + t * b;
        return true;
    }

    public void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
    {
        VertexPositionColor[] vertices = new VertexPositionColor[5];
        float halfSize = Size / 2;

        vertices[0] = new VertexPositionColor(new Vector3(Position.X - halfSize, Position.Y - halfSize, 0), Color.Blue);
        vertices[1] = new VertexPositionColor(new Vector3(Position.X + halfSize, Position.Y - halfSize, 0), Color.Blue);
        vertices[2] = new VertexPositionColor(new Vector3(Position.X + halfSize, Position.Y + halfSize, 0), Color.Blue);
        vertices[3] = new VertexPositionColor(new Vector3(Position.X - halfSize, Position.Y + halfSize, 0), Color.Blue);
        vertices[4] = vertices[0];

        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 4);
        }
    }
}
