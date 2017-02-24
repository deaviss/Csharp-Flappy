using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flappy
{
    public class Bird : Game
    {
        public Vector2 pos;
        public Rectangle rect;
        public float velocity;
        public float gravity = 0.4f;

        KeyboardState kb;
        KeyboardState pkb;

        public bool gra;

        public Bird()
        {
            rect = new Rectangle(50, 300, 32, 32);
            pos = new Vector2(50, 300);
            
        }

        public void Update()
        {
            if (gra)
            {
                kb = Keyboard.GetState();
                rect.X = (int)pos.X;
                rect.Y = (int)pos.Y;
                pos.Y += velocity;


                if (pos.Y < 580 && velocity < 6)
                    velocity += gravity;
                else if (pos.Y > 580)
                {
                    velocity = 0;
                    pos.Y = 580;
                }
            }
            if (kb.IsKeyDown(Keys.Space) && pkb.IsKeyUp(Keys.Space))
            {
                if (gra)
                    velocity = -15 * gravity;
            }
            pkb = kb;
        }
    }
}
