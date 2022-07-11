public static class ExtensionMethods {
    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static UnityEngine.Vector2 OnUnitCircle(this System.Random rand) {
        var angle = rand.NextDouble()*2*System.Math.PI;
        return new UnityEngine.Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle));
    }

    public static void Add(this ref UnityEngine.Vector2Int vec, int x, int y) {
        vec.Set(vec.x+x, vec.y+y);
    }

    public static UnityEngine.Color32 AverageColorFromTexture(this UnityEngine.Texture2D tex) {
        var texColors = tex.GetPixels32();

        int total = texColors.Length;
        float r = 0;
        float g = 0;
        float b = 0;

        for(int i = 0; i < total; i++) {
            r += texColors[i].r;
            g += texColors[i].g;
            b += texColors[i].b;
        }

        return new UnityEngine.Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , 255);
    }

    static UnityEngine.Color32 Multiply(this UnityEngine.Color32 c, int x) {
        return new UnityEngine.Color32((byte)(c.r*x), (byte)(c.g*x), (byte)(c.b*x), (byte)(c.a*x));
    }

    public static T Clamped<T>(this T[,] map, int x, int y) {
        return map[UnityEngine.Mathf.Clamp(x, 0, map.GetLength(0)-1), UnityEngine.Mathf.Clamp(y, 0, map.GetLength(1)-1)];
    }
}
