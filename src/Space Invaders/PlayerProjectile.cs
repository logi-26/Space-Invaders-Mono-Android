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
    class PlayerProjectile : Sprite
    {
        // Player sprite constructors
        public PlayerProjectile(Texture2D textureImage, Vector2 position,
            Point frameSize, Point currentFrame, Point sheetSize,
            Vector2 speed, int millisecondsPerFrame, float scale, Boolean spriteActive)
            : base(textureImage, position, frameSize, currentFrame,
            sheetSize, speed, millisecondsPerFrame, scale, spriteActive)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Moves the player projectile vertically
            spritePosition -= new Vector2(0, 10);

            base.Update(gameTime, clientBounds);
        }
    }
}