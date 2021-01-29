using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadAssetBundles : MonoBehaviour
{
    AssetBundle loadedAssetBundle;
    public string bundlePath;
    [SerializeField] Image testImage;
    void Start()
    {
        LoadAssetBundle(bundlePath);
    }

    void LoadAssetBundle(string bundleURL)
    {
        loadedAssetBundle = AssetBundle.LoadFromFile(bundleURL);
        Debug.Log(loadedAssetBundle == null ? " faliure to load assetBundle" : " assetBundle loaded");
        foreach( string i in loadedAssetBundle.GetAllAssetNames()) { 
            Debug.Log(i);
        }
    }

    public Sprite getSprite(string assetName)
    {
        return loadedAssetBundle.LoadAsset<Sprite>(assetName);
    }

    public Texture getTexture(string assetName)
    {
        return loadedAssetBundle.LoadAsset<Texture>(assetName);
    }

    public GameObject getPrefab(string assetName)
    {
        return loadedAssetBundle.LoadAsset<GameObject>(assetName);
    }

    public Animation getAnimation(string assetname)
    {
        return loadedAssetBundle.LoadAsset<Animation>(assetname);
    }
}
