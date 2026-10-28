namespace Core.JG.Extensions
{
    public static class IndexConverter
    {
        public static (int x, int y) Get2dPosition<T>(this T[,] array, int index)
        {
            var width = array.GetLength(0);
            return (index % width, index / width);
        }

        public static int Get1dPosition<T>(this T[,] array, int x, int y)
        {
            var width = array.GetLength(0);
            return width * y + x;
        }

        public static int Get1dPosition<T>(this T[,] array, (int x, int y) indexPair)
        {
            (int x, int y) = indexPair;
            return Get1dPosition(array, x, y);
        }
    }
}