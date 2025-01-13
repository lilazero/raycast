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

        Vector2 closestIntersection = Vector2.Zero;
        bool hasIntersection = false;
        float closestDistance = float.MaxValue;

        // Check each edge and find the closest intersection point
        Vector2 tempIntersection;
        if (LineIntersectsLine(start, end, new Vector2(left, top), new Vector2(right, top), out tempIntersection))
        {
            float dist = Vector2.DistanceSquared(start, tempIntersection);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIntersection = tempIntersection;
                hasIntersection = true;
            }
        }
        if (LineIntersectsLine(start, end, new Vector2(right, top), new Vector2(right, bottom), out tempIntersection))
        {
            float dist = Vector2.DistanceSquared(start, tempIntersection);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIntersection = tempIntersection;
                hasIntersection = true;
            }
        }
        if (LineIntersectsLine(start, end, new Vector2(right, bottom), new Vector2(left, bottom), out tempIntersection))
        {
            float dist = Vector2.DistanceSquared(start, tempIntersection);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIntersection = tempIntersection;
                hasIntersection = true;
            }
        }
        if (LineIntersectsLine(start, end, new Vector2(left, bottom), new Vector2(left, top), out tempIntersection))
        {
            float dist = Vector2.DistanceSquared(start, tempIntersection);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIntersection = tempIntersection;
                hasIntersection = true;
            }
        }

        if (hasIntersection)
        {
            intersection = closestIntersection;
            return true;
        }
        return false;
    }

    private bool LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
    {
        intersection = Vector2.Zero;

        float denominator = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);
        
        if (denominator == 0)
            return false;

        float ua = ((b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X)) / denominator;
        float ub = ((a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X)) / denominator;

        if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
        {
            intersection = new Vector2(
                a1.X + ua * (a2.X - a1.X),
                a1.Y + ua * (a2.Y - a1.Y)
            );
            return true;
        }

        return false;
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
