using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace csharp;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Circle _emitter;
    private Ray[] _rays;
    private BasicEffect _basicEffect;

    // Add these fields at the top with other private fields
    private MouseState _currentMouseState;
    private MouseState _previousMouseState;
    private Vector2 _dragOffset;

    // Add new field
    private Square _blocker;

    // Add new field to track which shape is being dragged
    private enum DragTarget
    {
        None,
        Circle,
        Square,
        Triangle
    }

    private DragTarget _currentDragTarget = DragTarget.None;

    private Triangle _player;
    private bool _gameOver;
    private SpriteFont _gameFont;
    private const float EMITTER_SPEED = 100f; // pixels per second

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 800; // Set window size
        _graphics.PreferredBackBufferHeight = 600;
        _graphics.ApplyChanges(); // Add this line
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Create circle emitter at the center of the screen
        _emitter = new Circle(
            new Vector2(
                _graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2
            ),
            50f, // Made circle bigger
            360
        );

        // Initialize rays
        _rays = new Ray[_emitter.NumberOfRays];
        for (int i = 0; i < _emitter.NumberOfRays; i++)
        {
            float angle = MathHelper.ToRadians(i);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            _rays[i] = new Ray(_emitter.Position, direction);
        }

        _basicEffect = new BasicEffect(GraphicsDevice)
        {
            VertexColorEnabled = true,
            World = Matrix.Identity,
            View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up),
            Projection = Matrix.CreateOrthographicOffCenter(
                0,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                0,
                0.1f,
                2f
            )
        };

        // Add blocker initialization
        _blocker = new Square(
            new Vector2(
                _graphics.PreferredBackBufferWidth / 2 + 200,
                _graphics.PreferredBackBufferHeight / 2
            ),
            100f
        );

        _player = new Triangle(new Vector2(_blocker.Position.X, _blocker.Position.Y), 20f);
        _gameOver = false;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        try
        {
            _gameFont = Content.Load<SpriteFont>("GameFont");
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading font: {e.Message}");
            Exit();
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        if (_gameOver)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.R))
                ResetGame();
            return;
        }

        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();

        Vector2 mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);

        // Move emitter automatically
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _emitter.Position += new Vector2(EMITTER_SPEED * deltaTime, 0);
        if (_emitter.Position.X > _graphics.PreferredBackBufferWidth)
            _emitter.Position = new Vector2(0, _emitter.Position.Y);

        UpdateRayPositions();

        // Check if any ray hits the triangle
        foreach (var ray in _rays)
        {
            Vector2 rayEnd = ray.Position + ray.Direction * 2000;
            if (_blocker.Intersects(ray.Position, rayEnd, out Vector2 intersection))
                rayEnd = intersection;

            if (_player.IsHitByRay(ray.Position, rayEnd))
            {
                _gameOver = true;
                break;
            }
        }

        // Handle dragging
        HandleDragging();

        // Handle dragging start
        if (
            _currentMouseState.LeftButton == ButtonState.Pressed
            && _previousMouseState.LeftButton == ButtonState.Released
        )
        {
            if (IsMouseOverCircle(mousePosition))
            {
                _currentDragTarget = DragTarget.Circle;
                _dragOffset = _emitter.Position - mousePosition;
            }
            else if (IsMouseOverSquare(mousePosition))
            {
                _currentDragTarget = DragTarget.Square;
                _dragOffset = _blocker.Position - mousePosition;
            }
        }
        else if (_currentMouseState.LeftButton == ButtonState.Released)
        {
            _currentDragTarget = DragTarget.None;
        }

        // Update positions while dragging
        if (_currentMouseState.LeftButton == ButtonState.Pressed)
        {
            switch (_currentDragTarget)
            {
                case DragTarget.Circle:
                    _emitter.Position = mousePosition + _dragOffset;
                    UpdateRayPositions();
                    break;
                case DragTarget.Square:
                    _blocker.Position = mousePosition + _dragOffset;
                    break;
            }
        }

        base.Update(gameTime);
    }

    private void HandleDragging()
    {
        Vector2 mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);

        if (
            _currentMouseState.LeftButton == ButtonState.Pressed
            && _previousMouseState.LeftButton == ButtonState.Released
        )
        {
            if (IsMouseOverTriangle(mousePosition))
            {
                _currentDragTarget = DragTarget.Triangle;
                _dragOffset = _player.Position - mousePosition;
            }
            else if (IsMouseOverSquare(mousePosition))
            {
                _currentDragTarget = DragTarget.Square;
                _dragOffset = _blocker.Position - mousePosition;
            }
        }
        else if (_currentMouseState.LeftButton == ButtonState.Released)
        {
            _currentDragTarget = DragTarget.None;
        }

        if (_currentMouseState.LeftButton == ButtonState.Pressed)
        {
            switch (_currentDragTarget)
            {
                case DragTarget.Square:
                    _blocker.Position = mousePosition + _dragOffset;
                    break;
                case DragTarget.Triangle:
                    _player.Position = mousePosition + _dragOffset;
                    break;
            }
        }
    }

    private bool IsMouseOverCircle(Vector2 mousePosition)
    {
        return Vector2.Distance(mousePosition, _emitter.Position) <= _emitter.Radius;
    }

    private bool IsMouseOverSquare(Vector2 mousePosition)
    {
        float halfSize = _blocker.Size / 2;
        return mousePosition.X >= _blocker.Position.X - halfSize
            && mousePosition.X <= _blocker.Position.X + halfSize
            && mousePosition.Y >= _blocker.Position.Y - halfSize
            && mousePosition.Y <= _blocker.Position.Y + halfSize;
    }

    private bool IsMouseOverTriangle(Vector2 mousePosition)
    {
        return Vector2.Distance(mousePosition, _player.Position) <= _player.Size;
    }

    private void UpdateRayPositions()
    {
        for (int i = 0; i < _rays.Length; i++)
        {
            float angle = MathHelper.ToRadians(i);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            _rays[i] = new Ray(_emitter.Position, direction);
        }
    }

    private void ResetGame()
    {
        _gameOver = false;
        _emitter.Position = new Vector2(0, _graphics.PreferredBackBufferHeight / 2);
        _blocker.Position = new Vector2(
            _graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2
        );
        _player.Position = _blocker.Position;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black); // Changed to black for better contrast

        // Set render states
        GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        GraphicsDevice.BlendState = BlendState.AlphaBlend;
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        // Draw rays, circle, and blocker
        foreach (var ray in _rays)
        {
            ray.Draw(GraphicsDevice, _basicEffect, _blocker);
        }
        _emitter.Draw(GraphicsDevice, _basicEffect);
        _blocker.Draw(GraphicsDevice, _basicEffect);
        _player.Draw(GraphicsDevice, _basicEffect, _gameOver ? Color.Red : Color.Green);

        // Draw game over text
        if (_gameOver)
        {
            _spriteBatch.Begin();
            string text = "Game Over! Press R to restart";
            Vector2 textSize = _gameFont.MeasureString(text);
            _spriteBatch.DrawString(
                _gameFont,
                text,
                new Vector2(
                    _graphics.PreferredBackBufferWidth / 2 - textSize.X / 2,
                    _graphics.PreferredBackBufferHeight / 2 - textSize.Y / 2
                ),
                Color.White
            );
            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }
}
