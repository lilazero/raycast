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

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 800;  // Set window size
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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black); // Changed to black for better contrast

        // Set render states
        GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        GraphicsDevice.BlendState = BlendState.AlphaBlend;
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        // Draw rays and circle
        _emitter.Draw(GraphicsDevice, _basicEffect);
        foreach (var ray in _rays)
        {
            ray.Draw(GraphicsDevice, _basicEffect);
        }

        base.Draw(gameTime);
    }
}
