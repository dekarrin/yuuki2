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
        public GraphicsDevice GraphicsDevice { protected get; set; }

        /// <summary>
        /// The default font to use for drawing text. This is not guarenteed to be set, so check
        /// before using.
        /// </summary>
        public SpriteFont DefaultFont { protected get; set; }

        public Texture2D DefaultTexture { protected get; set; }

        private IDictionary<string, Texture2D> textureCache;

        /// <summary>
        /// Creates a new Painter instance.
        /// </summary>
        public Painter()
        {
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
            textureCache = Load(content);
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

        protected void ConvertIDs(IList<Sprite> sprites)
        {
            foreach (Sprite spr in sprites)
            {
                //TODO: use preloaded graphics
                spr.Texture = IDToTexture(spr.TextureID);
            }
        }

        /// <summary>
        /// Attempts to convert the resource name into a texture. If resource name is null, default texture is used.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Texture2D IDToTexture(string name)
        {
            if (name != null && textureCache.ContainsKey(name))
            {
                return textureCache[name];
            }
            else
            {
                return DefaultTexture;
            }
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
        /// <returns>A dictionary mapping names to Texture2D resources.</returns>
        protected abstract IDictionary<string, Texture2D> Load(ContentManager content);

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
