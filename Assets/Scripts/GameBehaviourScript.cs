using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameAI;
using System;
using Unity.VisualScripting;

public class GameBehaviourScript : MonoBehaviour
{
    [SerializeField] private Transform tileEmpty = null, tileEmptySolved = null;

    private Camera _camera;

    [SerializeField] private TilesMovementBehaviourScript[] tiles, solution;
    [SerializeField] private Vector2[] SolutionTileCoords;
    [SerializeField] private int[,] tilesInt = new int[4,4];
    private int[] emptyCoord()
    {
        int [] coord = new int[2];
        
        for (int i =0; i < tilesInt.GetLength(0); i++)
        {
            for(int j =0; j < tilesInt.GetLength(1); j++)
            {
                if (tilesInt[i,j] == 0)
                {
                    coord[0] = i;
                    coord[1] = j;
                    return coord;
                }
            }
        }

        throw new Exception("ERROR! Empty tiles could not be found!");
    }

    private int findIn(TilesMovementBehaviourScript search)
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] is not null)
                if (tiles[i].Equals(search))
                {
                    return i;
                }
        }

        throw new Exception("ERROR! Element is not included in the array!");
    }

    private int findNull()
    {
        for (int i = 0; i < tiles.Length; i++)
            if (tiles[i] is null)
                return i;
        throw new Exception("Empty tiles has not been found!");
    }

    private void Move()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit)
        {
            //Debug.Log(hit.transform.name);
            if (Vector2.Distance(tileEmpty.position, hit.transform.position) <= 7)
            {
                Vector2 lastEmptySpacePos = tileEmpty.position;
                TilesMovementBehaviourScript current = hit.transform.GetComponent<TilesMovementBehaviourScript>();
                var index = findIn(current);
                tileEmpty.position = current.TargetPos;
                current.TargetPos = lastEmptySpacePos;
                tiles[findNull()] = current;
                tiles[index] = null;
            }
        }
    }

    public void Shuffle()
    {
        //Debug.Log("R key presed");
        var passer = new System.Random();
        for (int j = 0; j < 600; j++)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] != null && Vector2.Distance(tileEmpty.position, tiles[i].TargetPos) <= 7)
                    if (passer.Next(2) == 1)
                    {
                        Vector2 lastEmptySpacePos = tileEmpty.position;
                        tileEmpty.position = tiles[i].TargetPos;
                        tiles[i].TargetPos = lastEmptySpacePos;
                        tiles[findNull()] = tiles[i];
                        tiles[i] = null;
                        break;
                    }
            }
        }
    }

    public void StateVerification()
    {
        int tileCount = 0;
        while (tileCount < solution.Length)
        {
            for (int i = 0; i < Math.Sqrt(tilesInt.Length); i++)
            {
                for(int j = 0; j < Math.Sqrt(tilesInt.Length); j++)
                {
                    if (solution[tileCount] == null && tilesInt[i, j] != 0)
                        throw new Exception("Provided solution doesn't match puzzle's solution state!");
                    if (tileCount < solution.Length - 1 && solution[tileCount].key != tilesInt[i, j])
                        throw new Exception("Provided solution doesn't match puzzle's solution state!");

                    tileCount++;
                }
            }
        }
    }

    public void ArrayUpdate()
    {
        int tileCount = 0;
        for (int i = 0; i < tiles.Length / 4; i++)
        {
            for (int j = 0; j < tiles.Length / 4; j++)
            {
                if (tiles[tileCount] != null)
                {
                    tilesInt[i, j] = tiles[tileCount].key;
                    tileCount++;
                }
                else
                {
                    tilesInt[i, j] = 0;
                    tileCount++;
                }

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        solution = (TilesMovementBehaviourScript[])tiles.Clone();
        SolutionTileCoords = new Vector2[tiles.Length - 1];
        _camera = Camera.main;
        for(int i = 0; i < tiles.Length - 1; i++)
        {
            SolutionTileCoords[i] = solution[i].TargetPos;
        }
        ArrayUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Move();
            //ArrayUpdate();
        }

        if (Input.GetButtonDown("R Button"))
        {
            Shuffle();
            //ArrayUpdate();
        }

        if (Input.GetButtonDown("Submit"))
        {
            var solutionArray = (int[,])tilesInt.Clone();
            ArrayUpdate();
            //Debug.Log("Enter pressed");
            var coord = emptyCoord();
            var initialState = new StateNode<int>(tilesInt, coord[0], coord[1], 0);
            var solutionState = new StateNode<int>(solutionArray, 3, 3, 0);
            var aStar = new AStar<int>(initialState, solutionState, 0);
            var solved = aStar.Execute();
            tilesInt = (int[,])solved.State.Clone();
            StateVerification();
            tiles = (TilesMovementBehaviourScript[])solution.CloneViaSerialization();
            for (int i = 0; i < tiles.Length - 1; i++)
            {
                tiles[i].TargetPos = SolutionTileCoords[i];
            }
            tileEmpty.position = tileEmptySolved.position;
        }
    }
}
