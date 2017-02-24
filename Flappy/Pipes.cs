using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flappy
{
    public class Pipes
    {
        public Rectangle rectTop;
        public Rectangle rectBot;
        public Rectangle wolneMiejsce;


        public Pipes(Rectangle top, Rectangle bot)
        {
            this.rectBot = bot;
            this.rectTop = top;
        }
    }
}
