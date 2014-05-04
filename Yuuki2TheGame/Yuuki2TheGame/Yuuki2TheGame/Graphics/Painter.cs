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
        public static GraphicsDevice GraphicsDevice { protected get; set; }

        /// <summary>
        /// The default font to use for drawing text. This is not guarenteed to be set, so check
        /// before using.
        /// </summary>
        public static SpriteFont DefaultFont { protected get; set; }

        public static Texture2D DefaultTexture { protected get; set; }

        private static IDictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();

        private static IDictionary<string, SpriteFont> FontCache = new Dictionary<string, SpriteFont>();

        private enum ContentType
        {
            Model,
            Effect,
            SpriteFont,
            Texture,
            Texture2D,
            TextureCube
        }

        /// <summary>
        /// This is only set during the calling of LoadContent and UnloadContent. It is reset to null
        /// at the end of each method.
        /// </summary>
        private ContentManager Content;

        /// <summary>
        /// Creates a new Painter instance.
        /// </summary>
        public Painter()
        {}
        
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
            this.Content = content;
            Load();
            this.Content = null;
        }

        /// <summary>
        /// Unloads content not handled by the content manager.
        /// </summary>
        /// <param name="content">The content manager for the game.</param>
        public void UnloadContent(ContentManager content)
        {
            this.Content = content;
            Unload();
            this.Content = null;
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
        /// Creates a white, opaque, rectangular texture that is of the given size and stores it
        /// in the content cache. If the given ID has already had something assigned to it, a
        /// new rectangle is NOT created, and this method has no effect.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected void CreateRect(string id, int width, int height)
        {
            if (!Painter.TextureCache.ContainsKey(id))
            {
                Texture2D tex = new Texture2D(Painter.GraphicsDevice, width, height, false, SurfaceFormat.Color);
                Color[] data = new Color[width * height];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = new Color(255, 255, 255, 255);
                }
                tex.SetData(data);
                Painter.TextureCache[id] = tex;
            }
        }

        /// <summary>
        /// Loads a texture from the content pipeline and stores it in the cache.
        /// This will have no effect if the given ID has already been loaded or if called from
        /// outside of the Load() or Unload() methods.
        /// </summary>
        /// <param name="id"></param>
        protected void LoadTexture(string id)
        {
            if (Content != null && !Painter.TextureCache.ContainsKey(id))
            {
                Painter.TextureCache[id] = Content.Load<Texture2D>(id);
            }
        }

        protected void LoadFont(string id)
        {
            if (Content != null && !Painter.FontCache.ContainsKey(id))
            {
                Painter.FontCache[id] = Content.Load<SpriteFont>(id);
            }
        }

        protected void ConvertIDs(IList<Sprite> sprites)
        {
            foreach (Sprite spr in sprites)
            {
                spr.Texture = TextureFromID(spr.TextureID);
            }
        }

        /// <summary>
        /// Attempts to convert the resource name into a texture. If resource name is null, default texture is used.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Texture2D TextureFromID(string name)
        {
            if (name != null && Painter.TextureCache.ContainsKey(name))
            {
                return Painter.TextureCache[name];
            }
            else
            {
                return Painter.DefaultTexture;
            }
        }

        protected SpriteFont FontFromID(string name)
        {
            if (name != null && Painter.FontCache.ContainsKey(name))
            {
                return Painter.FontCache[name];
            }
            else
            {
                return Painter.DefaultFont;
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
        /// <returns>A dictionary mapping names to Texture2D resources.</returns>
        protected abstract void Load();

        /// <summary>
        /// Called by UnloadContent(). Override to dispose of graphical assets.
        /// </summary>
        protected abstract void Unload();
        
        /// <summary>
        /// Override to do the actual drawing to the screen. Do not call Begin() or End()
        /// on the provided SpriteBatch.
        /// </summary>
        /// <param name="gameTime">A snapshot of the current time.</param>
        /// <param name="batch">The SpriteBatch to use for drawing.</param>
        protected abstract void Paint(GameTime gameTime, SpriteBatch batch);
    }
}
