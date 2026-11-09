using UnityEngine;

// Builds a small round sprite in code (white centre that can be tinted, black outline).
public static class DotSprite
{
    static Sprite sprite;

    public static Sprite Get()
    {
        if (sprite != null) return sprite;

        const int n = 32;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float c = (n - 1) / 2f;
        for (int y = 0; y < n; y++)
        for (int x = 0; x < n; x++)
        {
            float d = Vector2.Distance(new Vector2(x, y), new Vector2(c, c));
            Color col = d <= 10f ? Color.white : d <= 15f ? Color.black : Color.clear;
            tex.SetPixel(x, y, col);
        }
        tex.Apply();
        sprite = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
        return sprite;
    }
}
