/************************
 * Samuel Goulet
 * Novembre 2016
 * Structure KeyWrapper, projet final Comm
 ***********************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeu_Comm
{
    /// <summary>
    /// États possibles du clavier et de la souris
    /// </summary>
    [Flags]
    public enum KeyState
    {
        None = 0,
        Up = 1,
        Left = 2,
        Down = 4,
        Right = 8,
        LeftMouse = 16,
        RightMouse = 32,
        Space = 64
    };

    /// <summary>
    /// Structure contenant les états du clavier et de la souris
    /// </summary>
    public struct KeyWrapper
    {
        public KeyState State;
        public int MouseX;
        public int MouseY;
        public KeyWrapper(KeyState State, int X, int Y)
        {
            this.State = State;
            MouseX = X;
            MouseY = Y;
        }
    }
}
