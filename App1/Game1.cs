using System;
using System.Collections.Generic;
using App1.Core.World;
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
    private Vector2 lastMousePosition;

    private Texture2D defaultTexture;
    Texture2D texture;
    Texture2D texture2;
    private List<CubeData> cubes;
    
    private CubeRenderer cubeRenderer;
    
    private ChunkGenerator chunkGenerator;
    
    private Camera camera;
    private ChunkMeshBuilder chunkMeshBuilder;
    private Chunk[] chunks;
    
    private double fps = 0;
    private List<double> fpsList = new List<double>();
    
    ContentManager content => Content;
    
    Texture2D crosshair;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        // _graphics.SynchronizeWithVerticalRetrace = false; 
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        int seed = 2134512354;
        chunkGenerator = new ChunkGenerator(seed);
    }

    protected override void Initialize()
    {
        camera = new Camera(GraphicsDevice);
        camera.Position = new Vector3(0, 50, 0);
        
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
        texture = content.Load<Texture2D>("Textures/Grass");
        texture2 = content.Load<Texture2D>("Textures/Stone");
        defaultTexture = content.Load<Texture2D>("Textures/Default");
        chunkGenerator.Textures = new Texture2D[]
        {
            defaultTexture,
            texture,
            texture2
        };
        chunks = new Chunk[50];
        int i = 0;
        // for (int x = 0; x < 2; x++)
        //     for (int z = 0; z < 2; z++)
        //     {   
        //         chunks[i++] = chunkGenerator.GenerateChunk(x, z);
        //     }
        chunks[0] = chunkGenerator.GenerateChunk(0, 0);
        
        lastMousePosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        Mouse.SetPosition((int)lastMousePosition.X, (int)lastMousePosition.Y);
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        IsFixedTimeStep = false;
        _graphics.ApplyChanges();
        cubeRenderer = new CubeRenderer(GraphicsDevice, effect, camera);
        
        CreateTestCubes();
        CreateTestChests();
        
        crosshair = content.Load<Texture2D>("Textures/Crosshair");
        
        base.Initialize();
    }

    private void CreateTestChests()
    {
        BlockModel model = BlockModel.LoadModel(content, "Chest", GraphicsDevice);
        model.PrintModel();
        
        Texture2D texture = content.Load<Texture2D>("Textures/Chest");
        
        cubes = new List<CubeData>();
        
        cubes.Add(
            new CubeData
            {
                Position = new Vector3(0, 0, 0),
                Scale = new Vector3(1, 1, 1),
                Texture = texture
            }
        );

        
        cubeRenderer.UpdateComplexInstances(cubes, model);
        
    }
    
    private void CreateTestCubes()
    {
        cubes = new List<CubeData>();

        for (int i = 0; i < 1; i++)
        {
            cubes.AddRange(chunks[i].getVisibleCubes(chunkGenerator.Textures));
        }

        Console.WriteLine($"Cubes: {cubes.Count}");
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
        
        fpsList.Add(1 / gameTime.ElapsedGameTime.TotalSeconds);
        
        if (fpsList.Count > 10)
        {
            fpsList.RemoveAt(0);
        }
        double sum = 0;
        foreach (var f in fpsList)
            sum += f;
        
        fps = sum / fpsList.Count;
        
        
        
        base.Update(gameTime);
    }
    
    private void HandleInput(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        float moveSpeed = 7f;
        float rotationSpeed = 0.25f;
        var mouseState = Mouse.GetState();
        
        Vector3 move = new Vector3(0, 0, 0);

        if (keyboardState.IsKeyDown(Keys.Z))
            move.Z += moveSpeed;
        if (keyboardState.IsKeyDown(Keys.S))
            move.Z -= moveSpeed;
        if (keyboardState.IsKeyDown(Keys.Q))
            move.X += moveSpeed;
        if (keyboardState.IsKeyDown(Keys.D))
            move.X -= moveSpeed;
        if (keyboardState.IsKeyDown(Keys.Space))
            move.Y += moveSpeed;
        if (keyboardState.IsKeyDown(Keys.LeftShift))
            move.Y -= moveSpeed;
        
        camera.Move(move * (float)gameTime.ElapsedGameTime.TotalSeconds);
        
        Vector2 mouseDelta = new Vector2(mouseState.X, mouseState.Y) - lastMousePosition;
        if ( mouseDelta != Vector2.Zero)
            camera.Rotate(new Vector3(-mouseDelta.Y * rotationSpeed, -mouseDelta.X * rotationSpeed, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds);

        Mouse.SetPosition((int)lastMousePosition.X, (int)lastMousePosition.Y);
        
        cubeRenderer.UpdateViewProjection(camera);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        
        foreach (var chunk in chunks)
        {
            // if (chunk.IsDirty)
            // {
            //     chunk.IsDirty = false;
            //     // chunkMeshBuilder.BuildMeshes(chunk);
            //     // cubeRenderer.UpdateInstances(chunkMeshBuilder.GetCubes());
            // }
        }
        
        
        cubeRenderer.Draw();
        
        _spriteBatch.Begin();
        _spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/File"), $"FPS: {(int)fps}", new Vector2(10, 10), Color.White);
        _spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/File"), $"Facing: {camera.Facing}", new Vector2(10, 30), Color.White);
        _spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/File"), $"Position:  {(int) camera.Position.X}, {(int) camera.Position.Y}, {(int) camera.Position.Z}", new Vector2(10, 50), Color.White);
        _spriteBatch.Draw(
            crosshair, 
            new Vector2(GraphicsDevice.Viewport.Width / 2 - crosshair.Width, GraphicsDevice.Viewport.Height / 2 - crosshair.Height), 
            null, 
            Color.White, 
            0f, 
            Vector2.Zero, 
            2f, 
            SpriteEffects.None, 
            0f
        );
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}