using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Yuuki2TheGame.Graphics
{

    abstract class Painter
    {
        /// <summary>
        /// The GraphicsDevice that we are drawing on.
        /// </summary>
        protected readonly GraphicsDevice GraphicsDevice;

        /// <summary>
        /// The default font to use for drawing text. This is not guarenteed to be set, so check
        /// before using.
        /// </summary>
        protected SpriteFont DefaultFont;

        /// <summary>
        /// Creates a new Painter instance.
        /// </summary>
        /// <param name="graphics">The graphics device that the Painter will be drawing on.</param>
        public Painter(GraphicsDevice graphics)
        {
            GraphicsDevice = graphics;
        }

        /// <summary>
        /// Sets the default font to use for this Painter. May be required by derived classes.
        /// </summary>
        /// <param name="font">The font to set as the default.</param>
        public void SetDefaultFont(SpriteFont font)
        {
            this.DefaultFont = font;
        }
        
        /// <summary>
        /// Queries for required resources and services. Execute this before any other method
        /// of Painter.
        /// </summary>
        public void Initialize()
        {
            Init();
        }

        /// <summary>
        /// Loads all graphical resources required by this Painter.
        /// </summary>
        /// <param name="content">The content manager to use for loading content.</param>
        public void LoadContent(ContentManager content)
        {
            Load(content);
        }

        /// <summary>
        /// Unloads content not handled by the content manager.
        /// </summary>
        /// <param name="content">The content manager for the game.</param>
        public void UnloadContent(ContentManager content)
        {
            Unload(content);
        }
        
        /// <summary>
        /// Draws the content of this Painter to the given SpriteBatch. This method will never call
        /// batch.End() and expects batch.Start() to already have been called.
        /// </summary>
        /// <param name="gameTime">A snapshot of the current time values.</param>
        /// <param name="batch">The SpriteBatch to do drawing on.</param>
        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            Paint(gameTime, batch);
        }

        /// <summary>
        /// Creates a white, opaque, rectangular texture that is of the given size.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected Texture2D CreateRectTexture(int width, int height)
        {
            Texture2D tex = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new Color(255, 255, 255, 255);
            }
            tex.SetData(data);
            return tex;
        }

        /// <summary>
        /// Called by Initialize(). Override to provide behavior that must execute before
        /// content is loaded.
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// Called by LoadContent(). Override to load graphical assets with the ContentManager.
        /// </summary>
        /// <param name="content">The content manager to use for loading.</param>
        protected abstract void Load(ContentManager content);

        /// <summary>
        /// Called by UnloadContent(). Override to dispose of graphical assets.
        /// </summary>
        /// <param name="content">The content manager to use for unloading.</param>
        protected abstract void Unload(ContentManager content);
        
        /// <summary>
        /// Override to do the actual drawing to the screen. Do not call Begin() or End()
        /// on the provided SpriteBatch.
        /// </summary>
        /// <param name="gameTime">A snapshot of the current time.</param>
        /// <param name="batch">The SpriteBatch to use for drawing.</param>
        protected abstract void Paint(GameTime gameTime, SpriteBatch batch);
    }
}
