using System;
using System.Collections.Generic;
using App1.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace App1;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private CubeRenderer cubeRenderer;
    private Camera camera;
    private List<CubeData> cubes;
    
    ContentManager content => Content;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        camera = new Camera(GraphicsDevice);

        Effect effect;
        try
        {
            effect = content.Load<Effect>("Effects/InstancedEffect");
            Console.WriteLine("Effect loaded successfully.");
        }
        catch (ContentLoadException e)
        {
            throw new ContentLoadException("Failed to load InstancedEffect. Make sure the .fx file is properly added to the Content project.", e);
        }
        
        cubeRenderer = new CubeRenderer(GraphicsDevice, effect, camera);
        
        CreateTestCubes();
        
        base.Initialize();
    }
    
    private void CreateTestCubes()
    {
        cubes = new List<CubeData>();
        Random random = new Random();
        
        // for (int i = 0; i < 1; i++)
        // {
        //     cubes.Add(new CubeData
        //     {
        //         Position = new Vector3(
        //             random.Next(-50, 50),
        //             random.Next(-50, 50),
        //             random.Next(-50, 50)
        //         ),
        //         Color = new Color(
        //             random.Next(0, 255),
        //             random.Next(0, 255),
        //             random.Next(0, 255)
        //         )
        //     });
        //     Console.WriteLine($"Cube {i} created at {cubes[i].Position}");
        // }
        
        cubes.Add(
            new CubeData
            {
                Position = new Vector3(0, 0, 0),
                Color = Color.Red,
                Scale = new Vector3(30, 30, 30)
            }
        );
        
        cubes.Add(
            new CubeData
            {
                Position = new Vector3(10, 0, 0),
                Color = Color.Green,
                Scale = new Vector3(2, 2, 2)
            }
        );
        
        cubeRenderer.UpdateInstances(cubes);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        HandleInput(gameTime);

        base.Update(gameTime);
    }
    
    private void HandleInput(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        float moveSpeed = 10f;
        
        Vector3 rotation = new Vector3(0, 0, 0);
        Vector3 move = new Vector3(0, 0, 0);
        
        if (keyboardState.IsKeyDown(Keys.Up))
            rotation.X += 0.01f;
        if (keyboardState.IsKeyDown(Keys.Down))
            rotation.X -= 0.01f;
        if (keyboardState.IsKeyDown(Keys.Left))
            rotation.Y += 0.01f;
        if (keyboardState.IsKeyDown(Keys.Right))
            rotation.Y -= 0.01f;
        
        camera.Rotate(rotation);

        if (keyboardState.IsKeyDown(Keys.Z))
            move.Z -= moveSpeed;
        if (keyboardState.IsKeyDown(Keys.S))
            move.Z += moveSpeed;
        if (keyboardState.IsKeyDown(Keys.Q))
            move.X -= moveSpeed;
        if (keyboardState.IsKeyDown(Keys.D))
            move.X += moveSpeed;
        if (keyboardState.IsKeyDown(Keys.Space))
            move.Y += moveSpeed;
        if (keyboardState.IsKeyDown(Keys.LeftShift))
            move.Y -= moveSpeed;
        
        camera.Move(move);
        
        cubeRenderer.UpdateViewProjection(camera);
        // Console.WriteLine($"Camera position: {camera.Position}");
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        cubeRenderer.Draw();

        base.Draw(gameTime);
    }
}