using App1.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace App1.Core.Player;

public class Spectator
{
    public float moveSpeed = 25f;
    public Vector3 position = new Vector3(0, 0, 0);
    public Vector3 rotation = new Vector3(0, 0, 0);
    
    private int previousScrollValue;


    public readonly Camera Camera;
    
    private Vector2 lastMousePosition = new Vector2(0, 0);
    
    
    
    public Spectator(GraphicsDevice graphicsDevice)
    {
        Camera = new Camera(graphicsDevice);
        
        Camera.Position = position;
        lastMousePosition = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
        Mouse.SetPosition((int)lastMousePosition.X, (int)lastMousePosition.Y);
    }
    
    public void Update(GameTime gameTime)
    {
        HandleInput(gameTime);   
    }

    private void HandleInput(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        float rotationSpeed = 0.4f;
        var mouseState = Mouse.GetState();
        
        Vector3 move = new Vector3(0, 0, 0);


        moveSpeed += mouseState.ScrollWheelValue - previousScrollValue;
        previousScrollValue = mouseState.ScrollWheelValue;
        
        
        
        
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
        
        move *= (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        Vector3 forwardNoPitch = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(Camera.Rotation.Y));
        forwardNoPitch = Vector3.Normalize(forwardNoPitch);

        position += new Vector3(
            forwardNoPitch.X * move.Z + forwardNoPitch.Z * move.X,
            move.Y,
            forwardNoPitch.Z * move.Z - forwardNoPitch.X * move.X
        );
        
        Camera.SetPosition(position);
        
        Vector2 mouseDelta = new Vector2(mouseState.X, mouseState.Y) - lastMousePosition;
        if ( mouseDelta != Vector2.Zero)
            Camera.Rotate(new Vector3(-mouseDelta.Y * rotationSpeed, -mouseDelta.X * rotationSpeed, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds);
        
        // lastMousePosition = new Vector2(mouseState.X, mouseState.Y);
        Mouse.SetPosition((int)lastMousePosition.X, (int)lastMousePosition.Y);
    }
    
    public Vector3 GetPosition()
    {
        return position;
    }
}