using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.YongHak;
using System.IO;
using System;

public class JsonTest : MonoBehaviour
{
    // Start is called before the first frame update
    Shop shop;

    public void Save()
    {
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        CoinData coinData = new CoinData();
        coinData.coinNum = shop.coin;
        Debug.Log(coinData.coinNum + " 저장 성공");

        string json = JsonUtility.ToJson(coinData);
        string filename = "CoinData";
        //string path = Application.dataPath + "/YoungHak/Json/" + filename + ".Json";
        string path = Application.persistentDataPath + filename + ".Json";

        File.WriteAllText(path, json);
    }

    public void Load()
    {
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        string filename = "CoinData";
        string path = Application.persistentDataPath + filename + ".Json";
        try{
            string json = File.ReadAllText(path);
            CoinData coinData = JsonUtility.FromJson<CoinData>(json);
            shop.coin = coinData.coinNum;
        }
        catch(Exception e)
        {
            shop.coin = 10000;
        }
    }

    public void Reset()
    {
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        shop.coin = 10000;
    }
}

public class CoinData
{
    public int coinNum;
}
