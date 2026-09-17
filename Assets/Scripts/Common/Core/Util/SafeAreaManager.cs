using UnityEngine;

public static class SafeAreaManager
{
    public static void SetSafeArea(GameObject targetObject, bool isButtomSafeArea, bool isTopSafeArea, bool isSideSafeArea, bool isParent)
    {
        // rectTransformを取得
        var rectTransform = targetObject.GetComponent<RectTransform>();

        // safeAreaポジションを取得
        var screenSafeArea = Screen.safeArea;
        var anchorMin = screenSafeArea.position;
        var anchorMax = screenSafeArea.position + screenSafeArea.size;

        // 横幅を設定
        anchorMin.x /= Screen.width;
        anchorMax.x /= Screen.width;

        // 縦幅を設定
        anchorMax.y /= Screen.height;
        anchorMin.y /= Screen.height;

        // 両サイドをsafeAreaに収めない場合
        if (!isSideSafeArea)
        {
            anchorMin.x = 0;
            anchorMax.x = 1;
        }

        // 上部をsafeAreaに収めない場合
        if (!isTopSafeArea)
        {
            anchorMax.y = 1;
        }

        // 下部をsafeAreaに収めない場合
        if (!isButtomSafeArea)
        {
            anchorMin.y = 0;
        }

        // アンカー更新
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        // 背面パネルの場合はヘッダの高さ分オフセットを適応
        if (isParent) rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -80f);

    }
}
