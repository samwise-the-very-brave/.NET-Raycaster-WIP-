using System.Collections.Generic;
using System.Windows.Forms;

namespace DDA
{
    static class Input
    {
        static HashSet<Keys> keyPressed = new HashSet<Keys>();
        static HashSet<MouseButtons> pressedMouseButtons = new HashSet<MouseButtons>();

        public static void KeyDown(Keys keyCode) => keyPressed.Add(keyCode);
        public static void KeyUp(Keys keyCode) => keyPressed.Remove(keyCode);
        public static bool IsKeyPressed(Keys keyCode) => keyPressed.Contains(keyCode);
        public static void MouseDown(MouseButtons button) => pressedMouseButtons.Add(button);
        public static void MouseUp(MouseButtons button) => pressedMouseButtons.Remove(button);
        public static bool IsMousePressed(MouseButtons button) => pressedMouseButtons.Contains(button);
    }
}
