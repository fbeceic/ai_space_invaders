    using System.IO;
    using Txt2Img.Util;
    using UnityEngine;
    using UnityEngine.UI;

    public class GalleryGridPagination : MonoBehaviour
    {
        private int numberOfPages;
        
        public FlexibleGridLayout gridLayout;
        public LoadImagesToGrid loadImagesToGrid;
        public Button nextPageButton;
        public Button previousPageButton;

        void Start()
        {
            numberOfPages = Directory.GetFiles(Constants.GeneratedImagesOutputFolder, "*.png").Length / gridLayout.itemsPerPage;
            nextPageButton.onClick.AddListener(OnNextPage);
            previousPageButton.onClick.AddListener(OnPreviousPage);
            
            LoadCurrentPage();
        }

        void Update()
        {
            previousPageButton.interactable = gridLayout.GetCurrentPage() > 0;
            nextPageButton.interactable = gridLayout.GetCurrentPage() < numberOfPages - 1;
        }

        void OnNextPage()
        {
            if (gridLayout.GetCurrentPage() >= numberOfPages - 1)
            {
                return;
            }

            gridLayout.NextPage();
            LoadCurrentPage();
        }

        void OnPreviousPage()
        {
            if (gridLayout.GetCurrentPage() <= 0)
            {
                return;
            }

            gridLayout.PreviousPage();
            LoadCurrentPage();
        }

        void LoadCurrentPage() => loadImagesToGrid.LoadImages(gridLayout.GetCurrentPage(), gridLayout.itemsPerPage);
    }