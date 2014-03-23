using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class Biome
    {
        private string biomeName;
       
       

        public string BiomeName
        {
            get { return biomeName; }
            set { biomeName = value; }
        }

        public Biome(string name, Texture2D sk, Texture2D bd)
        {

            biomeName = name;
            
        }

        
        
    }

}
