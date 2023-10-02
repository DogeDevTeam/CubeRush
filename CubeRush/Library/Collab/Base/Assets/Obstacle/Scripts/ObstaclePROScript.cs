using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePROScript : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject CubePrefab;
    public int GridSize = 10;
    public float Gap = 0.5f;
    public int N;

    [Header("Other")]
    [Space(10)]
    public float ObstacleSpeed;
    public GameObject Player;
    private PlayerScript playerScript;
    public Color PlayerColor;
    public Color ObstacleColor;
    private GameObject[,] Grid;

    void Start()
    {
        // Generate random colors for player and obstacle
        PlayerColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), (Random.Range(0f, 1f)));
        ObstacleColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), (Random.Range(0f, 1f)));

        // Setup for obstacle and player
        Player = GameObject.FindGameObjectWithTag("Player");
        playerScript = Player.GetComponent<PlayerScript>();
        playerScript.gameManager.CanCreateNext = true;
        N = playerScript.gameManager.ObstacleN;
        ObstacleSpeed = playerScript.gameManager.ObstacleSpeed;
        Grid = new GameObject[GridSize, GridSize];
        CreateCubesGrid(Grid, GridSize);  // Create grid
        GenerateShape(Grid, N);  // Create player object
        transform.Rotate(0, 0, Random.Range(1, 4) * 90f);  // Change rotation to make challenge for player
    }

    void Update()
    {
        transform.Translate(0, 0, -ObstacleSpeed * Time.deltaTime);
    }

    private void CreateCubesGrid(GameObject[,] grid, int size)
    {
        float xPos = transform.position.x + ((-size / 2) * Gap);
        float yPos = transform.position.y + ((size / 2) * Gap);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject NewCube = (GameObject)Instantiate(CubePrefab, new Vector3(xPos, yPos, transform.position.z), Quaternion.identity);
                NewCube.GetComponent<Renderer>().material.color = ObstacleColor;
                grid[j, i] = NewCube;
                NewCube.transform.SetParent(transform);  // Create new cube as a child
                xPos += Gap;
            }

            xPos = transform.position.x + ((-size / 2) * Gap);
            yPos -= Gap;
        }
    }

    private void GenerateShape(GameObject[,] grid, int n)
    {
        List<GameObject> PlayerShape = new List<GameObject>();  // Cubes for player shape
        Vector2 StartCords = new Vector2(Random.Range(0, GridSize), Random.Range(0, GridSize));
        List<Vector2> Neigbors = GetNeigbors(grid, StartCords);
        GameObject StartCube = grid[(int)StartCords.x, (int)StartCords.y];
        PlayerShape.Add(StartCube);

        for (int i = 0; i < n; i++)
        {
            Vector2 RandNeighbor = Neigbors[Random.Range(0, Neigbors.Count)];
            Neigbors = GetNeigbors(grid, RandNeighbor);
            GameObject CubeToAdd = grid[(int)RandNeighbor.x, (int)RandNeighbor.y];
            
            if(n <= 2 && i == n - 1 || n > 2 && i == n - 2)
            {
                CubeToAdd.tag = "StartCube";
            }

            if (!PlayerShape.Contains(CubeToAdd))
            {
                PlayerShape.Add(CubeToAdd);
            }
        }

        // Create player shape
        CreatePlayerShape(PlayerShape);
        playerScript.Shape = PlayerShape;
    }

    private List<Vector2> GetNeigbors(GameObject[,] grid, Vector2 cords)
    {
        List<Vector2> nb = new List<Vector2>();

        Vector2 LEFT = new Vector2((int)cords.x - 1, (int)cords.y);
        Vector2 UP = new Vector2((int)cords.x, (int)cords.y - 1);
        Vector2 RIGHT = new Vector2((int)cords.x + 1, (int)cords.y);
        Vector2 DOWN = new Vector2((int)cords.x, (int)cords.y + 1);

        if (cords.x == 0 && cords.y == 0)
        {
            nb.Add(RIGHT);
            nb.Add(DOWN);
        }
        else if (cords.x == GridSize - 1 && cords.y == 0)
        {
            nb.Add(LEFT);
            nb.Add(DOWN);
        }
        else if (cords.x == GridSize - 1 && cords.y == GridSize - 1)
        {
            nb.Add(UP);
            nb.Add(LEFT);
        }
        else if (cords.x == 0 && cords.y == GridSize - 1)
        {
            nb.Add(UP);
            nb.Add(RIGHT);
        }
        else if (cords.x == 0)
        {
            nb.Add(UP);
            nb.Add(RIGHT);
            nb.Add(DOWN);
        }
        else if (cords.x == GridSize - 1)
        {
            nb.Add(UP);
            nb.Add(LEFT);
            nb.Add(DOWN);
        }
        else if (cords.y == 0)
        {
            nb.Add(LEFT);
            nb.Add(DOWN);
            nb.Add(RIGHT);
        }
        else if (cords.y == GridSize - 1)
        {
            nb.Add(LEFT);
            nb.Add(UP);
            nb.Add(RIGHT);
        }
        else
        {
            nb.Add(LEFT);
            nb.Add(UP);
            nb.Add(RIGHT);
            nb.Add(DOWN);
        }

        return nb;
    }

    private void CreatePlayerShape(List<GameObject> ShapeList)
    {
        Vector3 CorrectionPosition = Vector3.zero;
        foreach (GameObject cube in ShapeList)  // Find start cube in ShapeList
        {
            if (cube.CompareTag("StartCube"))
            {
                CorrectionPosition = cube.transform.position;
            }
        }

        // Add cubes to player object and fix position for all cubes in player
        foreach (GameObject cube in ShapeList)
        {
            cube.transform.SetParent(Player.transform);
            cube.GetComponent<Renderer>().material.color = PlayerColor;

            float fixX = cube.transform.position.x + (-CorrectionPosition.x);
            float fixY = cube.transform.position.y + (-CorrectionPosition.y);
            
            // Set correct position
            cube.transform.position = new Vector3(fixX, fixY, Player.transform.position.z);
        }
    }

    // Complete area collision
    private void OnTriggerEnter(Collider other)
    {
        playerScript.gameManager.NewObstacleSet();
        Destroy(gameObject);
    }
}
