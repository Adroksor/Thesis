using UnityEngine;

public class ItemFlip : MonoBehaviour
{
    [SerializeField] private SpriteRenderer handItem;   // drag HandItem's SpriteRenderer here

    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool mouseIsRight = mouseWorld.x >= transform.position.x;

        // flip horizontally by toggling flipX
        handItem.flipX = !mouseIsRight;
    }
}