using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [HideInInspector]
    public bool DetectTouches = true;

    public readonly int MinimumMatchNumber = 3;

    [SerializeField]
    private Grid Grid;

    [SerializeField]
    private float GemWidth;

    [SerializeField]
    private GameManager GameController;

    private Gem[] Gems;

    private Vector2 InitialScreenTouchedPosition;

    private GridGem TouchedObject;

    private readonly float DelayBetweenMatches = 2f;

    public void LoadGems()
    {
        Gems = Resources.LoadAll<Gem>("Prefabs");
        for (int i = 0; i < Gems.Length; i++)
        {
            Gems[i].SetGemType((GemType)i);
        }
    }

    public void CreateGrid()
    {
        Grid.GridGems = new GridGem[Grid.Columns, Grid.Rows];

        for (int x = 0; x < Grid.Columns; x++)
        {
            for (int y = 0; y < Grid.Rows; y++)
            {
                Grid.GridGems[x, y] = InstantiateGem(x, y);
            }
        }
    }

    public void DetectInputEvents()
    {
        if (Input.touchCount > 0 && DetectTouches == true)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TouchedObject = GetTouchedObject(touch);
                    InitialScreenTouchedPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    if (TouchedObject != null)
                    {
                        GridGem swipeToGem = GetSwipeGem(touch);
                        if (swipeToGem != null)
                        {
                            StartCoroutine(TryMatch(TouchedObject, swipeToGem));
                            TouchedObject = null;
                        }
                    }
                    break;
            }
        }
    }

    public void ClearInitialMatches()
    {
        for (int x = 0; x < Grid.Columns; x++)
        {
            for (int y = 0; y < Grid.Rows; y++)
            {
                HashSet<GridGem> matches = SearchMatches(Grid.GridGems[x, y]);
                if (matches.Count >= MinimumMatchNumber)
                {
                    Destroy(Grid.GridGems[x, y].gameObject);
                    Grid.GridGems[x, y] = InstantiateGem(x, y);
                    y--;
                }
            }
        }
    }

    public IEnumerator CheckPossibleMoves()
    {
        if (DetectTouches) { DetectTouches = false; }

        bool shuffleNeeded = true;
        foreach (GridGem gem in Grid.GridGems)
        {
            List<GridGem> neighbors = SearchGemNeighbors(gem);
            shuffleNeeded = !FindFutureMatches(neighbors, gem.GemType);

            if (!shuffleNeeded) {
                break; 
            }
        }
        if (shuffleNeeded) { yield return StartCoroutine(ShuffleGrid()); }

        if (!DetectTouches) { DetectTouches = true; }
    }

    private void InstantiateGemsAfterDestroy(int x, int numberOfDestroyedColumnGems)
    {
        for (int y = (Grid.Rows - numberOfDestroyedColumnGems); y < Grid.Rows; y++)
        {
            Grid.GridGems[x, y] = InstantiateGem(x, y);
        }
    }

    private void SwapGemIndexes(GridGem touchedGem, GridGem swipedGem)
    {
        GridGem auxiliarGem = Grid.GridGems[touchedGem.Position.x, touchedGem.Position.y];

        Grid.GridGems[touchedGem.Position.x, touchedGem.Position.y] = swipedGem;
        Grid.GridGems[swipedGem.Position.x, swipedGem.Position.y] = auxiliarGem;

        Vector2Int auxiliarVector2 = swipedGem.Position;
        swipedGem.ChangeGemPosition(touchedGem.Position.x, touchedGem.Position.y);
        touchedGem.ChangeGemPosition(auxiliarVector2.x, auxiliarVector2.y);
    }

    private void AllowSwapBetweenGems(bool status)
    {
        foreach (GridGem gem in Grid.GridGems)
        {
            gem.transform.GetComponent<Rigidbody2D>().isKinematic = status;
        }
    }

    private bool CalculateFuturePossibleMatch(string direction, Vector2Int lowIndex, Vector2Int highIndex, GemType analyzedGem, int gridPosition)
    {
        switch (direction)
        {
            case "Left":

                if (lowIndex.x - 2 >= 0)
                {
                    bool leftPossibleGem = (Grid.GridGems[lowIndex.x - 2, gridPosition].GemType == analyzedGem);
                    return leftPossibleGem;
                }

                break;
            case "Right":

                if (highIndex.x + 2 < Grid.Columns)
                {
                    bool rightPossibleGem = (Grid.GridGems[highIndex.x + 2, gridPosition].GemType == analyzedGem);
                    return rightPossibleGem;
                }

                break;
            case "Up":

                if (highIndex.y + 2 < Grid.Rows)
                {
                    bool upPossibleGem = (Grid.GridGems[gridPosition, highIndex.y + 2].GemType == analyzedGem);
                    return upPossibleGem;
                }

                break;
            case "Down":

                if (lowIndex.y - 2 >= 0)
                {
                    bool downPossibleGem = (Grid.GridGems[gridPosition, lowIndex.y - 2].GemType == analyzedGem);
                    return downPossibleGem;
                }

                break;
        }
        return false;
    }

    private bool FindFutureMatches(List<GridGem> gemList, GemType analyzedGem)
    {
        IEnumerable<GridGem> sameGems = gemList.Where(gem => gem.GemType == analyzedGem);

        Vector2Int lowIndex = new Vector2Int(sameGems.Min(gem => gem.Position.x), sameGems.Min(gem => gem.Position.y));
        Vector2Int highIndex = new Vector2Int(sameGems.Max(gem => gem.Position.x), sameGems.Max(gem => gem.Position.y));

        int horizontalDifference = highIndex.x - lowIndex.x;
        int verticalDifference = highIndex.y - lowIndex.y;

        if (sameGems.Count() == MinimumMatchNumber - 1)
        {
            if (horizontalDifference + verticalDifference > 1) { return false; }

            else if (horizontalDifference == 1)
            {
                bool rightMatch = CalculateFuturePossibleMatch("Right", lowIndex, highIndex, analyzedGem, lowIndex.y);
                bool leftMatch = CalculateFuturePossibleMatch("Left", lowIndex, highIndex, analyzedGem, lowIndex.y);
                return rightMatch || leftMatch;
            }
            else if (verticalDifference == 1)
            {
                bool downMatch = CalculateFuturePossibleMatch("Down", lowIndex, highIndex, analyzedGem, lowIndex.x);
                bool upMatch = CalculateFuturePossibleMatch("Up", lowIndex, highIndex, analyzedGem, highIndex.x);
                return downMatch || upMatch;
            }
        }

        if (sameGems.Count() == MinimumMatchNumber)
        {
            if ((horizontalDifference + verticalDifference == 4) && sameGems.Count() == MinimumMatchNumber) { return false; }

            else if (horizontalDifference + verticalDifference == 3) { return true; }

            if (horizontalDifference + verticalDifference < 3)
            {
                if (sameGems.Where(gem => gem.Position.y == lowIndex.y).Count() == 2)
                {
                    bool leftDownPossibleGem = CalculateFuturePossibleMatch("Left", lowIndex, highIndex, analyzedGem, lowIndex.y);
                    bool rightDownPossibleGem = CalculateFuturePossibleMatch("Right", lowIndex, highIndex, analyzedGem, lowIndex.y);
                    return leftDownPossibleGem || rightDownPossibleGem;
                }

                if (sameGems.Where(gem => gem.Position.y == highIndex.y).Count() == 2)
                {
                    bool leftUpPossibleGem = CalculateFuturePossibleMatch("Left", lowIndex, highIndex, analyzedGem, highIndex.y);
                    bool rightUpPossibleGem = CalculateFuturePossibleMatch("Right", lowIndex, highIndex, analyzedGem, highIndex.y);
                    return leftUpPossibleGem || rightUpPossibleGem;
                }

                if (sameGems.Where(gem => gem.Position.x == lowIndex.x).Count() == 2)
                {
                    bool downLeftPossibleGem = CalculateFuturePossibleMatch("Down", lowIndex, highIndex, analyzedGem, lowIndex.x);
                    bool upLeftPossibleGem = CalculateFuturePossibleMatch("Up", lowIndex, highIndex, analyzedGem, lowIndex.x);
                    return downLeftPossibleGem || upLeftPossibleGem;
                }

                if (sameGems.Where(gem => gem.Position.x == highIndex.x).Count() == 2)
                {
                    bool downRightPossibleGem = CalculateFuturePossibleMatch("Down", lowIndex, highIndex, analyzedGem, highIndex.x);
                    bool upRightPossibleGem = CalculateFuturePossibleMatch("Up", lowIndex, highIndex, analyzedGem, highIndex.x);
                    return downRightPossibleGem || upRightPossibleGem;
                }
            }
        }

        if (sameGems.Count() > MinimumMatchNumber && (horizontalDifference + verticalDifference > 1)) { return true; }

        return false;
    }

    private List<GridGem> SearchGemNeighbors(GridGem gem)
    {
        List<GridGem> gemNeighbors = new List<GridGem>();

        int x = (gem.Position.x > 0) ? gem.Position.x - 1 : gem.Position.x;

        while (x < Grid.Columns && x <= (gem.Position.x + 1))
        {
            int y = (gem.Position.y > 0) ? gem.Position.y - 1 : gem.Position.y;

            while (y < Grid.Rows && y <= (gem.Position.y + 1))
            {
                gemNeighbors.Add(Grid.GridGems[x, y]);
                y++;
            }

            x++;
        }

        return gemNeighbors;
    }

    private IEnumerator TryMatch(GridGem touchedGem, GridGem swipedGem)
    {
        DetectTouches = false;
        AllowSwapBetweenGems(true);

        yield return StartCoroutine(SwapGem(touchedGem, swipedGem, 0.4f));

        HashSet<GridGem> destroySet = SearchMatches(touchedGem);
        destroySet.UnionWith(SearchMatches(swipedGem));

        if(destroySet.Count < MinimumMatchNumber)
        {
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(SwapGem(touchedGem, swipedGem, 0.4f));
        }

        AllowSwapBetweenGems(false);

        if (destroySet.Count >= MinimumMatchNumber)
        {
            yield return StartCoroutine(UpdateGrid(destroySet));
            yield return StartCoroutine(CheckPossibleMoves());
        }
        
        DetectTouches = true;
    }

    private IEnumerator ShuffleGrid()
    {
        AllowSwapBetweenGems(true);

        System.Random random = new System.Random();
        
        for(int x = 0; x < Grid.Columns; x++)
        {
            for(int y = 0; y < Grid.Rows; y++)
            {
                int newX = random.Next(Grid.Columns - x);
                int newY = random.Next(Grid.Rows - y);
                yield return StartCoroutine(SwapGem(Grid.GridGems[x, y], Grid.GridGems[newX, newY], 0.02f));
            }
        }

        AllowSwapBetweenGems(false);

        yield return new WaitForSeconds(DelayBetweenMatches);
        yield return StartCoroutine(FindMatchesAfterGridUpdate());
        yield return StartCoroutine(CheckPossibleMoves());
    }

    private IEnumerator UpdateGrid(HashSet<GridGem> destroySet)
    {
        yield return StartCoroutine(DestroyMatchedGems(destroySet));
        yield return StartCoroutine(UpdateGridReferences(destroySet));
        yield return StartCoroutine(FindMatchesAfterGridUpdate());

    }

    private IEnumerator DestroyMatchedGems(HashSet<GridGem> destroySet)
    {
        foreach (GridGem gem in destroySet)
        {
            yield return StartCoroutine(gem.transform.Reduce(Vector3.zero, 0.15f));
            Destroy(gem.gameObject);
        }
        GameController.UpdateScore(destroySet.Count);
    }

    private IEnumerator UpdateGridReferences(HashSet<GridGem> destroyedGems)
    {
        IEnumerable<GridGem> orderedSet = destroyedGems.OrderBy(gem => gem.Position.x);

        for (int x = orderedSet.First().Position.x; x <= orderedSet.Last().Position.x; x++)
        {
            int highestYValue = orderedSet.Where(gem => gem.Position.x == x).Max(gem => gem.Position.y);
            int numberOfDestroyedColumnGems = orderedSet.Where(gem => gem.Position.x == x).Count();

            for (int y = highestYValue + 1; y < Grid.Rows; y++)
            {
                Grid.GridGems[x, y].ChangeGemPosition(x, y - numberOfDestroyedColumnGems);
                Grid.GridGems[x, y - numberOfDestroyedColumnGems] = Grid.GridGems[x, y];
            }

            InstantiateGemsAfterDestroy(x, numberOfDestroyedColumnGems);
        }
        yield return new WaitForSeconds(DelayBetweenMatches);
    }

    private IEnumerator FindMatchesAfterGridUpdate()
    {
        for (int x = 0; x < Grid.Columns; x++)
        {
            for (int y = 0; y < Grid.Rows; y++)
            {
                HashSet<GridGem> subsequentMatches = SearchMatches(Grid.GridGems[x, y]);
                if (subsequentMatches.Count >= MinimumMatchNumber)
                {
                    yield return StartCoroutine(UpdateGrid(subsequentMatches));
                }
            }
        }
    }

    private IEnumerator SwapGem(GridGem touchedGem, GridGem swipedGem, float swapDuration)
    {
        Vector2 initialGemPosition = touchedGem.transform.position;

        StartCoroutine(touchedGem.transform.Move(swipedGem.transform.position, swapDuration));
        StartCoroutine(swipedGem.transform.Move(initialGemPosition, swapDuration));

        yield return new WaitForSeconds(swapDuration);

        SwapGemIndexes(touchedGem, swipedGem);
    }

    private HashSet<GridGem> SearchMatches(GridGem gem)
    {
        HashSet<GridGem> rowMatches = SearchRowMatches(gem);
        HashSet<GridGem> columnMatches = SearchColumnMatches(gem);
        HashSet<GridGem> matchesToDestroy = new HashSet<GridGem>();

        if (rowMatches.Count >= MinimumMatchNumber)
        {
            matchesToDestroy.UnionWith(rowMatches);
            matchesToDestroy.UnionWith(CheckForPerpendicularMatches(rowMatches, true));
        }
        else if(columnMatches.Count >= MinimumMatchNumber)
        {
            matchesToDestroy.UnionWith(columnMatches);
            matchesToDestroy.UnionWith(CheckForPerpendicularMatches(columnMatches, false));
        }

        return matchesToDestroy;
    }

    private HashSet<GridGem> CheckForPerpendicularMatches(HashSet<GridGem> matchedGems, bool isHorizontalMatch)
    {
        HashSet<GridGem> possibleMatch;
        HashSet<GridGem> perpendicularMatches = new HashSet<GridGem>();

        foreach (GridGem matchedGem in matchedGems)
        {
            possibleMatch = isHorizontalMatch ? SearchColumnMatches(matchedGem) : SearchRowMatches(matchedGem);

            bool newMatchesFound = possibleMatch.Count >= MinimumMatchNumber;

            if (newMatchesFound)
            {
                if (possibleMatch.Contains(matchedGem))
                {
                    possibleMatch.Remove(matchedGem);
                }
                perpendicularMatches = possibleMatch;
                perpendicularMatches.UnionWith(CheckForPerpendicularMatches(perpendicularMatches, !isHorizontalMatch));
            }
        }
        return perpendicularMatches;
    }

    private HashSet<GridGem> SearchRowMatches(GridGem gem)
    {
        HashSet<GridGem> rowMatches = new HashSet<GridGem> { gem };
        int left = gem.Position.x - 1;
        int right = gem.Position.x + 1;

        while (right < Grid.Columns && Grid.GridGems[right, gem.Position.y].GemType == gem.GemType)
        {
            rowMatches.Add(Grid.GridGems[right, gem.Position.y]);
            right++;
        }

        while (left >= 0 && Grid.GridGems[left, gem.Position.y].GemType == gem.GemType)
        {
            rowMatches.Add(Grid.GridGems[left, gem.Position.y]);
            left--;
        }
        return rowMatches;
    }

    private HashSet<GridGem> SearchColumnMatches(GridGem gem)
    {
        HashSet<GridGem> columnMatches = new HashSet<GridGem> { gem };
        int up = gem.Position.y + 1;
        int down = gem.Position.y - 1;

        while(up < Grid.Rows && Grid.GridGems[gem.Position.x, up].GemType == gem.GemType)
        {
            columnMatches.Add(Grid.GridGems[gem.Position.x, up]);
            up++;
        }

        while(down >= 0 && Grid.GridGems[gem.Position.x, down].GemType == gem.GemType)
        {
            columnMatches.Add(Grid.GridGems[gem.Position.x, down]);
            down--;
        }
        return columnMatches;
    }

    private GridGem GetSwipeGem(Touch touch)
    {
        Vector2 direction = (touch.position - InitialScreenTouchedPosition).normalized;

        float angleToXAxis = Vector2.SignedAngle(direction, Vector2.right);

        if (((angleToXAxis <= 45 && angleToXAxis > 0) || (angleToXAxis >= -45 && angleToXAxis < 0)) && ((TouchedObject.Position.x + Vector2.right.x) < Grid.Columns))
        {
            return Grid.GridGems[(TouchedObject.Position.x + Vector2Int.right.x), (TouchedObject.Position.y + Vector2Int.right.y)];
        }
        else if ((angleToXAxis > 45 && angleToXAxis <= 135) && ((TouchedObject.Position.y + Vector2.down.y) >= 0))
        {
            return Grid.GridGems[(TouchedObject.Position.x + Vector2Int.down.x), (TouchedObject.Position.y + Vector2Int.down.y)];
        }
        else if ((angleToXAxis < -45 && angleToXAxis >= -135) && ((TouchedObject.Position.y + Vector2Int.up.y) < Grid.Rows))
        {
            return Grid.GridGems[(TouchedObject.Position.x + Vector2Int.up.x), (TouchedObject.Position.y + Vector2Int.up.y)];
        }
        else if ((angleToXAxis > 135 || angleToXAxis < -135) && ((TouchedObject.Position.x + Vector2.left.x) >= 0))
        {
            return Grid.GridGems[(TouchedObject.Position.x + Vector2Int.left.x), (TouchedObject.Position.y + Vector2Int.left.y)];
        }
        else
        {
            return null;
        }
    }

    private GridGem GetTouchedObject(Touch touch)
    {
        Vector2 worldTouchedPosition = Camera.main.ScreenToWorldPoint(touch.position);

        RaycastHit2D hitInformation = Physics2D.Raycast(worldTouchedPosition, Camera.main.transform.forward);

        if (hitInformation.collider != null)
        {
            return hitInformation.transform.gameObject.GetComponent<GridGem>();
        }
        return null;
    }

    private GridGem InstantiateGem(int x, int y)
    {
        Gem randomGem = Gems[UnityEngine.Random.Range(0, Gems.Length)];
        GridGem newGem = Instantiate(randomGem, new Vector2(x * GemWidth, y-3), Quaternion.identity).GetComponent<GridGem>();
        newGem.ChangeGemPosition(x, y);
        newGem.SetGemType(randomGem.GemType);
        return newGem;
    }
}
