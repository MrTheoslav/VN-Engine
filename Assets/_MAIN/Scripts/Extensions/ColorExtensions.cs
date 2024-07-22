using UnityEngine;

public static class ColorExtension
{
    public static Color SetAlpha(this Color original, float alpha)
    {
        return new Color(original.r, original.g, original.b, alpha);
    }

    public static Color GetColorFromName(this Color original, string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red":
                return Color.red;
            case "green":
                return Color.green;
            case "yellow":
                return Color.yellow;
            case "white":
                return Color.white;
            case "black":
                return Color.black;
            case "gray":
                return Color.gray;
            case "magenta":
                return Color.magenta;
            case "blue":
                return Color.blue;
            case "cyan":
                return Color.cyan;
            case "orange":
                return new Color(1f, 0.5f, 0f); //Orange is not a predefined color, so we create it manually
            default:
                Debug.LogWarning("Unrecognised color name " +  colorName);
                return Color.clear;

        }
    }
}
