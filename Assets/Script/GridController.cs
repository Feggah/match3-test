using System.Collections;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Grid Grid;

    private PossibleGem[] Gems;

    private Vector2 InitialScreenTouchedPosition;

    private Gem TouchedObject;

    public void DetectInputEvents()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TouchedObject = GetTouchedObject(touch);
                    InitialScreenTouchedPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Ended:
                    Gem swipeToGem = GetSwipeGem(touch);
                    if(swipeToGem != null)
                    {
                        StartCoroutine(SwapGem(TouchedObject, swipeToGem, 0.4f));
                    }
                    break;
            }
        }
    }

    private IEnumerator SwapGem(Gem from, Gem to, float swapDuration)
    {
        Vector2 initialGemPosition = from.transform.position;

        AllowSwapBetweenGems(true);

        StartCoroutine(from.transform.Move(to.transform.position, swapDuration));
        StartCoroutine(to.transform.Move(initialGemPosition, swapDuration));

        yield return new WaitForSeconds(swapDuration);

        SwapGemIndexes(from, to);

        AllowSwapBetweenGems(false);
    }

    private Gem GetSwipeGem(Touch touch)
    {
        Vector2 direction = (touch.position - InitialScreenTouchedPosition).normalized;

        float angleToXAxis = Vector2.SignedAngle(direction, Vector2.right);

        if (((angleToXAxis <= 45 && angleToXAxis > 0) || (angleToXAxis >= -45 && angleToXAxis < 0)) && ((TouchedObject.Position.x + Vector2.right.x) < Grid.Columns))
        {
            return Grid.GridGems[(int)(TouchedObject.Position.x + Vector2.right.x), (int)(TouchedObject.Position.y + Vector2.right.y)];
        }
        else if ((angleToXAxis > 45 && angleToXAxis <= 135) && ((TouchedObject.Position.y + Vector2.down.y) >= 0))
        {
            return Grid.GridGems[(int)(TouchedObject.Position.x + Vector2.down.x), (int)(TouchedObject.Position.y + Vector2.down.y)];
        }
        else if ((angleToXAxis < -45 && angleToXAxis >= -135) && ((TouchedObject.Position.y + Vector2.up.y) < Grid.Rows))
        {
            return Grid.GridGems[(int)(TouchedObject.Position.x + Vector2.up.x), (int)(TouchedObject.Position.y + Vector2.up.y)];
        }
        else if ((angleToXAxis > 135 || angleToXAxis < -135) && ((TouchedObject.Position.x + Vector2.left.x) >= 0))
        {
            return Grid.GridGems[(int)(TouchedObject.Position.x + Vector2.left.x), (int)(TouchedObject.Position.y + Vector2.left.y)];
        }
        else
        {
            return null;
        }
    }

    private void SwapGemIndexes(Gem from, Gem to)
    {
        Gem auxiliarFromGem = Grid.GridGems[(int)from.Position.x, (int)from.Position.y];

        Grid.GridGems[(int)from.Position.x, (int)from.Position.y] = to;
        Grid.GridGems[(int)to.Position.x, (int)to.Position.y] = auxiliarFromGem;

        Vector2 auxiliarVector2 = to.Position;
        to.ChangeGemPosition(from.Position.x, from.Position.y);
        from.ChangeGemPosition(auxiliarVector2.x, auxiliarVector2.y);
    }

    private Gem GetTouchedObject(Touch touch)
    {
        Vector2 worldTouchedPosition = Camera.main.ScreenToWorldPoint(touch.position);

        RaycastHit2D hitInformation = Physics2D.Raycast(worldTouchedPosition, Camera.main.transform.forward);

        if (hitInformation.collider != null)
        {
            return hitInformation.transform.gameObject.GetComponent<Gem>();
        }
        return null;
    }

    public Gem InstantiateGem(int x, int y)
    {
        PossibleGem randomGem = Gems[UnityEngine.Random.Range(0, Gems.Length)];
        Gem newGem = Instantiate(randomGem.GetComponent<Gem>(), new Vector2(x, y), Quaternion.identity);
        newGem.ChangeGemPosition(x, y);
        return newGem;
    }

    public void LoadGems()
    {
        Gems = Resources.LoadAll<PossibleGem>("Prefabs");
        for (int i = 0; i < Gems.Length; i++)
        {
            Gems[i].SetGemType((GemType)i);
        }
    }

    private void AllowSwapBetweenGems(bool status)
    {
        foreach (Gem gem in Grid.GridGems)
        {
            gem.transform.GetComponent<Rigidbody2D>().isKinematic = status;
        }
    }
}
