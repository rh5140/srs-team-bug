using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private float sizePadding = 1f;

    [SerializeField]
    private Vector2Int offset = new Vector2Int(0, 0);

    [SerializeField]
    private float menuBarPadding = 1.15f;

    private Board board;

    private float? lastAspect = null;

    private float safeScreenAspect => Screen.safeArea.width / Screen.safeArea.height;

    private void Start()
    {
        board = GetComponent<Board>();
        Debug.Assert(board != null, "Camera Manager must be attached to the board");
    }

    private void Update()
    {
        if(!(lastAspect is float lastAspectNN) || !Mathf.Approximately(lastAspectNN, safeScreenAspect))
        {
            CenterCamera();
        }
    }

    private void CenterCamera()
    {
        lastAspect = safeScreenAspect;

        // Center camera
        Camera.main.gameObject.transform.position = new Vector3(
            (board.width-1)/ 2f + offset.x, (board.height * menuBarPadding - 1) / 2f + offset.y,
            Camera.main.gameObject.transform.position.z
        );
        
        float boardAspect = (float)(board.width + 2*sizePadding)/ (board.height * menuBarPadding + 2 * sizePadding);
        if(boardAspect > lastAspect)
        {
            // width can vary freely
            // half-height will be determined by the half-width
            Camera.main.orthographicSize = (board.width / 2f + sizePadding)/lastAspect.Value;
        }
        else
        {
            // height can vary freely without making board go off screen
            // half-height based off of board height
            Camera.main.orthographicSize = board.height * menuBarPadding / 2f + sizePadding;
        }
    }
}