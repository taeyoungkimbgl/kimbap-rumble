namespace seiko.framework.gyomu.constant
{
    public static class AppConstants
    {
        /* 操作パラメータ */

        // 給電・保守操作区分: 給電操作
        public const string KYUHOKBN_KYUDEN_SOSA = "1";
        // 給電・保守操作区分: 保守操作
        public const string KYUHOKBN_HOSHU_SOSA = "2";

        // 停止・送電操作区分: 停止操作
        public const string TEISOKBN_TESHI_SOSA = "1";
        // 停止・送電操作区分: 送電操作
        public const string TEISOKBN_SODEN_SOSA = "2";

        // 試験伝票フラグ: 通常伝票
        public const string SHIKENFLG_TSUJO_DENPYO = "0";
        // 試験伝票フラグ: 試験伝票
        public const string SHIKENFLG_SHIKEN_DENPYO = "1";

        // 現地給電操作フラグ: 現地給電操作無
        public const string GENCHIKYUDENFLG_GENCHISOSA_NASHI = "0";
        // 現地給電操作フラグ: 現地給電操作有
        public const string GENCHIKYUDENFLG_GENCHISOSA_ARI = "1";
        // 現地給電操作フラグ: 甲アース操作有
        public const string GENCHIKYUDENFLG_KOEARTH_SOSA = "3";

        // 伝票ダウンロード状態: 前日確認前
        public const int DENPYOSTATUS_MIKAKUNIN = 0;
        // 伝票ダウンロード状態: 前日確認済+ダウンロード済
        public const int DENPYOSTATUS_SUMIDOWNLOAD = 1;
        // 伝票ダウンロード状態: 前日確認済+未ダウンロード
        public const int DENPYOSTATUS_MIDOWNLOAD = 2;

        // 指令方法コード: 一指令一操作
        public const int SHIREIHOHO_ICHI = 1;
        // 指令方法コード: 目的指令操作
        public const int SHIREIHOHO_MOKUTEKI = 2;
        // 指令方法コード: 保守操作
        public const int SHIREIHOHO_HOSHU = 0;

        // デフォルト値(未設定値)
        public const int DEFAULT = 0;

        // 操作種別: 現地一人操作
        public const int SOSASHUBETSU_GENCHI_HITORI_SOSA = 1;
        // 操作種別: 二人操作
        public const int SOSASHUBETSU_FUTARI_SOSA = 2;
        // 操作種別: 完全一人操作
        public const int SOSASHUBETSU_KANZEN_HITORI_SOSA = 3;

        // 補修担当箇所応援: 無
        public const int HOSHU_OEN_NASHI = 1;
        // 補修担当箇所応援: 有
        public const int HOSHU_OEN_ARI = 2;

        // 取引先助勢: 無
        public const int TORIHIKI_JYOSEI_NASHI = 1;
        // 取引先助勢: 有
        public const int TORIHIKI_JYOSEI_ARI = 2;

        // 補修担当箇所助勢: 無
        public const int HOSHU_JYOSEI_NASHI = 1;
        // 補修担当箇所助勢: 有
        public const int HOSHU_JYOSEI_ARI = 2;

        // 操作者名：なし
        public const string SOSASHA_NAME_NONE = "―";

        /* 停止作業件名パラメータ */

        // 許可証発行状態: 未発行
        public const int HAKKOSTATUS_MIHAKKO = 0;
        // 許可証発行状態: 発行済
        public const int HAKKOSTATUS_SUMIHAKKO = 2;
        // 許可証発行状態: 発行報告済
        public const int HAKKOSTATUS_SUMIHOKOKU = 3;

        // 許可証回収状態: 未回収
        public const int KAISHUSTATUS_MIKAISHU = 0;
        // 許可証回収状態: 回収済み
        public const int KAISHUSTATUS_SUMIKAISHU = 2;
        // 許可証回収状態: 報告済
        public const int KAISHUSTATUS_SUMIHOKOKU = 3;

        /* 操作手順パラメータ */

        // 現地給電操作区分: 現地給電操作
        public const string GENCHIKBN_GENCHI_SOSA2 = "2";
        public const string GENCHIKBN_GENCHI_SOSA3 = "3";

        // 現地給電操作区分、お客様自主操作区分: 保守操作
        public const string KBN_HOSHU_SOSA = null;

        // お客様自主操作区分: お客様自主操作
        public const string OKYAKUKBN_OKYAKU_SOSA = "2";

        // 操作状況: 未操作
        public const int SOSASTATUS_MISOSA = 0;
        // 操作状況: 操作中
        public const int SOSASTATUS_SOSACHU = 1;
        // 操作状況: 操作済
        public const int SOSASTATUS_SUMISOSA = 2;
        // 操作状況: 報告済
        public const int SOSASTATUS_SUMIHOKOKU = 3;

        // 開閉器番号札コード: タグ無し
        public const int FUDACD_NASHI = 0;
        // 開閉器番号札コード: 機器札
        public const int FUDACD_KIKI = 1;
        // 開閉器番号札コード: アース札(付け)
        public const int FUDACD_EARTH_TSUKE = 4;
        // 開閉器番号札コード: アース札(外し)
        public const int FUDACD_EARTH_HAZUSHI = 5;
        // 開閉器番号札コード: 甲アース札
        public const int FUDACD_KOEARTH = 6;

        // 開閉器番号タグ周波数コード: タグ無し
        public const int TAGFREQ_NASHI = 0;
        // 開閉器番号タグ周波数コード: Mu-chip
        public const int TAGFREQ_MUCHIP = 1;
        // 開閉器番号タグ周波数コード: UHF
        public const int TAGFREQ_UHF = 2;

        // 開閉器番号添付ファイル区分: CGデータファイル
        public const string FILEKBN_CG = "1";
        // 開閉器番号添付ファイル区分: ユーザ添付ファイル
        public const string FILEKBN_USER = "2";

        // 現場端末ダウンロードフラグ: ダウンロード済
        public const string DOWNLOADFLG_SUMIDOWNLOAD = "1";

        /* 操作履歴パラメータ */

        // 現地操作フラグ: 現地操作
        public const string GENCHIFLG_GENCHI_SOSA = "1";
        // 現地操作フラグ: WEB操作
        public const string GENCHIFLG_WEB_SOSA = "0";
        /* APIリクエストパラメータ */

        // 書式なし
        public const string FORMATCONTENT_NASHI = "0";
        // 表示書式に合わせた内容を返す
        public const string FORMATCONTENT_ARI = "1";

        // 種別： 甲アース操作
        public const int SHUBETSU_KOEARTH = 2;

        /* ポップアップボタン オブジェクト名称 */

        // ポップアップボタン: OK
        public const string POPUP_MSG_OK = "popupBtnOk";
        // ポップアップボタン: キャンセル
        public const string POPUP_MSG_CANCEL = "popupBtnCancel";

        /* 操作画面表示用 */

        // 操作手順テーブル：透過度
        public const int DEFAULT_IMAGE_TRANSPARENCY = 200;
        // 操作手順テーブル：行高
        public const int DEFAULT_TEJYUN_ROW_HEIGHT = 60;
        // 操作手順テーブル：ヘッダ文字サイズ
        public const int DEFAULT_HEADER_FONT_SIZE = 18;
        // 操作手順テーブル：手順部文字サイズ
        public const int DEFAULT_TEJYUN_FONT_SIZE = 14;


        /* デバッグ用 */
        //Scene Name
        public const string GAME_SCENE = "GameScene";

        //For Dev
        public const string SPREAD_SHEET_URL = "https://script.google.com/macros/s/AKfycbz35iYxtt62dCyab7zabHZcGKPl3dqY4DzN-4diE1VI1pOr4fGwaSLZLrIJ22Purw95ZQ/exec";
        public const string SPREAD_SHEET_ID = "1_r270u-K9wb-Mi0p6aVWXtp8yIzFE4JjxaJLXgzRGRo";

        //For Release
        // public const string SPREAD_SHEET_URL = "https://script.google.com/macros/s/AKfycbzwfvbb0qywyDKIumOxk0HKXsvhSNxGjBeKeNLGnelUX1KRARHEZtRB_1Md4f0sD8ADwQ/exec";
        // public const string SPREAD_SHEET_ID = "1PAAMNHMx-4dRUhycXQUhTISRMIhGywCKPuObEpj6xyI";

        public const string SPREAD_SHEET_NAME_TEJYUN = "tejyunData";
        public const string SPREAD_SHEET_NAME_INPUT = "inputDataV2";

    }
}
