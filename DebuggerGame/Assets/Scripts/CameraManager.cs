using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private float sizePadding = 1f;

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

        Camera.main.gameObject.transform.position = new Vector3(
            board.width / 2f - 0.5f, board.height / 2f - 0.5f,
            Camera.main.gameObject.transform.position.z
        );
        
        float boardAspect = (float)board.width / board.height;
        if(boardAspect > lastAspect)
        {
            Camera.main.orthographicSize = board.width / 2f + 2 * sizePadding;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2f + 2 * sizePadding;
        }
    }
}
