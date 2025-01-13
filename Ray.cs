using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace csharp;

public class Ray
{
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; }

    public Ray(Vector2 position, Vector2 direction)
    {
        Position = position;
        Direction = Vector2.Normalize(direction);
    }

    public void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
    {
        VertexPositionColor[] vertices = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(Position, 0), Color.Yellow),
            new VertexPositionColor(new Vector3(Position + Direction * 200, 0), Color.Yellow)
        };

        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }

    public void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect, Square blocker)
    {
        Vector2 endPoint = Position + Direction * 2000;
        if (blocker.Intersects(Position, endPoint, out Vector2 intersection))
        {
            endPoint = intersection;
        }

        VertexPositionColor[] vertices = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(Position, 0), Color.Yellow),
            new VertexPositionColor(new Vector3(endPoint, 0), Color.Yellow)
        };

        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
