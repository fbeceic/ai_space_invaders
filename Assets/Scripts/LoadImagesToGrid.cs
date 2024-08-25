using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using TMPro;
using Txt2Img.Util;

public class LoadImagesToGrid : MonoBehaviour
{
    public string folderPath = Constants.GeneratedImagesOutputFolder;
    public FlexibleGridLayout flexibleGridLayout;
    public GameObject imagePrefab;

    public void LoadImages(int pageNumber, int pageSize)
    {
        if (Directory.Exists(folderPath))
        {
            ClearExistingImages();

            var files = Directory.GetFiles(folderPath, "*.png").ToList();
            var pagedFiles = files.Skip(pageNumber * pageSize).Take(pageSize).ToList();

            foreach (var file in pagedFiles)
            {
                var fileData = File.ReadAllBytes(file);
                var texture = new Texture2D(2, 2);
                texture.LoadImage(fileData);
                texture.Apply();

                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                var newImage = Instantiate(imagePrefab, flexibleGridLayout.gameObject.transform);

                var fileName = Path.GetFileName(file);
                newImage.transform.Find("GalleryTextWrapper/Filename").GetComponent<TextMeshProUGUI>().text = fileName;
                newImage.transform.Find("GalleryImageWrapper/GalleryImage").GetComponent<Image>().sprite = sprite;
            }
        }
        else
        {
            Debug.LogError("Folder path does not exist.");
        }
    }

    private void ClearExistingImages()
    {
        foreach (Transform child in flexibleGridLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }
}