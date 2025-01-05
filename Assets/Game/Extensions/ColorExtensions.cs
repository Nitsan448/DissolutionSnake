using UnityEngine;

public static class ColorExtensions
{
    public static Color GetWithNewAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}