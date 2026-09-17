using System;
using UnityEngine;

public static class TextureConvertor
{
    static int targetSizeKB = 100;
    static Texture2D tempTexture;

    public static string TextureToBase64(Texture2D texture)
    {
        int quality = 80;
        byte[] jpgBytes = texture.EncodeToJPG(quality);

        // byte[] jpgBytes = null;
        // while (quality > 80)
        // {
        //     jpgBytes = texture.EncodeToJPG(quality);
        //     if (jpgBytes.Length < targetSizeKB * 1024)
        //         break;
        //     quality -= 5;
        // }

        Debug.Log($"最終JPEGサイズ: {jpgBytes.Length / 1024} KB / Quality={quality}");

        // Base64化
        var result = Convert.ToBase64String(jpgBytes);
        Debug.Log($"Base64文字数: {result.Length}");

        return result;
    }

    public static Sprite Base64ToSprite(string base64)
    {
        return TextureToSprite(Base64ToTexture(base64));
    }

    public static Texture2D Base64ToTexture(string base64)
    {
        Dispose(tempTexture);
        byte[] imageBytes = Convert.FromBase64String(base64);
        tempTexture = new Texture2D(2, 2); // サイズは後で自動的に調整される
        if (!tempTexture.LoadImage(imageBytes))
        {
            Debug.LogError("テクスチャのロードに失敗しました。Base64が無効かもしれません。");
            return null;
        }

        Debug.Log($"画像サイズ: {tempTexture.width}x{tempTexture.height} / メモリ: {imageBytes.Length / 1024} KB");
        return tempTexture;
    }

    public static Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f) // pivot（中心を指定）
        );
    }

    static void Dispose(Texture2D texture)
    {
        if (texture != null)
        {
            UnityEngine.Object.Destroy(texture);
        }
    }
}
