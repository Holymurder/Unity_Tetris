using UnityEngine;

public class GhostTetromino : MonoBehaviour
{
    
    void Start()
    {
        tag = "currentGhostTetromino";

        foreach (Transform mino in transform) 
        {
            mino.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .2f);
        }

    }

    void Update()
    {
        
        FollowActiveTetromino();
        MoveDown();
    }

    void FollowActiveTetromino() 
    {
        
        Transform currentActiveTetrominoTransform = GameObject.FindGameObjectWithTag("currentActiveTetromino").transform;
        
        transform.position = currentActiveTetrominoTransform.position;
        transform.rotation = currentActiveTetrominoTransform.rotation;
        
    }

    void MoveDown() 
    {
        while (CheckIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }
        if (!CheckIsValidPosition()) 
        {
            transform.position += new Vector3(0, 1, 0);
        }

    }

    bool CheckIsValidPosition()
    {

        foreach (Transform mino in transform)
        {

            Vector2 pos = Object.FindAnyObjectByType<Game>().Round(mino.position);

            if (Object.FindAnyObjectByType<Game>().CheckIsInsideGrid(pos) == false)
                return false;

            if (Object.FindAnyObjectByType<Game>().GetTransformformGridPosition(pos) != null && Object.FindAnyObjectByType<Game>().GetTransformformGridPosition(pos).parent.tag == "currentActiveTetromino")
                return true;

            if (Object.FindAnyObjectByType<Game>().GetTransformformGridPosition(pos) != null && Object.FindAnyObjectByType<Game>().GetTransformformGridPosition(pos).parent != transform)
                return false;
        }

        return true;
    }


}
