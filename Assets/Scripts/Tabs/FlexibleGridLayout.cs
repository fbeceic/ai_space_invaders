using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    [Header("Flexible Grid")] 
    public FitType fitType = FitType.Uniform;

    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;

    public bool fitX;
    public bool fitY;

    [Header("Pagination")] 
    public int itemsPerPage;
    public int currentPage = 0;

    public int GetCurrentPage()
    {
        return currentPage;
    }

    public void NextPage()
    {
        currentPage++;
        SetDirty();
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            SetDirty();
        }
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            int childCount = Mathf.Min(rectChildren.Count, itemsPerPage);
            float squareRoot = Mathf.Sqrt(childCount);
            rows = columns = Mathf.CeilToInt(squareRoot);
            
            switch (fitType)
            {
                case FitType.Width:
                    fitX = true;
                    fitY = false;
                    break;
                case FitType.Height:
                    fitX = false;
                    fitY = true;
                    break;
                case FitType.Uniform:
                    fitX = fitY = true;
                    break;
            }
        }

        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt((float)rectChildren.Count / columns);
        }

        if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt((float)rectChildren.Count / rows);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = parentWidth / columns - ((spacing.x / columns) * (columns - 1))
                                               - (padding.left / columns) - (padding.right / columns);
        float cellHeight = parentHeight / rows - ((spacing.y / rows) * (rows - 1))
                                              - (padding.top / rows) - (padding.bottom / rows);

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            int rowIndex = i / columns;
            int columnIndex = i % columns;

            var item = rectChildren[i];

            var xPos = (cellSize.x * columnIndex) + (spacing.x * columnIndex) + padding.left;
            var yPos = (cellSize.y * rowIndex) + (spacing.y * rowIndex) + padding.top;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
        // Vertical layout calculation logic, if needed.
    }

    public override void SetLayoutHorizontal()
    {
        // Horizontal layout setting logic, if needed.
    }

    public override void SetLayoutVertical()
    {
        // Vertical layout setting logic, if needed.
    }

    // Ensures layout recalculates whenever children change
    protected override void OnTransformChildrenChanged()
    {
        base.OnTransformChildrenChanged();
        SetDirty(); // Marks the layout as dirty, forcing recalculation
    }
}
