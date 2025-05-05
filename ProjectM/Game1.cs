using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ProjectM.Core.Player;
using ProjectM.Core.World;
using ProjectM.Graphics;
using ProjectM.Graphics.Renderer;
using ProjectM.Graphics.Textures;
using ProjectM.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectM.Core;
using ProjectM.Debug.Tools;
using BlocModel = ProjectM.Graphics.BlocModel;


namespace ProjectM;

public class Game1 : Game
{
    private DebugTool debugTool;
    private Player player;
    
    private OrientationGraph orientationGraph;
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private World world;

    private Texture2D defaultTexture;
    private Texture2D texture;
    private Texture2D texture2;
    private Boolean displayChunkBorder = false;
    
    // private CubeRenderer cubeRenderer;
    // private FaceRenderer faceRenderer;
    private Renderer renderer;
    
    private ChunkRenderer chunkMeshBuilder;
    
    Texture2D crosshair;
    
    Atlas atlas;
    
    bool debug = false;
    bool debugSwitch = false;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";                 
        // IsMouseVisible = false;
        graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60);

    }

    protected override void Initialize()
    {
        world = new World(467465678, GraphicsDevice);
        player = new Player(GraphicsDevice);
        debugTool = new DebugTool(world, player, Content);

        
        // Effect effect;
        Effect effectbis;
        Effect vertexEffect;
        try
        {
            // effect = content.Load<Effect>("Effects/InstancedEffect");
            effectbis = Content.Load<Effect>("Effects/FaceEffect");
            vertexEffect = Content.Load<Effect>("Effects/VertexEffect");
            Console.WriteLine("All Effects loaded successfully.");
        }
        catch (ContentLoadException e)
        {
            throw new ContentLoadException("Failed to load 1 Effect. Make sure the .fx file is properly added to the Content project.", e);
        }
        texture = Content.Load<Texture2D>("Textures/Grass");
        texture2 = Content.Load<Texture2D>("Textures/Stone");

        Dictionary<string, Texture2D> textureDic = new Dictionary<string, Texture2D>();
        defaultTexture = Content.Load<Texture2D>("Textures/Default");
        crosshair = Content.Load<Texture2D>("Textures/Crosshair");
        textureDic.Add("grass", texture);
        textureDic.Add("stone", texture2);
        textureDic.Add("default", defaultTexture);
        textureDic.Add("chest", Content.Load<Texture2D>("Textures/Chest"));
        textureDic.Add("crosshair", Content.Load<Texture2D>("Textures/Crosshair"));
        textureDic.Add("leaves", Content.Load<Texture2D>("Textures/Leaves"));
        textureDic.Add("wood", Content.Load<Texture2D>("textures/Wood"));
        
        atlas = new Atlas(GraphicsDevice, new Vector2(2048, 2048), textureDic);
        atlas.Save();
        BlocTextureCoord.SetTextureCoords(atlas);
        chunkMeshBuilder = new ChunkRenderer(GraphicsDevice);
        orientationGraph = new OrientationGraph(GraphicsDevice);
        


        
        
        graphics.PreferredBackBufferWidth = 1920 / 2;
        graphics.PreferredBackBufferHeight = 1080 / 2;
        graphics.ApplyChanges();
        // cubeRenderer = new CubeRenderer(GraphicsDevice, effect, camera);
        // faceRenderer = new FaceRenderer(GraphicsDevice, effectbis, player.Camera, 16f / 2048f, 50000);
        // faceRenderer.SetAtlasTexture(atlas.Texture);
        

        renderer = new Renderer(GraphicsDevice, vertexEffect);
        renderer.SetAtlasTexture(atlas.Texture);
        // CreateTestCubes();
        // CreateTestChests();
        CreateFaces();
        base.Initialize();
    }
    
    private void CreateTestChests()
    {
        BlocModel model = BlocModel.LoadModel(Content, "Chest", GraphicsDevice);
        model.PrintModel();
        
        Texture2D texture = Content.Load<Texture2D>("Textures/Chest");
        
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

        
        // cubeRenderer.UpdateComplexInstances(cubes, model);
        
    }
    
    private void CreateFaces()
    {
        List<FaceData> faces;
        
        faces = world.GetVisibleFaces(atlas);
        Console.WriteLine($"Vertexes: {faces.Count}");
        Console.WriteLine($"Faces: {faces.Count / 4}");
        // faceRenderer.UpdateInstances(faces);
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Input.IsKeyPressed(Keys.H))
        {
            Vector3 pos = player.Position;
            world.SetBloc(0,60,0, 3);
            Bloc bloc = world.GetBloc(Vector3.Zero);
            if (bloc != null)
                Console.WriteLine("Min: "+ bloc.getCollider().Min +" Max: " + bloc.getCollider().Max);
            else
                Console.WriteLine("Bloc is null");
            
            // world.SetBloc((int)pos.X, (int)(pos.Y - 1.8), (int)pos.Z, 3);
        }
        
        world.GenerateChunks(world.GetChunkPosition(player.Position), 4);
        world.CreateChunksBuffers(player.Position, 4);
        
        player.Update(gameTime, world);
        HandleInput(gameTime);
        
        debugTool.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        base.Update(gameTime);
    }
    
    private void HandleInput(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        
        // cubeRenderer.UpdateViewProjection(camera);
        // faceRenderer.UpdateViewProjection(camera);
        renderer.UpdateViewProjection(player.Camera);
        chunkMeshBuilder.UpdateViewProjection(player.Camera);
        if (Input.IsKeyDown(Keys.LeftControl) && Input.IsKeyPressed(Keys.H))
        {
            displayChunkBorder = !displayChunkBorder;
            Console.WriteLine("Display chunk border: " + displayChunkBorder);
        }
        
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
        renderer.DrawWorld(world, world.GetChunkPosition(player.Position), 16);
        player.Draw();
        Vector3 camRotation = player.Camera.Rotation;
        orientationGraph.DrawOrientationGraph(player.Camera);
        if (displayChunkBorder)
            chunkMeshBuilder.DrawChunkBorderGrid(world.GetChunkPosition(player.Position) * 16, (int)player.Position.Y);
        

        
        spriteBatch.Begin();
        debugTool.Draw(spriteBatch);
        
        spriteBatch.Draw(
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
        spriteBatch.End();
        base.Draw(gameTime);
    }
}