using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Space_Invaders
{
    class PlayerSprite : Sprite
    {
        // Player lives
        private int _playerLives = 3;

        public int playerLives
        {
            get { return _playerLives; }
            set { _playerLives = value; }
        }

        // Player sprite constructors
        public PlayerSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, Point currentFrame, Point sheetSize,
            Vector2 speed, int millisecondsPerFrame, float scale, Boolean spriteActive)
            : base(textureImage, position, frameSize, currentFrame,
            sheetSize, speed, millisecondsPerFrame, scale, spriteActive)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Enables the player sprite to move left and right using the touch screen display
            TouchCollection touchcollection = TouchPanel.GetState();
            foreach (TouchLocation t1 in touchcollection)
            {
                if (t1.State == TouchLocationState.Moved)
                {
                    Vector2 touchposition = t1.Position;
                    if (touchposition.X >= (spritePosition.X + 66) && touchposition.Y >= 630)
                        spritePosition = new Vector2(spritePosition.X + 3, spritePosition.Y);
                    else if (touchposition.X <= spritePosition.X && touchposition.Y >= 630)
                        spritePosition = new Vector2(spritePosition.X - 3, spritePosition.Y);
                    else
                        spritePosition = spritePosition;
                }
            }

            // Enables the player sprite to move left and right using the controller thumbsticks
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            if (gamepadState.ThumbSticks.Left.X != 0)
                spritePosition = new Vector2(spritePosition.X + gamepadState.ThumbSticks.Left.X * 3, spritePosition.Y);

            // Keeps the player sprite within the bounds of the screen
            // Left side
            if (spritePosition.X < 0)
                spritePosition = new Vector2(0, spritePosition.Y);

            // Right side, (The frame size is *2 because the player sprite has been scaled twice as big)
            if (spritePosition.X > 1200 - frameSize.X * 2)
                spritePosition = new Vector2(1200 - frameSize.X * 2, spritePosition.Y);

            base.Update(gameTime, clientBounds);
        }
    }
}