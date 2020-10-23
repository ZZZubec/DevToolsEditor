using System.IO.Compression;

namespace GPE.utils
{
    public class Vector3Int : Vector2Int
    {
        public int z;

        public Vector3Int(int x, int y, int z):base(x,z)
        {
            this.z = (int)z;
        }

        public Vector3Int(float x, float y, float z) : base(x, z)
        {
            this.z = (int)z;
        }
    }
}