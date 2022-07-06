public static class ExtensionMethods {
    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static UnityEngine.Vector2 OnUnitCircle(this System.Random rand) {
        var angle = rand.NextDouble()*2*System.Math.PI;
        return new UnityEngine.Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle));
    }
}
