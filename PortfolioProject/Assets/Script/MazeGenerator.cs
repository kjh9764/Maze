using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    [Header("미로 설정")]
    public int width = 10;                   // 미로 가로 셀 수
    public int height = 10;                  // 미로 세로 셀 수
    public float cellSize = 4f;              // 셀 하나의 크기

    [Range(0f, 1f)]
    [Header("분기 확률 (0=무작위, 1=순수 DFS)")]
    public float branchProbability = 0.7f;   // 분기 선택 확률
    [Range(0f, 0.2f)]
    [Header("루프 확률 (사후 벽 제거)")]
    public float loopProbability = 0.05f;    // 사후 벽 제거 확률

    [Header("프리팹")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject player;
    public GameObject endPoint;

    [Header("그 외")]
    public Text timerText;
    public Button exitBtn;
    public GameObject miniMap;

    GameManager gameManager;
    PlayerController playerController;

    // 내부에서 사용할 셀 클래스
    private class Cell
    {
        public bool visited = false;
        // 인덱스: 0 = 상, 1 = 우, 2 = 하, 3 = 좌
        public bool[] walls = new bool[4] { true, true, true, true };
    }

    private Cell[,] maze;
    private Vector2Int start = new Vector2Int(0, 0);

    void Start()
    {
        InitialSettings();
        GenerateMaze(); // 미로 데이터 생성
        DrawMaze();     // 미로 그리기
        GameObject playerCharacter = Instantiate(player);
        playerController =playerCharacter.transform.GetChild(0).GetComponent<PlayerController>();
        FindFurthestCell();
    }

    void GenerateMaze()
    {
        // 초기화
        maze = new Cell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = new Cell();

        // Growing Tree
        List<Vector2Int> list = new List<Vector2Int>();
        list.Add(start);
        maze[start.x, start.y].visited = true;
        System.Random rnd = new System.Random();

        while (list.Count > 0)
        {
            // 분기 확률에 따라 DFS(최근) 또는 랜덤 선택
            Vector2Int current = (rnd.NextDouble() < branchProbability)
                ? list[list.Count - 1]
                : list[rnd.Next(list.Count)];

            // 미방문 이웃 수집
            var neighbours = GetUnvisitedNeighbours(current);
            if (neighbours.Count > 0)
            {
                // 랜덤으로 선택해 벽 제거
                Vector2Int chosen = neighbours[rnd.Next(neighbours.Count)];
                RemoveWallsBetween(current, chosen);
                maze[chosen.x, chosen.y].visited = true;
                list.Add(chosen);
            }
            else
            {
                // 막다른 길이면 리스트에서 제거
                list.Remove(current);
            }
        }

        // 사후에 루프 추가 (벽 일부 제거)
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                if (rnd.NextDouble() < loopProbability)
                {
                    Vector2Int[] dirs = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
                    Vector2Int d = dirs[rnd.Next(dirs.Length)];
                    int nx = x + d.x, ny = y + d.y;
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        int dirIndex = DirToIndex(d);
                        if (maze[x, y].walls[dirIndex] && maze[nx, ny].visited)
                        {
                            maze[x, y].walls[dirIndex] = false;
                            maze[nx, ny].walls[(dirIndex + 2) % 4] = false;
                        }
                    }
                }
            }
    }

    // 미로 데이터를 기반으로 게임 오브젝트(벽, 바닥) 생성
    void DrawMaze()
    {
        GameObject mazeParent = new GameObject("Maze");
        Vector3 floorPos = new Vector3((width * cellSize - cellSize) / 2, -0.5f, (height * cellSize - cellSize) / 2);
        GameObject floor = Instantiate(floorPrefab, floorPos, Quaternion.identity, mazeParent.transform);
        floor.transform.localScale = new Vector3(width * cellSize / 10f, 1, height * cellSize / 10f);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Vector3 cellPos = new Vector3(x * cellSize, 0, y * cellSize);
                // 상
                if (maze[x, y].walls[0]) Instantiate(wallPrefab, cellPos + new Vector3(0, 0, cellSize / 2), Quaternion.identity, mazeParent.transform)
                    .transform.localScale = new Vector3(cellSize, wallPrefab.transform.localScale.y, 0.5f);
                // 우
                if (maze[x, y].walls[1]) Instantiate(wallPrefab, cellPos + new Vector3(cellSize / 2, 0, 0), Quaternion.Euler(0, 90, 0), mazeParent.transform)
                    .transform.localScale = new Vector3(cellSize, wallPrefab.transform.localScale.y, 0.5f);
                // 하
                if (maze[x, y].walls[2]) Instantiate(wallPrefab, cellPos + new Vector3(0, 0, -cellSize / 2), Quaternion.identity, mazeParent.transform)
                    .transform.localScale = new Vector3(cellSize, wallPrefab.transform.localScale.y, 0.5f);
                // 좌
                if (maze[x, y].walls[3]) Instantiate(wallPrefab, cellPos + new Vector3(-cellSize / 2, 0, 0), Quaternion.Euler(0, 90, 0), mazeParent.transform)
                    .transform.localScale = new Vector3(cellSize, wallPrefab.transform.localScale.y, 0.5f);
            }
    }

    // 이웃 중 미방문 셀 반환
    List<Vector2Int> GetUnvisitedNeighbours(Vector2Int cell)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        if (cell.y + 1 < height && !maze[cell.x, cell.y + 1].visited) list.Add(new Vector2Int(cell.x, cell.y + 1));
        if (cell.x + 1 < width && !maze[cell.x + 1, cell.y].visited) list.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.y - 1 >= 0 && !maze[cell.x, cell.y - 1].visited) list.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.x - 1 >= 0 && !maze[cell.x - 1, cell.y].visited) list.Add(new Vector2Int(cell.x - 1, cell.y));
        return list;
    }

    // 두 셀 사이 벽 제거
    void RemoveWallsBetween(Vector2Int a, Vector2Int b)
    {
        int dx = b.x - a.x;
        int dy = b.y - a.y;
        if (dx == 0 && dy == 1) // 상
        {
            maze[a.x, a.y].walls[0] = false;
            maze[b.x, b.y].walls[2] = false;
        }
        else if (dx == 1 && dy == 0) // 우
        {
            maze[a.x, a.y].walls[1] = false;
            maze[b.x, b.y].walls[3] = false;
        }
        else if (dx == 0 && dy == -1) // 하
        {
            maze[a.x, a.y].walls[2] = false;
            maze[b.x, b.y].walls[0] = false;
        }
        else if (dx == -1 && dy == 0) // 좌
        {
            maze[a.x, a.y].walls[3] = false;
            maze[b.x, b.y].walls[1] = false;
        }
    }

    // 방향을 인덱스로 변환
    int DirToIndex(Vector2Int d)
    {
        if (d == Vector2Int.up) return 0;
        if (d == Vector2Int.right) return 1;
        if (d == Vector2Int.down) return 2;
        return 3;
    }

    // 출발지로부터 가장 먼 셀 찾고 엔드포인트 생성
    public Vector2Int FindFurthestCell()
    {
        int[,] distance = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                distance[x, y] = -1;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        distance[start.x, start.y] = 0;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            int dist = distance[cur.x, cur.y];
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            for (int i = 0; i < 4; i++)
            {
                var n = cur + dirs[i];
                if (n.x < 0 || n.x >= width || n.y < 0 || n.y >= height) continue;
                if (distance[n.x, n.y] != -1) continue;
                if (!maze[cur.x, cur.y].walls[i])
                {
                    distance[n.x, n.y] = dist + 1;
                    queue.Enqueue(n);
                }
            }
        }

        int maxDist = 0;
        Vector2Int furthest = start;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (distance[x, y] > maxDist)
                {
                    maxDist = distance[x, y];
                    furthest = new Vector2Int(x, y);
                }

        Debug.Log($"최대 이동 거리: {maxDist}, 위치: {furthest}");
        Quaternion rot = Quaternion.identity;
        float scale = cellSize * 0.5f;
        endPoint.transform.localScale = new Vector3(scale, 10, scale);
        Vector3 pos = new Vector3(furthest.x * cellSize, 10, furthest.y * cellSize);
        Instantiate(endPoint, pos, rot);
        return furthest;
    }

    //초기세팅
    void InitialSettings()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        endPoint.GetComponent<EndPointChk>().gameManager = gameManager;

        switch (gameManager.gameLevel)
        {
            case 0:
                width = 10;
                height = 10;
                wallPrefab.transform.localScale = new Vector3(1f, 2f, 1f);
                miniMap.SetActive(true);
                break;
            case 1:
                width = 15;
                height = 15;
                wallPrefab.transform.localScale = new Vector3(1f, 3f, 1f);
                miniMap.SetActive(false);
                break;
            case 2:
                width = 20;          
                height = 20;
                wallPrefab.transform.localScale = new Vector3(1f, 4f, 1f);
                miniMap.SetActive(false);
                break;
            default:
                break;
        }

        exitBtn.onClick.AddListener(() => gameManager.Exit());

    }

    public void GameStart()
    { 
        gameManager.Timer(timerText);
        playerController.gameStartChk = true;
    }
}

