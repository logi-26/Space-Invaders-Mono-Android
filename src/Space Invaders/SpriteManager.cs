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
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        // Start background colour (Light Blue)
        Color startBackgroundColour = new Color(0, 128, 218);

        // Timer variables
        private int counter = 1;
        private float countDuration = 1f;
        private float currentTime = 0f;

        // Score variable
        private int playerScore = 0;
        private int finalScore = 0;
        private Boolean finalScoreCalculated = false;

        // Random number
        private Random rnd = new Random();

        // Font sprites
        private SpriteFont gameFont;
        private SpriteFont timerFont;

        // Background textures
        private Texture2D startBackground;
        private Texture2D endBackground;
        private Texture2D wonBackground;

        // Barrier sprites
        private BackgroundSprite barrierFirst;
        private BackgroundSprite barrierSecond;
        private BackgroundSprite barrierThird;
        private int barrierFirstHits = 0;
        private int barrierSecondHits = 0;
        private int barrierThirdHits = 0;

        // Player sprite
        private PlayerSprite player;

        // Player projectile sprite
        private PlayerProjectile playerProjectile;

        // 2D array of enemy sprites
        private Sprite[,] enemySpriteArray = new Sprite[3, 8];

        // 2D array of enemy projectile sprites
        private EnemyProjectile[,] enemyProjectileArray = new EnemyProjectile[3, 8];

        public SpriteManager(Game game)
            : base(game)
        {
        }

        //Game states
        enum GameState { GameStart, GamePlay, GameOver, GameWon };
        GameState currentGameState = GameState.GameStart;

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            // Loads the fonts
            gameFont = Game.Content.Load<SpriteFont>(@"Font/gameFont");
            timerFont = Game.Content.Load<SpriteFont>(@"Font/timerFont");
            
            // Loads the background sprites
            startBackground = Game.Content.Load<Texture2D>(@"Backgrounds/Start_Screen_1");
            endBackground = Game.Content.Load<Texture2D>(@"Backgrounds/GameEnd_Screen");
            wonBackground = Game.Content.Load<Texture2D>(@"Backgrounds/GameWon_Screen");

            // Loads the barrier 1 sprite
            barrierFirst = new BackgroundSprite(
                Game.Content.Load<Texture2D>(@"Backgrounds/Barrier"),
                new Vector2(100, 580), new Point(129, 38), new Point(0, 0),
                new Point(2, 1), new Vector2(1, 0), 400, 1, true);

            // Loads the barrier 2 sprite
            barrierSecond = new BackgroundSprite(
                Game.Content.Load<Texture2D>(@"Backgrounds/Barrier"),
                new Vector2(540, 580), new Point(129, 38), new Point(0, 0),
                new Point(2, 1), new Vector2(1, 0), 400, 1, true);

            // Loads the barrier 3 sprite
            barrierThird = new BackgroundSprite(
                Game.Content.Load<Texture2D>(@"Backgrounds/Barrier"),
                new Vector2(970, 580), new Point(129, 38), new Point(0, 0),
                new Point(2, 1), new Vector2(1, 0), 400, 1, true);

            // Loads the player sprite
            player = new PlayerSprite(
                Game.Content.Load<Texture2D>(@"Player/Spaceship_Centre"),
                new Vector2(570, 650), new Point(39, 43), new Point(0, 0),
                new Point(2, 1), new Vector2(4, 0), 30, 2, true);

            // Loads the player projectile sprite
            playerProjectile = new PlayerProjectile(
                Game.Content.Load<Texture2D>(@"Player/Spaceship_Projectile"),
                new Vector2(0, 0), new Point(4, 14), new Point(0, 0),
                new Point(2, 1), new Vector2(4, 0), 30, 2, false);

            // Loads the enemy sprites into the 2D array
            Vector2 enemyLocation;
            Point frameSize;
            Texture2D spriteImage;

            for (int i = 0; i <= 2; i++)
            {
                // Loads the row 1 enemy sprites
                if (i == 0)
                {
                    enemyLocation = new Vector2(50, 100);
                    frameSize = new Point(48, 48);
                    spriteImage = Game.Content.Load<Texture2D>(@"Enemies/Alien_3");
                }
                // Loads the row 2 enemy sprites
                else if (i == 1)
                {
                    enemyLocation = new Vector2(40, 170);
                    frameSize = new Point(66, 48);
                    spriteImage = Game.Content.Load<Texture2D>(@"Enemies/Alien_1");
                }
                else
                // Loads the row 3 enemy sprites
                {
                    enemyLocation = new Vector2(34, 240);
                    frameSize = new Point(72, 48);
                    spriteImage = Game.Content.Load<Texture2D>(@"Enemies/Alien_2");
                }
                for (int j = 0; j <= 7; j++)
                {
                    // Generates a new random number 
                    int randomX = rnd.Next(1, 1024);

                    enemySpriteArray[i, j] = new EnemySprite(spriteImage, enemyLocation, frameSize, new Point(0, 0),
                    new Point(2, 1), new Vector2(2, 0), 400, 1, true, true, new Vector2(randomX, enemyLocation.Y));

                    enemyLocation += new Vector2(100, 0);
                }
            }

            // Loads the enemy projectiles into a 2D array
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    enemyProjectileArray[i, j] = new EnemyProjectile(
                        Game.Content.Load<Texture2D>(@"Player/Spaceship_Projectile"),
                        new Vector2(-10, 0), new Point(4, 14), new Point(0, 0),
                        new Point(2, 1), new Vector2(4, 0), 30, 2, false);
                }
            }
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // Update depending on current gamestate
            switch (currentGameState)
            {
                case GameState.GameStart:
                    // Switches the start screen texture every second
                    if (counter % 2 == 0)
                    {
                        startBackground = Game.Content.Load<Texture2D>("Backgrounds/Start_Screen_1");
                    }
                    else
                    {
                        startBackground = Game.Content.Load<Texture2D>("Backgrounds/Start_Screen_2");
                    }

                    // If the screen is tapped, the game will begin
                    foreach (TouchLocation l in TouchPanel.GetState())
                    {
                        if (l.State == TouchLocationState.Pressed)
                        {
                            currentGameState = GameState.GamePlay;
                            NewGame();
                            counter = 1;
                            countDuration = 1f;
                            currentTime = 0f;
                            finalScoreCalculated = false;
                        }
                    }
                    break;
                case GameState.GamePlay:
                    // Updates the player sprite
                    player.Update(gameTime, Game.Window.ClientBounds);

                    // Updates the player projectile
                    if (playerProjectile.spriteActive != false)
                        playerProjectile.Update(gameTime, Game.Window.ClientBounds);

                    // If the screen is tapped, a projectile is created
                    TouchCollection touchcollection = TouchPanel.GetState();
                    foreach (TouchLocation t1 in touchcollection)
                    {
                        if (t1.State == TouchLocationState.Moved)
                        {
                            Vector2 touchposition = t1.Position;
                            if (touchposition.Y <= (player.spritePosition.Y - 40) && touchposition.Y >= 70 && playerProjectile.spriteActive != true)
                            {
                                playerProjectile = new PlayerProjectile(
                                Game.Content.Load<Texture2D>(@"Player/Spaceship_Projectile"),
                                player.spritePosition + new Vector2(35, -30), new Point(4, 14), new Point(0, 0),
                                new Point(2, 1), new Vector2(4, 0), 30, 2, true);

                                playerProjectile.spriteActive = true;
                            }
                        }
                    }

                    // If the projectile reaches the upper screen bounds it is made inactive
                    if (playerProjectile.spritePosition.Y < 60)
                        playerProjectile.spriteActive = false;

                    // Updates all of the enemy sprites in the array
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 7; j++)
                        {
                            enemySpriteArray[i, j].Update(gameTime, Game.Window.ClientBounds);

                            // If any of the enemy sprites reach the right bounds of the screen
                            if (enemySpriteArray[i, j].spriteActive != false && enemySpriteArray[i, j].spritePosition.X == 1200 - 66)
                            {
                                // loops through the entire enemy array
                                for (int a = 0; a <= 2; a++)
                                {
                                    for (int b = 0; b <= 7; b++)
                                    {
                                        // All of the sprites in the array have their position Y incremented                                  
                                        enemySpriteArray[a, b].spritePosition += new Vector2(0, 50);

                                        // The moving foward variable is set false
                                        enemySpriteArray[a, b].movingFoward = false;

                                        // Generates a new random number 
                                        int randomX = rnd.Next(1, 1024);

                                        // Sets the random number as the enemy sprites firePosition X 
                                        // This is used to give each enemy sprite a new random fire position 
                                        enemySpriteArray[a, b].firePosition = new Vector2(randomX, enemySpriteArray[a, b].spritePosition.Y);
                                    }
                                }
                            }
                            // If any of the enemy sprites reach the left bounds of the screen
                            else if (enemySpriteArray[i, j].spriteActive != false && enemySpriteArray[i, j].spritePosition.X < 0)
                            {
                                // loops through the entire enemy array
                                for (int a = 0; a <= 2; a++)
                                {
                                    for (int b = 0; b <= 7; b++)
                                    {
                                        // All of the sprites in the array have their position Y incremented
                                        enemySpriteArray[a, b].spritePosition += new Vector2(0, 50);

                                        // The moving foward variable is set true
                                        enemySpriteArray[a, b].movingFoward = true;

                                        // Generates a new random number 
                                        int randomX = rnd.Next(1, 1024);

                                        // Sets the random number as the enemy sprites firePosition X 
                                        // This is used to give each enemy sprite a new random fire position 
                                        enemySpriteArray[a, b].firePosition = new Vector2(randomX, enemySpriteArray[a, b].spritePosition.Y);
                                    }
                                }
                            }
                        }
                    }

                    // Loops through the enemy sprite array
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 7; j++)
                        {
                            // If the enemy sprites position is equal to the enemy sprites random fire posistion
                            if (enemySpriteArray[i, j].spritePosition == enemySpriteArray[i, j].firePosition && enemySpriteArray[i, j].spriteActive != false)
                            {
                                // A new enemy projectile is created
                                enemyProjectileArray[i, j] = new EnemyProjectile(
                                Game.Content.Load<Texture2D>(@"Enemies/Alien_Projectile"),
                                enemySpriteArray[i, j].spritePosition, new Point(4, 14), new Point(0, 0),
                                new Point(2, 1), new Vector2(1, 0), 30, 2, true);

                                enemyProjectileArray[i, j].spriteActive = true;
                            }

                            // If the projectile is active, it is updated
                            if (enemyProjectileArray[i, j].spriteActive != false)
                                enemyProjectileArray[i, j].Update(gameTime, Game.Window.ClientBounds);

                            // Once the projectile goes out of the lower screen bounds, the sprite is made in-active
                            if (enemyProjectileArray[i, j].spritePosition.Y > 768)
                                enemyProjectileArray[i, j].spriteActive = false;
                        }
                    }

                    // Checks for collisions between the enemy sprites, the player sprite, the barriers or the projectile sprites
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 7; j++)
                        {
                            // If the enemy sprites collide with the player or barrier sprites, the game is over
                            if (enemySpriteArray[i, j].collisionRect.Intersects(player.collisionRect) && enemySpriteArray[i, j].spriteActive != false ||
                                enemySpriteArray[i, j].collisionRect.Intersects(barrierFirst.collisionRect) &&
                                enemySpriteArray[i, j].spriteActive != false && barrierFirst.spriteActive != false ||
                                enemySpriteArray[i, j].collisionRect.Intersects(barrierSecond.collisionRect) &&
                                enemySpriteArray[i, j].spriteActive != false && barrierSecond.spriteActive != false ||
                                enemySpriteArray[i, j].collisionRect.Intersects(barrierThird.collisionRect) &&
                                enemySpriteArray[i, j].spriteActive != false && barrierThird.spriteActive != false)
                                currentGameState = GameState.GameOver;

                            // If the players projectile hits an enemy sprite, the enemy sprite is removed and the players score is incremented
                            if (enemySpriteArray[i, j].collisionRect.Intersects(playerProjectile.collisionRect) && playerProjectile.spriteActive != false)
                            {
                                if (enemySpriteArray[i, j].spriteActive != false)
                                {
                                    enemySpriteArray[i, j].spriteActive = false;
                                    playerProjectile.spriteActive = false;
                                    playerScore += 10;
                                }
                            }

                            // If the enemies projectile hits the player sprite, the players lives are decremented
                            if (enemyProjectileArray[i, j].collisionRect.Intersects(player.collisionRect) && enemyProjectileArray[i, j].spriteActive != false)
                            {
                                player.playerLives -= 1;
                                enemyProjectileArray[i, j].spriteActive = false;
                            }
                        }
                    }

                    // Prevents the player from shooting through the barrier sprites
                    if (playerProjectile.collisionRect.Intersects(barrierFirst.collisionRect) && barrierFirst.spriteActive != false ||
                                playerProjectile.collisionRect.Intersects(barrierSecond.collisionRect) && barrierSecond.spriteActive != false ||
                                playerProjectile.collisionRect.Intersects(barrierThird.collisionRect) && barrierThird.spriteActive != false)
                        playerProjectile.spriteActive = false;

                    // Detects if an enemy projectile has collided with a barrier
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 7; j++)
                        {
                            if (enemyProjectileArray[i, j].collisionRect.Intersects(barrierFirst.collisionRect) &&
                                barrierFirst.spriteActive != false && enemyProjectileArray[i, j].spriteActive != false)
                            {
                                barrierFirstHits += 1;
                                enemyProjectileArray[i, j].spriteActive = false;
                            }
                            else if (enemyProjectileArray[i, j].collisionRect.Intersects(barrierSecond.collisionRect) &&
                                barrierSecond.spriteActive != false && enemyProjectileArray[i, j].spriteActive != false)
                            {
                                barrierSecondHits += 1;
                                enemyProjectileArray[i, j].spriteActive = false;
                            }
                            else if (enemyProjectileArray[i, j].collisionRect.Intersects(barrierThird.collisionRect) &&
                                barrierThird.spriteActive != false && enemyProjectileArray[i, j].spriteActive != false)
                            {
                                barrierThirdHits += 1;
                                enemyProjectileArray[i, j].spriteActive = false;
                            }
                        }
                    }

                    // These change the barrier images if the barrier has been hit
                    if (barrierFirstHits == 1)
                    {
                        barrierFirst = new BackgroundSprite(
                        Game.Content.Load<Texture2D>(@"Backgrounds/Barrier_Crack_1"),
                        new Vector2(100, 580), new Point(129, 38), new Point(0, 0),
                        new Point(2, 1), new Vector2(1, 0), 400, 1, true);
                    }
                    else if (barrierFirstHits == 2)
                    {
                        barrierFirst = new BackgroundSprite(
                        Game.Content.Load<Texture2D>(@"Backgrounds/Barrier_Crack_2"),
                        new Vector2(100, 580), new Point(129, 38), new Point(0, 0),
                        new Point(2, 1), new Vector2(1, 0), 400, 1, true);
                    }
                    if (barrierSecondHits == 1)
                    {
                        barrierSecond = new BackgroundSprite(
                        Game.Content.Load<Texture2D>(@"Backgrounds/Barrier_Crack_1"),
                        new Vector2(540, 580), new Point(129, 38), new Point(0, 0),
                        new Point(2, 1), new Vector2(1, 0), 400, 1, true);
                    }
                    else if (barrierSecondHits == 2)
                    {
                        barrierSecond = new BackgroundSprite(
                         Game.Content.Load<Texture2D>(@"Backgrounds/Barrier_Crack_2"),
                         new Vector2(540, 580), new Point(129, 38), new Point(0, 0),
                         new Point(2, 1), new Vector2(1, 0), 400, 1, true);
                    }
                    if (barrierThirdHits == 1)
                    {
                        barrierThird = new BackgroundSprite(
                        Game.Content.Load<Texture2D>(@"Backgrounds/Barrier_Crack_1"),
                        new Vector2(970, 580), new Point(129, 38), new Point(0, 0),
                        new Point(2, 1), new Vector2(1, 0), 400, 1, true);
                    }
                    else if (barrierThirdHits == 2)
                    {
                        barrierThird = new BackgroundSprite(
                        Game.Content.Load<Texture2D>(@"Backgrounds/Barrier_Crack_2"),
                        new Vector2(970, 580), new Point(129, 38), new Point(0, 0),
                        new Point(2, 1), new Vector2(1, 0), 400, 1, true);
                    }

                    // If a barrier has been hit 3 times, it is made in-active
                    if (barrierFirstHits >= 3)
                        barrierFirst.spriteActive = false;
                    if (barrierSecondHits >= 3)
                        barrierSecond.spriteActive = false;
                    if (barrierThirdHits >= 3)
                        barrierThird.spriteActive = false;

                    // When the player has no lives left, the game is over
                    if (player.playerLives == 0)
                        currentGameState = GameState.GameOver;

                    // Variable to hold the number of in-active enemies
                    int wonCheck = 0;

                    // This loops through the enemy array counting the number of in-active sprites
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 7; j++)
                        {
                            if (enemySpriteArray[i, j].spriteActive != true)
                                wonCheck += 1;
                        }
                    }

                    // If all 24 enemy sprites are in-active, the gamestate is set to game won
                    if (wonCheck == 24)
                        currentGameState = GameState.GameWon;
                    break;
                case GameState.GameOver:
                    // Updates the end screen background texture
                    if (counter % 2 == 0)
                    {
                        endBackground = Game.Content.Load<Texture2D>("Backgrounds/GameEnd_Screen");
                    }
                    else
                    {
                        endBackground = Game.Content.Load<Texture2D>("Backgrounds/Blank_Screen");
                    }

                    // If the screen is tapped, the gamestate is set to game start
                    TouchCollection touchcollectionOver = TouchPanel.GetState();
                    foreach (TouchLocation t1 in touchcollectionOver)
                    {
                        if (t1.State == TouchLocationState.Pressed)
                            currentGameState = GameState.GameStart;
                    }
                    break;
                case GameState.GameWon:
                    // Updates the won screen background texture
                    if (counter % 2 == 0)
                    {
                        wonBackground = Game.Content.Load<Texture2D>("Backgrounds/GameWon_Screen");
                    }
                    else
                    {
                        wonBackground = Game.Content.Load<Texture2D>("Backgrounds/Blank_Screen");
                    }

                    // Calculates the final score
                    if (finalScoreCalculated != true)
                        CalculateFinalScore();

                    // If the screen is tapped, the gamestate is set to game start
                    TouchCollection touchcollectionWon = TouchPanel.GetState();
                    foreach (TouchLocation t1 in touchcollectionWon)
                    {
                        if (t1.State == TouchLocationState.Pressed)
                            currentGameState = GameState.GameStart;
                    }
                    break;
            }

            // Updates the game timer
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currentTime >= countDuration)
            {
                counter++;
                currentTime -= countDuration;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Draw depending on current gamestate
            switch (currentGameState)
            {
                case GameState.GameStart:

                    GraphicsDevice.Clear(startBackgroundColour);

                    // Draws the start screen background sprite
                    spriteBatch.Draw(startBackground, new Vector2(0, 0), Color.White);
                    break;
                case GameState.GamePlay:

                    // Draws the barrier sprites if they are currently active
                    if (barrierFirst.spriteActive != false)
                        barrierFirst.Draw(gameTime, spriteBatch);
                    if (barrierSecond.spriteActive != false)
                        barrierSecond.Draw(gameTime, spriteBatch);
                    if (barrierThird.spriteActive != false)
                        barrierThird.Draw(gameTime, spriteBatch);

                    // Draws the player sprite
                    player.Draw(gameTime, spriteBatch);

                    // Draws the player projectile
                    if (playerProjectile.spriteActive != false)
                        playerProjectile.Draw(gameTime, spriteBatch);

                    // Draws the enemy sprites from the 2D array
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 7; j++)
                        {
                            if (enemySpriteArray[i, j].spriteActive != false)
                                enemySpriteArray[i, j].Draw(gameTime, spriteBatch);
                        }
                    }

                    // Draws the enemy sprite projectiles from the 2D array
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 7; j++)
                        {
                            // If the enemy projectile is active, it is draw to the screen
                            if (enemyProjectileArray[i, j].spriteActive != false)
                                enemyProjectileArray[i, j].Draw(gameTime, spriteBatch);
                        }
                    }
                    
                    // Draws the game font
                    spriteBatch.DrawString(gameFont, "Score:", new Vector2(30, 5), Color.White,
                        0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
                    
                    // Draws the player score font
                    spriteBatch.DrawString(gameFont, playerScore.ToString(), new Vector2(125, 5), Color.White,
                        0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

                    // Draws the timer font
                    spriteBatch.DrawString(timerFont, counter.ToString(), new Vector2(600, 10), Color.White,
                        0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

                    // Draws the player lives font
                    spriteBatch.DrawString(gameFont, "Lives: ", new Vector2(1060, 10), Color.White,
                        0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

                    // Draws the player lives number
                    spriteBatch.DrawString(gameFont, player.playerLives.ToString(), new Vector2(1145, 10), Color.White,
                        0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

                    break;
                case GameState.GameOver:
                    // Draws the end screen background sprite
                    spriteBatch.Draw(endBackground, new Vector2(0, 0), Color.White);
                    break;
                case GameState.GameWon:
                    // Draws the end screen background sprite
                    spriteBatch.Draw(wonBackground, new Vector2(0, 0), Color.White);

                    // Draws the game font
                    spriteBatch.DrawString(gameFont, "Your final score is:", new Vector2(450, 500), Color.White,
                        0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

                    // Draws the player score font
                    spriteBatch.DrawString(gameFont, finalScore.ToString(), new Vector2(685, 500), Color.White,
                        0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void NewGame()
        {
            // Resets the values so a new game can begin
            player.playerLives = 3;
            currentTime = 0f;
            counter = 0;
            playerScore = 0;
            finalScore = 0;
            barrierFirstHits = 0;
            barrierSecondHits = 0;
            barrierThirdHits = 0;
            LoadContent();
        }

        public void CalculateFinalScore()
        {
            // This calculates the players final score
            // The final score is determined by the players lives, the time taken to win and the condition of the barrier spries
            finalScore = playerScore;

            // Adds points to the final score depending on the amount of lives the player had remaining
            finalScore += player.playerLives * 100;

            // Deducts the time taken to complete the game from the players final score
            finalScore -= counter;

            // Deducts points from the final score depending on the condition of the 3 barrier sprites
            finalScore -= barrierFirstHits * 10;
            finalScore -= barrierSecondHits * 10;
            finalScore -= barrierThirdHits * 10;

            finalScoreCalculated = true;
        }
    }
}