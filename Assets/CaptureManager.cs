using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaptureManager : MonoBehaviour
{
    public List<string> listPath = new List<string>();
    public TMP_InputField[] path;

    public string layer0Path;
    public string layer1Path;
    public string layer2Path;

    public RawImage[] rImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPathAndLoadFiles()
    {
        StartCoroutine(CoGetPathAndLoadFiles());
    }

    public List<string> layer0List = new List<string>();
    public List<string> layer1List = new List<string>();
    public List<string> layer2List = new List<string>();
    public IEnumerator CoGetPathAndLoadFiles()
    {
        layer0Path = path[0].text;
        layer1Path = path[1].text;
        layer2Path = path[2].text;


        DirectoryInfo d = new DirectoryInfo(layer0Path);
        FileInfo[] fis = d.GetFiles();

        for (int i = 0; i < fis.Length; i++)
        {
            layer0List.Add(layer0Path + "/" + fis[i].Name);
        }

        d = new DirectoryInfo(layer1Path);
        fis = d.GetFiles();
        for (int i = 0; i < fis.Length; i++)
        {
            layer1List.Add(layer1Path + "/" + fis[i].Name);
        }

        d = new DirectoryInfo(layer2Path);
        fis = d.GetFiles();
        for (int i = 0; i < fis.Length; i++)
        {
            layer2List.Add(layer2Path + "/" + fis[i].Name);
        }


        for (int i = 0; i < layer1List.Count; i++)
        {

            byte[] fileData;
            fileData = File.ReadAllBytes(layer1List[i]);
            Texture2D tex = null;
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            rImage[1].texture = tex;

            fileData = File.ReadAllBytes(layer0List[i%layer0List.Count]);
           
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            rImage[0].texture = tex;

            fileData = File.ReadAllBytes(layer2List[i % layer2List.Count]);
         
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            rImage[2].texture = tex;

            string fileName = layer0List[i % layer0List.Count].Split("/")[layer0List[i % layer0List.Count].Split("/").Length - 1] + "##"
                + layer1List[i].Split("/")[layer1List[i].Split("/").Length - 1] + "##"
                + layer2List[i % layer2List.Count].Split("/")[layer2List[i % layer2List.Count].Split("/").Length - 1];

            fileName = fileName.Replace(".png", "").Replace(".PNG", "");

            TakeScreenshot(fileName);
           
            yield return null;

            Destroy(rImage[0].texture);
            Destroy(rImage[1].texture);
            Destroy(rImage[2].texture);

        }

    }

    public string stringOutputPath;
    public int fileCounter = 0;
    public Camera renderCam;
    int num;
    public void TakeScreenshot(string str)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderCam.targetTexture;

        renderCam.Render();

        Texture2D Image = new Texture2D(renderCam.targetTexture.width, renderCam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, renderCam.targetTexture.width, renderCam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();

        Destroy(Image);

        //when layer3 is empty
        str = str.Replace("##empty", "");
        str = str.Replace("empty##", "");


        for (int i = 0; i < 17; i++)
        {
            str = str.Replace("_copied" + i.ToString(), "");
        }

        File.WriteAllBytes(stringOutputPath + "/" + str + ".png", Bytes);

        fileCounter++;

       
    }

}
