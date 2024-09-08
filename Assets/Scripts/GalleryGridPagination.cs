    using System;
    using System.IO;
    using Txt2Img.Util;
    using UnityEngine;
    using UnityEngine.UI;

    public class GalleryGridPagination : MonoBehaviour
    {
        public int numberOfPages;
        
        public FlexibleGridLayout gridLayout;
        public LoadImagesToGrid loadImagesToGrid;
        public Button nextPageButton;
        public Button previousPageButton;

        private AudioSource _audioSource;

        void Start()
        {
            nextPageButton.onClick.AddListener(OnNextPage);
            previousPageButton.onClick.AddListener(OnPreviousPage);
            _audioSource = GetComponent<AudioSource>();
            
            LoadCurrentPage();
        }

        void OnEnable()
        {
            gridLayout.currentPage = 0;
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

            _audioSource.Play();
            gridLayout.NextPage();
            LoadCurrentPage();
        }

        void OnPreviousPage()
        {
            if (gridLayout.GetCurrentPage() <= 0)
            {
                return;
            }

            _audioSource.Play();
            gridLayout.PreviousPage();
            LoadCurrentPage();
        }

        void LoadCurrentPage() => loadImagesToGrid.LoadImages(gridLayout.GetCurrentPage(), gridLayout.itemsPerPage);
    }