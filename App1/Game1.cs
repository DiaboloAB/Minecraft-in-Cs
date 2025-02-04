using System;
using System.Collections.Generic;
using App1.Core.World;
using App1.Graphics;
using App1.Graphics.Renderer;
using App1.Graphics.Textures;
using App1.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace App1;

public class Game1 : Game
{
    private OrientationGraph orientationGraph;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Vector2 lastMousePosition;
    private World world;

    private Texture2D defaultTexture;
    Texture2D texture;
    Texture2D texture2;
    
    private CubeRenderer cubeRenderer;
    private FaceRenderer faceRenderer;
    private Renderer renderer;
    
    private ChunkGenerator chunkGenerator;
    
    private Camera camera;
    private ChunkMeshBuilder chunkMeshBuilder;
    private Chunk[] chunks;
    
    private double fps = 0;
    private double fpsTimer = 0;
    
    ContentManager content => Content;
    
    Texture2D crosshair;
    
    Atlas atlas;
    
    bool debug = false;
    bool debugSwitch = false;

    public Game1()
    {
        world = new World(467465678);
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";                 
        // IsMouseVisible = false;
        _graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60);

    }

    protected override void Initialize()
    {
        camera = new Camera(GraphicsDevice);
        camera.Position = new Vector3(0, 50, 0);
        
        // Effect effect;
        Effect effectbis;
        Effect vertexEffect;
        try
        {
            // effect = content.Load<Effect>("Effects/InstancedEffect");
            effectbis = content.Load<Effect>("Effects/FaceEffect");
            vertexEffect = Content.Load<Effect>("Effects/VertexEffect");
            Console.WriteLine("All Effects loaded successfully.");
        }
        catch (ContentLoadException e)
        {
            throw new ContentLoadException("Failed to load 1 Effect. Make sure the .fx file is properly added to the Content project.", e);
        }
        texture = content.Load<Texture2D>("Textures/Grass");
        texture2 = content.Load<Texture2D>("Textures/Stone");

        Dictionary<string, Texture2D> textureDic = new Dictionary<string, Texture2D>();
        defaultTexture = content.Load<Texture2D>("Textures/Default");
        crosshair = content.Load<Texture2D>("Textures/Crosshair");
        textureDic.Add("grass", texture);
        textureDic.Add("stone", texture2);
        textureDic.Add("default", defaultTexture);
        textureDic.Add("chest", content.Load<Texture2D>("Textures/Chest"));
        textureDic.Add("crosshair", content.Load<Texture2D>("Textures/Crosshair"));
        textureDic.Add("leaves", content.Load<Texture2D>("Textures/Leaves"));
        textureDic.Add("wood", Content.Load<Texture2D>("textures/Wood"));
        
        atlas = new Atlas(GraphicsDevice, new Vector2(2048, 2048), textureDic);
        atlas.Save();
        BlockTextureCoord.SetTextureCoords(atlas);
        orientationGraph = new OrientationGraph(GraphicsDevice);
        


        
        lastMousePosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        Mouse.SetPosition((int)lastMousePosition.X, (int)lastMousePosition.Y);
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();
        // cubeRenderer = new CubeRenderer(GraphicsDevice, effect, camera);
        faceRenderer = new FaceRenderer(GraphicsDevice, effectbis, camera, 16f / 2048f, 50000);
        faceRenderer.SetAtlasTexture(atlas.Texture);
        

        renderer = new Renderer(GraphicsDevice, vertexEffect);
        renderer.SetAtlasTexture(atlas.Texture);
        // CreateTestCubes();
        // CreateTestChests();
        CreateFaces();
        
        world.CreateChunksBuffers(GraphicsDevice);
        
        base.Initialize();
    }
    
    private void CreateTestChests()
    {
        BlockModel model = BlockModel.LoadModel(content, "Chest", GraphicsDevice);
        model.PrintModel();
        
        Texture2D texture = content.Load<Texture2D>("Textures/Chest");
        
        List<CubeData> cubes;
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
    
    private void CreateFaces()
    {
        List<FaceData> faces;
        
        faces = world.GetVisibleFaces(atlas);
        Console.WriteLine($"Vertexes: {faces.Count}");
        Console.WriteLine($"Faces: {faces.Count / 4}");
        faceRenderer.UpdateInstances(faces);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        HandleInput(gameTime);


        fpsTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (fpsTimer > 0.5f)
        {
            fps = 1 / gameTime.ElapsedGameTime.TotalSeconds;
            fpsTimer = 0;
        }

        base.Update(gameTime);
    }
    
    private void HandleInput(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        float moveSpeed = 25f;
        float rotationSpeed = 0.4f;
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
        // cubeRenderer.UpdateViewProjection(camera);
        faceRenderer.UpdateViewProjection(camera);
        renderer.UpdateViewProjection(camera);
        
        if (keyboardState.IsKeyDown(Keys.F1) && !debugSwitch)
        {
            debugSwitch = true;
            debug = !debug;
            if (debug)
                    Console.WriteLine("Debug mode enabled.");
            else
                Console.WriteLine("Debug mode disabled.");
            
        }
        if (keyboardState.IsKeyUp(Keys.F1))
            debugSwitch = false;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // cubeRenderer.Draw();
        if (debug)
            faceRenderer.Draw();
        renderer.DrawWorld(world);
        Vector3 camRotation = camera.Rotation;
        orientationGraph.DrawOrientationGraph(camera);
        
        _spriteBatch.Begin();
        _spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/File"), $"FPS: {(int)fps}", new Vector2(10, 10), Color.White);
        _spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/File"), $"Facing: {camera.Facing}", new Vector2(10, 30), Color.White);
        _spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/File"), $"Position:  {(int) camera.Position.X}, {(int) camera.Position.Y}, {(int) camera.Position.Z}", new Vector2(10, 50), Color.White);
        _spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/File"), "Rotation: " + camRotation, new Vector2(10, 70), Color.White);
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