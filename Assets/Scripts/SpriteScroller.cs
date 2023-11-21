using UnityEngine;

public class SpriteScroller : MonoBehaviour
{
    [SerializeField] private Vector2 moveSpeed;

    private Vector2 offset;
    private Material material;
    private bool move = false;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;

        MainBall.onMoveBackground += MoveBackground;
        MainBall.onDestination += StopBackground;
    }
    private void OnDestroy()
    {
        MainBall.onMoveBackground -= MoveBackground;
        MainBall.onDestination -= StopBackground;
    }

    private void Update()
    {
        if (move)
        {
            material.mainTextureOffset += offset;
        }
        else
        {
            material.mainTextureOffset += offset;
            offset = moveSpeed * Time.deltaTime;
        }
    }
    private void MoveBackground(Vector3 distance)
    {
        distance = distance.normalized;

        float x = distance.x;
        float y = distance.y;
        Vector2 newDistance= new Vector2(x, y);

        moveSpeed.y = 1;
        offset = moveSpeed * Time.deltaTime* newDistance;
        move = true;
    }
    private void StopBackground()
    {
        moveSpeed.y = 0.05f;
        move = false;
    }
}
