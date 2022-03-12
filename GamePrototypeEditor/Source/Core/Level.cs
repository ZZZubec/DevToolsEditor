using System.Collections.Generic;
using GPE;
using Urho3DNet;

namespace GPE.Core
{

    public class Level
    {
        public string name;
        private StaticModel level_static;

        private int width;
        private int depth;

        private List<LevelTile[]> tiles;
        private int current_tile = 0;

        public Level(string name, int width, int depth)
        {
            this.name = name;
            tiles = new List<LevelTile[]>();
            var tile = new LevelTile[width * depth];
            tiles.Add(tile);
        }
    }
}