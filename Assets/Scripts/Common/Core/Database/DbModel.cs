using System;
using UnityEngine;

public class DbModel { }

[Serializable]
public class TejyunData
{
    public int device_no;
    public string tejyun_id;
    public string tejyun_name;
    public int tejyun_order;
    public string input_field_type;
    public string input_range_type;
    public string base_value_comment;
    public string base_min;
    public string base_max;
    public int input_range_min;
    public int input_range_max;
    public string result;
    public string comment;
    public string scrsht;
}

[Serializable]
public class InputDataV2
{
    public int device_no;
    public string tejyun_id;
    public string input_min;
    public string input_max;
    public string comment;
    public string scrsht;
    public long date;
}

public class DbConfig
{
    // リトライ回数
    public int ReteryTimes = 3;
    // DBファイルの格納先
    public string FilePath = "file/DB/";
    // DBファイル名
    public string FileName = "sqlite.db";
    // 排他制御のセマフォ取得の待機時間(ミリ秒)
    public int TransactionWaitTime = 5000;
}