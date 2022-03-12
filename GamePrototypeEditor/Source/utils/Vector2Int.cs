namespace GPE.utils
{
    public class Vector2Int
    {
        public static Vector2Int zero = new Vector3Int(0,0,0);
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Int(float x, float y)
        {
            this.x = (int)x;
            this.y = (int)y;
        }
    }
}