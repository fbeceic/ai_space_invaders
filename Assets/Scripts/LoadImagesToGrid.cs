using UnityEngine;
using System.IO;
using System.Linq;
using Txt2Img.Util;
using UnityEngine.Serialization;

public class LoadImagesToGrid : MonoBehaviour
{
    public FlexibleGridLayout flexibleGridLayout;
    public GameObject imagePrefab;
    public GalleryGridPagination galleryGridPagination;

    public void LoadImages(int pageNumber, int pageSize)
    {
        var folderPath = Constants.GeneratedImagesOutputFolder;
        var editingPromptResult = AIManager.Instance.editingPromptResult;
        PromptTheme? activePromptTheme;
        if (editingPromptResult != null)
        {
            activePromptTheme = editingPromptResult.theme;
        }
        else
        {
            activePromptTheme = null;
        }
        
        if (Directory.Exists(folderPath))
        {
            ClearExistingImages();

            var files = Directory.GetFiles(folderPath, "*.png")
                .Where(file =>
                {
                    var (prompt, promptTheme, promptThemeString) = AIManager.Instance.ResolveAttributesFromFilename(Path.GetFileName(file));
                    if (activePromptTheme == null || activePromptTheme == promptTheme)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                })
                .OrderByDescending(File.GetLastWriteTime)
                .ToList();

            AIManager.Instance.galleryResultFilenames = files;
            galleryGridPagination.numberOfPages = files.Count / galleryGridPagination.gridLayout.itemsPerPage;
            
            var pagedFiles = files.Skip(pageNumber * pageSize).Take(pageSize).ToList();

            foreach (var file in pagedFiles)
            {
                var fileData = File.ReadAllBytes(file);
                var texture = new Texture2D(2, 2);
                texture.LoadImage(fileData);
                texture.Apply();

                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                var newImage = Instantiate(imagePrefab, flexibleGridLayout.gameObject.transform);

                newImage.GetComponent<GalleryResult>().ApplyResultFeatures(Path.GetFileName(file), sprite);
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