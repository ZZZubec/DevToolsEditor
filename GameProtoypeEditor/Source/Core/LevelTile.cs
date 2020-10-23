using Urho3DNet;

namespace GPE.Core
{
    public class LevelTile
    {
        public Vector4 height;
        public int texture_floor_index;
        public int texture_wall_index;
        public int texture_roof_index;

        public LevelTile(Vector4 height, int texture_floor_index, int texture_wall_index, int texture_roof_index)
        {
            this.height = height;
            this.texture_floor_index = texture_floor_index;
            this.texture_wall_index = texture_wall_index;
            this.texture_roof_index = texture_roof_index;
        }
    }
}