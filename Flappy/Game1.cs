using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Flappy
{
   
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch sb;

        Texture2D birdT;
        Texture2D pipaT;
        Texture2D przyciskT;

        MouseState ms;
        MouseState pms;

        Rectangle cursor;
        Rectangle przyciskRect;

        bool odliczanie,odejmij,dodaj = true, nowyWynik;
        float czas = 60;
        float speed = 2;

        SpriteFont font;

        Texture2D bg;

        int points;

        List<Pipes> rury = new List<Pipes>();

        float timer = 5;
        int _szerokosc = 140;

        Color kolor,kolor2;

        bool gra;

        Random random = new Random();
        int max_points;
        int cos;


        enum State
        {
            START,
            GAME,
            G_OVER
        }

        State state;

        Bird bird = new Bird();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        
        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 480;
            graphics.ApplyChanges();
            

            base.Initialize();
        }

        
        protected override void LoadContent()
        {
            
            sb = new SpriteBatch(GraphicsDevice);

            birdT = Content.Load<Texture2D>("bullet");
            pipaT = Content.Load<Texture2D>("player");
            font = Content.Load<SpriteFont>("font");
            cos = graphics.PreferredBackBufferHeight / 2;

            bg = Content.Load<Texture2D>("bg");
            przyciskT = Content.Load<Texture2D>("przycisk");
            przyciskRect = new Rectangle(graphics.PreferredBackBufferWidth - przyciskT.Width - (przyciskT.Width/6), graphics.PreferredBackBufferHeight / 2 - przyciskT.Width/6, przyciskT.Width, przyciskT.Height);
            state = State.START;
            cursor = new Rectangle(0, 0, 1, 1);
            IsMouseVisible = true;

            kolor = new Color(200,200,200,255);
            kolor2 = new Color(0, 0, 0, 255);

            loadPoints();
        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            ms = Mouse.GetState();
            speed += 1.4f;

            cursor.X = (int)ms.X;
            cursor.Y = (int)ms.Y;

            switch (state)
            {
                case State.START:
                    if (cursor.Intersects(przyciskRect) && LPM() && !dodaj)
                    {
                        bird.gra = true;
                        odejmij = true;
                        nowyWynik = false;
                        loadPoints();
                    }
                    if(dodaj && !odejmij)
                    {
                        kolor.A += 3;
                        kolor2.A += 3;
                        if (kolor.A >= 222)
                            dodaj = false;
                    }
                    else if(odejmij && !dodaj)
                    {
                        kolor.A -= 6;
                        kolor2.A -= 6;
                        if (kolor.A <= 5)
                        {
                            state = State.GAME;
                            odejmij = false;
                        }
                            
                    }
                    break;

                case State.GAME:

                    if (bird.gra)
                    {
                        timer--;
                        if (timer < 0)
                        {
                            timer = 120;
                            var pipe = new Pipes(new Rectangle(560, 0, 32, random.Next(0, 500)), new Rectangle(560, 600 - random.Next(0, cos), 32, 700));
                            rury.Add(pipe);
                        }

                        rury.ForEach(p =>
                        {

                            p.rectTop.X -= 2;
                            p.rectBot.X -= 2;


                            p.wolneMiejsce = new Rectangle(p.rectBot.X + 16, p.rectTop.Y + p.rectTop.Height - 16, 1, p.rectTop.Y + p.rectTop.Height + _szerokosc);

                            p.rectBot.Y = p.rectTop.Y + p.rectTop.Height + _szerokosc;

                            if (bird.rect.Intersects(p.rectBot) || bird.rect.Intersects(p.rectTop))
                            {
                                if (points > max_points)
                                {
                                    savePoints();
                                    nowyWynik = true;
                                }
                                bird.gra = false;
                                
                                state = State.G_OVER;

                            }
                            else if (bird.rect.Intersects(p.wolneMiejsce))
                            {
                                if (!odliczanie)
                                    points++;
                                odliczanie = true;
                            }

                        });


                        if (odliczanie)
                        {
                            czas--;
                            if (czas <= 0)
                            {
                                czas = 60;
                                odliczanie = false;
                            }
                        }
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                        _szerokosc--;
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                        _szerokosc++;
                    bird.Update();
                    break;

                case State.G_OVER:
                    if(cursor.Intersects(przyciskRect) && LPM())
                    {
                        resetGame();
                        
                    }
                    break;

            }

            if (points > max_points)
            {
                nowyWynik = true;
                savePoints();
            }

            pms = ms;

            base.Update(gameTime);
        }

        public bool LPM()
        {
            if (ms.LeftButton == ButtonState.Pressed && pms.LeftButton == ButtonState.Released)
                return true;
            else
                return false;
        }

        public void loadPoints()
        {
            max_points = Properties.Settings.Default.max_score;
            Properties.Settings.Default.Save();
        }
        public void savePoints()
        {
            Properties.Settings.Default.max_score = points;
            Properties.Settings.Default.Save();
        }
        public void resetGame()
        {
            rury.ForEach(x =>
            {
                rury.Remove(x);
            });
            bird.pos.Y = 300;
            bird.gra = true;
            bird.velocity = 0;
            points = 0;
            state = State.GAME;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, null, null);
            sb.Draw(bg, new Vector2(1, 1), new Rectangle((int)speed, 0, 600, 800), Color.White);
            switch (state)
            {
                case State.START:
                    sb.Draw(przyciskT, przyciskRect, kolor);
                    sb.DrawString(font, "Rozpocznij gre klikajac przycisk",new Vector2(przyciskRect.X + 66, przyciskRect.Y + 66), kolor2);
                    break;

                case State.GAME:
                    foreach (var p in rury)
                    {
                        sb.Draw(pipaT, p.rectTop, Color.White);
                        sb.Draw(pipaT, p.rectBot, Color.White);
                    }

                    sb.Draw(birdT, bird.rect, Color.White);
                    sb.DrawString(font, string.Format("Points: {0}", points), new Vector2(0, 0), Color.Black);
                    break;

                case State.G_OVER:
                    sb.Draw(przyciskT, przyciskRect, Color.White);
                    if(nowyWynik)
                        sb.DrawString(font, string.Format("Pobito rekord! {0}", points), new Vector2(przyciskRect.X + 66, przyciskRect.Y + 88), Color.Black);
                    sb.DrawString(font, string.Format("Przegrales z wynikiem {0}\nNajlepszy wynik {1}", points, max_points), new Vector2(przyciskRect.X + 66, przyciskRect.Y + 33), Color.Black);
                    break;

            }



            
            
            sb.End();
            

            base.Draw(gameTime);
        }
    }
}
