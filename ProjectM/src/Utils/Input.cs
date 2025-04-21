using Microsoft.Xna.Framework.Input;

namespace App1.Utils;

public static class Input
{
    private static KeyboardState currentKeyboardState;
    private static KeyboardState previousKeyboardState;

    private static MouseState currentMouseState;
    private static MouseState previousMouseState;

    static Input()
    {
        currentKeyboardState = Keyboard.GetState();
        previousKeyboardState = currentKeyboardState;

        currentMouseState = Mouse.GetState();
        previousMouseState = currentMouseState;
    }

    public static void Update()
    {
        previousKeyboardState = currentKeyboardState;
        currentKeyboardState = Keyboard.GetState();

        previousMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
    }

    public static bool IsKeyDown(Keys key)
    {
        return currentKeyboardState.IsKeyDown(key);
    }

    public static bool IsKeyUp(Keys key)
    {
        return currentKeyboardState.IsKeyUp(key);
    }

    public static bool IsKeyPressed(Keys key)
    {
        return currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
    }
}