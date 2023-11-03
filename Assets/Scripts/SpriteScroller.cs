using System.Collections;
using System.Collections.Generic;
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
        MainBall.onGameOver += StopBackground;
    }
    private void OnDestroy()
    {
        MainBall.onMoveBackground -= MoveBackground;
        MainBall.onDestination -= StopBackground;
        MainBall.onGameOver -= StopBackground;
    }

    private void Update()
    {
        if(move)
            material.mainTextureOffset += offset;
    }
    private void MoveBackground(Vector3 distance)
    {
        distance = distance.normalized;

        float x = distance.x;
        float y = distance.y;
        Vector2 newDistance= new Vector2(x, y);

        offset = moveSpeed * Time.deltaTime* newDistance;
        move = true;
    }
    private void StopBackground()
    {
        move = false;
    }
}
