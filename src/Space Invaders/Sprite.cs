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

namespace Space_Invaders
{
    abstract class Sprite
    {
        // Sprite sheet variables
        private Texture2D spriteImage;
        private float scale = 1;
        protected Point frameSize;
        private Point currentFrame;
        private Point sheetSize;

        // Frame speed variables
        private int timeSinceLastFrame = 0;
        private int millisecondsPerFrame;

        // Sprite move variables
        protected Vector2 spriteSpeed;
        protected Vector2 _spritePosition;
        protected Vector2 _firePosition;
        private Boolean _movingFoward;
        private Boolean _spriteActive;

        // Get/Set for the sprite position variable
        public Vector2 spritePosition
        {
            get { return _spritePosition; }
            set { _spritePosition = value; }
        }

        // Get/Set for the sprite fire position variable
        public Vector2 firePosition
        {
            get { return _firePosition; }
            set { _firePosition = value; }
        }

        // Get/Set for the sprite active variable
        public Boolean spriteActive
        {
            get { return _spriteActive; }
            set { _spriteActive = value; }
        }

        // Get/Set for the sprite active variable
        public Boolean movingFoward
        {
            get { return _movingFoward; }
            set { _movingFoward = value; }
        }

        // Sprite constructor
        // This can be used to constructor an animated sprite
        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            Point currentFrame, Point sheetSize, Vector2 speed,
            int millisecondsPerFrame, float scale, Boolean spriteActive)
        {
            this.spriteImage = textureImage;
            this.spritePosition = position;
            this.frameSize = frameSize;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.spriteSpeed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
            this.scale = scale;
            this.spriteActive = spriteActive;
        }

        // Sprite constructor with movingFoward and firePosition variables (Used for constructing enemy sprites)
        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            Point currentFrame, Point sheetSize, Vector2 speed,
            int millisecondsPerFrame, float scale, Boolean spriteActive, Boolean movingFoward, Vector2 firePosition)
        {
            this.spriteImage = textureImage;
            this._spritePosition = position;
            this.frameSize = frameSize;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.spriteSpeed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
            this.scale = scale;
            this._spriteActive = spriteActive;
            this._movingFoward = movingFoward;
            this._firePosition = firePosition;
        }

        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Animates the sprite sheet
            // Uses the millisecondsPerFrame variable to determine the speed of the animation
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                currentFrame.X++;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    currentFrame.Y++;
                    if (currentFrame.Y >= sheetSize.Y)
                        currentFrame.Y = 0;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draws the sprite
            spriteBatch.Draw(spriteImage, spritePosition, new Rectangle(currentFrame.X * frameSize.X,
                currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0, Vector2.Zero,
                scale, SpriteEffects.None, 0);
        }

        // This returns a rectangle that is used to determine if a sprite has collided with another object
        public Rectangle collisionRect
        {
            get
            {
                // Rectangle based on the sprites current position and its frame size
                if (scale == 1)
                {
                    return new Rectangle((int)spritePosition.X, (int)spritePosition.Y, frameSize.X, frameSize.Y);
                }
                // Rectangle for any sprites that are using a scale of 2 (Just the player sprite)
                else
                {
                    return new Rectangle((int)spritePosition.X, (int)spritePosition.Y, frameSize.X * 2, frameSize.Y * 2);
                }
            }
        }
    }
}