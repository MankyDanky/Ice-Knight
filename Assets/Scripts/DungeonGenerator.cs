
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    int[,] dungeonMap;
    int[,] dungeonRooms;
    Tuple<int,int> startRoom;
    Tuple<int,int> finalRoom;
    public Tile[] groundTiles;
    public Tile[] wallTiles;
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public Tilemap torchTilemap;
    Dictionary<Tuple<int,int>, List<Tuple<int,int>>> roomConnections;
    Transform player;
    public GameObject[] iceCrystals;
    public RoomTrigger[,] roomTriggers;
    public GameObject roomTrigger;
    public Tile torchTopTile;
    public Tile torchSideTile;
    public GameObject torchLight;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        player = GameObject.FindWithTag("Player").transform;
        roomConnections = new Dictionary<Tuple<int, int>, List<Tuple<int, int>>>();
        dungeonRooms = new int[5, 5];
        dungeonMap = new int[80, 80];
        roomTriggers = new RoomTrigger[5, 5];
        startRoom = new Tuple<int, int>(UnityEngine.Random.Range(0,5), UnityEngine.Random.Range(0,5));
        player.transform.position = new Vector3(startRoom.Item1*16 + 8, startRoom.Item2*16 + 8, -0.1f);

        void FillRoom(int x, int y, int size) {
            int offset = (16 - size)/2;
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    dungeonMap[i + x + offset, j + y + offset] = 1;
                }
            }
        }

        void GenerateRoom (int i, int j) {
            dungeonRooms[i, j] = UnityEngine.Random.Range(6,7)*2;

            // Place crystals
            int offset = (16 - dungeonRooms[i,j])/2;
            for (int c = 0; c < UnityEngine.Random.Range(0, 3); c++) {
                Instantiate(iceCrystals[UnityEngine.Random.Range(0, iceCrystals.Length)], new Vector3(i*16 + offset + UnityEngine.Random.Range(2f, dungeonRooms[i,j] - 2f), j*16 + offset + UnityEngine.Random.Range(2f, dungeonRooms[i,j] - 2f), -0.1f), Quaternion.identity);
            }
            if (i != startRoom.Item1 || j != startRoom.Item2) {
                GameObject rt = Instantiate(roomTrigger, new Vector3(i*16 + offset + dungeonRooms[i,j]/2, j*16 + offset + dungeonRooms[i,j]/2, -0.1f), Quaternion.identity);
                rt.transform.localScale = new Vector3(dungeonRooms[i,j] - 2, dungeonRooms[i,j] - 1, 2);
                roomTriggers[i, j] = rt.GetComponent<RoomTrigger>();
            }
            

            FillRoom(i*16,j*16,dungeonRooms[i,j]);

            // Connect to adjacent rooms
            if (i > 0 && dungeonRooms[i-1, j] != 0 && !roomConnections[new Tuple<int, int>(i-1, j)].Contains(new Tuple<int, int>(i, j))) {
                roomConnections.Add(new Tuple<int, int>(i, j), new List<Tuple<int, int>>{new Tuple<int, int>(i-1, j)});
            } else if (i < 4 && dungeonRooms[i+1, j] != 0 && !roomConnections[new Tuple<int, int>(i+1, j)].Contains(new Tuple<int, int>(i, j))) {
                roomConnections.Add(new Tuple<int, int>(i, j), new List<Tuple<int, int>>{new Tuple<int, int>(i+1, j)});
            } else if (j > 0 && dungeonRooms[i, j-1] != 0 && !roomConnections[new Tuple<int, int>(i, j-1)].Contains(new Tuple<int, int>(i, j))) {
                roomConnections.Add(new Tuple<int, int>(i, j), new List<Tuple<int, int>>{new Tuple<int, int>(i, j-1)});
            } else if (j < 4 && dungeonRooms[i, j+1] != 0 && !roomConnections[new Tuple<int, int>(i, j+1)].Contains(new Tuple<int, int>(i, j))) {
                roomConnections.Add(new Tuple<int, int>(i, j), new List<Tuple<int, int>>{new Tuple<int, int>(i, j+1)});
            }


            // Check if final room
            if ((i == 0 || dungeonRooms[i-1, j] != 0) && (i == 4 || dungeonRooms[i+1, j] != 0) && (j == 0 || dungeonRooms[i, j-1] != 0) && (j == 4 || dungeonRooms[i, j+1] != 0)) {
                finalRoom = new Tuple<int, int>(i, j);
                roomTriggers[i, j].finalRoom = true;   
                return;
            }

            // Generated next room
            while (true) {
                switch (UnityEngine.Random.Range(0,4)) {
                    case 0:
                        if (i > 0 && dungeonRooms[i-1, j] == 0) {
                            if (roomConnections.ContainsKey(new Tuple<int, int>(i, j))) {
                                roomConnections[new Tuple<int, int>(i, j)].Add(new Tuple<int, int>(i-1, j));
                            } else {
                                roomConnections.Add(new Tuple<int, int>(i, j), new List<Tuple<int, int>>{new Tuple<int, int>(i-1, j)});
                            }
                            GenerateRoom(i-1, j);
                            return;
                        }
                        break;
                    case 1:
                        if (i < 4 && dungeonRooms[i+1, j] == 0) {
                            if (roomConnections.ContainsKey(new Tuple<int, int>(i, j))) {
                                roomConnections[new Tuple<int, int>(i, j)].Add(new Tuple<int, int>(i+1, j));
                            } else {
                                roomConnections.Add(new Tuple<int, int>(i, j), new List<Tuple<int, int>>{new Tuple<int, int>(i+1, j)});
                            }
                            GenerateRoom(i+1, j);
                            return;
                        }
                        break;
                    case 2:
                        if (j > 0 && dungeonRooms[i, j-1] == 0) {
                            if (roomConnections.ContainsKey(new Tuple<int, int>(i, j))) {
                                roomConnections[new Tuple<int, int>(i, j)].Add(new Tuple<int, int>(i, j-1));
                            } else {
                                roomConnections.Add(new Tuple<int, int>(i, j), new List<Tuple<int, int>>{new Tuple<int, int>(i, j-1)});
                            }
                            GenerateRoom(i, j-1);
                            return;
                        }
                        break;
                    case 3:
                        if (j < 4 && dungeonRooms[i, j+1] == 0) {
                            if (roomConnections.ContainsKey(new Tuple<int, int>(i, j))) {
                                roomConnections[new Tuple<int, int>(i, j)].Add(new Tuple<int, int>(i, j+1));
                            } else {
                                roomConnections.Add(new Tuple<int, int>(i, j), new List<Tuple<int, int>>{new Tuple<int, int>(i, j+1)});
                            }
                            GenerateRoom(i, j+1);
                            return;
                        }
                        break;
                }
            }
        }

        GenerateRoom(startRoom.Item1, startRoom.Item2);


        
        // Connect rooms
        foreach (var connection in roomConnections) {
            Tuple<int, int> key = connection.Key;
            List<Tuple<int, int>> values = connection.Value;
            foreach (var value in values) {
                RoomTrigger rt1 = roomTriggers[key.Item1, key.Item2];
                RoomTrigger rt2 = roomTriggers[value.Item1, value.Item2];
                int size1 = dungeonRooms[key.Item1, key.Item2];
                int size2 = dungeonRooms[value.Item1, value.Item2];
                int offset1 = (16 - size1)/2;
                int offset2 = (16 - size2)/2;

                if (key.Item1 == value.Item1) {
                    if (key.Item2 < value.Item2) {
                        for (int i = 0; i < offset1 + offset2; i++) {
                            dungeonMap[key.Item1*16 + offset1 + size1/2, i + key.Item2*16 + 16 - offset1] = 1;
                            dungeonMap[key.Item1*16 + offset1 + size1/2 - 1, i + key.Item2*16 + 16 - offset1] = 1;
                        }
                        if (! startRoom.Equals(new Tuple<int,int>(key.Item1, key.Item2))) {
                            rt1.entrances.Add(new Tuple<int, int>(key.Item1*16 + offset1 + size1/2, key.Item2*16 + 16 - offset1));
                            rt1.entrances.Add(new Tuple<int, int>(key.Item1*16 + offset1 + size1/2 - 1, key.Item2*16 + 16 - offset1));
                        }
                        if (! startRoom.Equals(new Tuple<int, int>(value.Item1, value.Item2))) {
                            rt2.entrances.Add(new Tuple<int, int>(key.Item1*16 + offset1 + size1/2, key.Item2*16 + 16 + offset2 - 1));
                            rt2.entrances.Add(new Tuple<int, int>(key.Item1*16 + offset1 + size1/2 - 1, key.Item2*16 + 16 + offset2 - 1));
                        }
                        int tx1 = key.Item1*16 + offset1 + size1/2 + 1;
                        int ty1 = key.Item2*16 + 16 - offset1 - 1;
                        int tx2 = key.Item1*16 + offset1 + size1/2 - 2;
                        int ty2 = key.Item2*16 + 16 - offset1 - 1;
                        int tx3 = key.Item1*16 + offset1 + size1/2 + 1;
                        int ty3 = key.Item2*16 + 16 + offset2;
                        int tx4 = key.Item1*16 + offset1 + size1/2 - 2;
                        int ty4 = key.Item2*16 + 16 + offset2;
                        dungeonMap[tx1, ty1] = 2;
                        dungeonMap[tx2, ty2] = 2;
                        dungeonMap[tx3, ty3] = 2;
                        dungeonMap[tx4, ty4] = 2;
                        Instantiate(torchLight, new Vector3(tx1 + 0.5f, ty1 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx2 + 0.5f, ty2 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx3 + 0.5f, ty3 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx4 + 0.5f, ty4 + 1f, -0.1f), Quaternion.identity);
                    } else {
                        for (int i = 0; i < offset1 + offset2; i++) {
                            dungeonMap[value.Item1*16 + offset2 + size2/2, value.Item2*16 + 16 - offset2 + i] = 1;
                            dungeonMap[value.Item1*16 + offset2 + size2/2 - 1, value.Item2*16 + 16 - offset2 + i] = 1;
                        }
                        if (! startRoom.Equals(new Tuple<int,int>(key.Item1, key.Item2))) {
                            rt1.entrances.Add(new Tuple<int, int>(value.Item1*16 + offset2 + size2/2, value.Item2*16 + 16 + offset1 - 1));
                            rt1.entrances.Add(new Tuple<int, int>(value.Item1*16 + offset2 + size2/2 - 1, value.Item2*16 + 16 + offset1 - 1));
                        }
                        if (! startRoom.Equals(new Tuple<int, int>(value.Item1, value.Item2))) {
                            rt2.entrances.Add(new Tuple<int, int>(value.Item1*16 + offset2 + size2/2, value.Item2*16 + 16 - offset2));
                            rt2.entrances.Add(new Tuple<int, int>(value.Item1*16 + offset2 + size2/2 - 1, value.Item2*16 + 16 - offset2));
                        }
                        int tx1 = value.Item1*16 + offset2 + size2/2 + 1;
                        int ty1 = value.Item2*16 + 16 + offset1;
                        int tx2 = value.Item1*16 + offset2 + size2/2 - 2;
                        int ty2 = value.Item2*16 + 16 + offset1;
                        int tx3 = value.Item1*16 + offset2 + size2/2 + 1;
                        int ty3 = value.Item2*16 + 16 - offset2 - 1;
                        int tx4 = value.Item1*16 + offset2 + size2/2 - 2;
                        int ty4 = value.Item2*16 + 16 - offset2 - 1;
                        dungeonMap[tx1, ty1] = 2;
                        dungeonMap[tx2, ty2] = 2;
                        dungeonMap[tx3, ty3] = 2;
                        dungeonMap[tx4, ty4] = 2;
                        Instantiate(torchLight, new Vector3(tx1 + 0.5f, ty1 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx2 + 0.5f, ty2 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx3 + 0.5f, ty3 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx4 + 0.5f, ty4 + 1f, -0.1f), Quaternion.identity);
                    }
                } else {
                if (key.Item1 < value.Item1) {
                        for (int i = 0; i < offset1 + offset2; i++) {
                            dungeonMap[i + key.Item1*16 + 16 - offset1, key.Item2*16 + offset1 + size1/2] = 1;
                            dungeonMap[i + key.Item1*16 + 16 - offset1, key.Item2*16 + offset1 + size1/2 - 1] = 1;
                        }
                        if (! startRoom.Equals(new Tuple<int,int>(key.Item1, key.Item2))) {
                            rt1.entrances.Add(new Tuple<int, int>(key.Item1*16 + 16 - offset1, key.Item2*16 + offset1 + size1/2));
                            rt1.entrances.Add(new Tuple<int, int>(key.Item1*16 + 16 - offset1, key.Item2*16 + offset1 + size1/2 - 1));
                        }
                        if (! startRoom.Equals(new Tuple<int, int>(value.Item1, value.Item2))) {
                            rt2.entrances.Add(new Tuple<int, int>(key.Item1*16 + 16 + offset2 - 1, key.Item2*16 + offset1 + size1/2));
                            rt2.entrances.Add(new Tuple<int, int>(key.Item1*16 + 16 + offset2 - 1, key.Item2*16 + offset1 + size1/2 - 1));
                        }

                        int tx1 = key.Item1*16 + 16 - offset1 - 1;
                        int ty1 = key.Item2*16 + offset1 + size1/2 + 1;
                        int tx2 = key.Item1*16 + 16 - offset1 - 1;
                        int ty2 = key.Item2*16 + offset1 + size1/2 - 2;
                        int tx3 = key.Item1*16 + 16 + offset2;
                        int ty3 = key.Item2*16 + offset1 + size1/2 + 1;
                        int tx4 = key.Item1*16 + 16 + offset2;
                        int ty4 = key.Item2*16 + offset1 + size1/2 - 2;
                        dungeonMap[tx1, ty1] = 2;
                        dungeonMap[tx2, ty2] = 2;
                        dungeonMap[tx3, ty3] = 2;
                        dungeonMap[tx4, ty4] = 2;
                        Instantiate(torchLight, new Vector3(tx1 + 0.5f, ty1 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx2 + 0.5f, ty2 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx3 + 0.5f, ty3 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx4 + 0.5f, ty4 + 1f, -0.1f), Quaternion.identity);
                    } else {
                        for (int i = 0; i < offset1 + offset2; i++) {
                            dungeonMap[value.Item1*16 + 16 - offset2 + i, value.Item2*16 + offset2 + size2/2] = 1;
                            dungeonMap[value.Item1*16 + 16 - offset2 + i, value.Item2*16 + offset2 + size2/2 - 1] = 1;
                        }
                        if (! startRoom.Equals(new Tuple<int,int>(key.Item1, key.Item2))) {
                            rt1.entrances.Add(new Tuple<int, int>(value.Item1*16 + 16 + offset1 - 1, value.Item2*16 + offset2 + size2/2));
                            rt1.entrances.Add(new Tuple<int, int>(value.Item1*16 + 16 + offset1 - 1, value.Item2*16 + offset2 + size2/2 - 1));
                        }
                        if (! startRoom.Equals(new Tuple<int, int>(value.Item1, value.Item2))) {
                            rt2.entrances.Add(new Tuple<int, int>(value.Item1*16 + 16 - offset2, value.Item2*16 + offset2 + size2/2));
                            rt2.entrances.Add(new Tuple<int, int>(value.Item1*16 + 16 - offset2, value.Item2*16 + offset2 + size2/2 - 1));
                        }
                        int tx1 = value.Item1*16 + 16 + offset1;
                        int ty1 = value.Item2*16 + offset2 + size2/2 + 1;
                        int tx2 = value.Item1*16 + 16 + offset1;
                        int ty2 = value.Item2*16 + offset2 + size2/2 - 2;
                        int tx3 = value.Item1*16 + 16 - offset2 - 1;
                        int ty3 = value.Item2*16 + offset2 + size2/2 + 1;
                        int tx4 = value.Item1*16 + 16 - offset2 - 1;
                        int ty4 = value.Item2*16 + offset2 + size2/2 - 2;
                        dungeonMap[tx1, ty1] = 2;
                        dungeonMap[tx2, ty2] = 2;
                        dungeonMap[tx3, ty3] = 2;
                        dungeonMap[tx4, ty4] = 2;
                        Instantiate(torchLight, new Vector3(tx1 + 0.5f, ty1 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx2 + 0.5f, ty2 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx3 + 0.5f, ty3 + 1f, -0.1f), Quaternion.identity);
                        Instantiate(torchLight, new Vector3(tx4 + 0.5f, ty4 + 1f, -0.1f), Quaternion.identity);
                    }
                }
            }
            
        }
        
        void DrawGround () {
            for (int i = 0; i < 80; i++) {
                for (int j = 0; j < 80; j++) {
                    if (dungeonMap[i, j] == 1) {
                        int top = dungeonMap[i+1,j];
                        int bottom = dungeonMap[i-1,j];
                        int right = dungeonMap[i,j+1];
                        int left = dungeonMap[i,j-1];
                        int topLeft = dungeonMap[i+1,j-1];
                        int bottomLeft = dungeonMap[i-1,j-1];
                        int topRight = dungeonMap[i+1,j+1];
                        int bottomRight = dungeonMap[i-1,j+1];
                        if (top == 1 && bottom == 1 && right == 1 && left == 1 && topLeft == 1 && bottomLeft == 1 && topRight == 1 && bottomRight == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[0]); // Center tile
                        } else if (top == 1 && bottom == 1 && right == 1 && left == 1 && topLeft == 1 && bottomLeft == 1 && topRight == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[1]); // Bottom right inverted tile
                        } else if (top == 1 && bottom == 1 && right == 1 && left == 1 && topLeft == 1 && bottomLeft == 1 && bottomRight == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[2]); // Top right inverted tile
                        } else if (top == 1 && bottom == 1 && right == 1 && left == 1 && topLeft == 1 && bottomRight == 1 && topRight == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[3]); // Bottom left inverted tile
                        } else if (top == 1 && bottom == 1 && right == 1 && left == 1 && bottomLeft == 1 && bottomRight == 1 && topRight == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[4]); // Top left inverted tile
                        } else if (top == 1 && topRight == 1 && right == 1 && bottomRight == 1 && bottom == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[5]); // Left tile
                        } else if (top == 1 && topLeft == 1 && left == 1 && bottomLeft == 1 && bottom == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[6]); // Right tile
                        } else if (left == 1 && topLeft == 1 && top == 1 && topRight == 1 && right == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[7]); // Bottom tile
                        } else if (right == 1 && bottomRight == 1 && bottom == 1 && bottomLeft == 1 && left == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[8]); // Top tile
                        } else if (top == 1 && topLeft == 1 && left == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[9]); // Bottom right tile
                        } else if (top == 1 && topRight == 1 && right == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[10]); // Bottom left tile
                        } else if (bottom == 1 && bottomLeft == 1 && left == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[11]); // Top right tile
                        } else if (bottom == 1 && bottomRight == 1 && right == 1) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[12]); // Top left tile
                        }
                    } else if (dungeonMap[i, j] == 2) {
                        groundTilemap.SetTile(new Vector3Int(i, j, 0), torchSideTile); // Torch side tile
                        torchTilemap.SetTile(new Vector3Int(i, j, 0), torchTopTile); // Torch top tile
                    } else if (dungeonMap[i, j] == 0) {
                        if ((i > 0 && dungeonMap[i-1, j] == 1) || (i < 79 && dungeonMap[i+1, j] == 1) || (j > 0 && dungeonMap[i, j-1] == 1) || (j < 79 && dungeonMap[i, j+1] == 1) || (i > 0 && j > 0 && dungeonMap[i-1, j-1] == 1) || (i < 79 && j > 0 && dungeonMap[i+1, j-1] == 1) || (i > 0 && j < 79 && dungeonMap[i-1, j+1] == 1) || (i < 79 && j < 79 && dungeonMap[i+1, j+1] == 1)) {
                            groundTilemap.SetTile(new Vector3Int(i, j, 0), groundTiles[13 + i%3]); // Wall tile
                            wallTilemap.SetTile(new Vector3Int(i, j, 0),wallTiles[i%3]);
                        }
                    }
                }
            }
        }

        DrawGround();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
