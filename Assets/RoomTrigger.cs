using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class RoomTrigger : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public List<Tuple<int, int>> entrances;
    public Tile sideDoorTile;
    public Tile topDoorTile;
    List<TileBase> originalTiles;
    public List<GameObject> enemies;
    public List<Entity> enemiesSpawned;
    public bool active;
    public bool cleared;
    [SerializeField] int maxCount;
    [SerializeField] int minCount;
    public AudioSource doorShutSound;
    public AudioSource doorOpenSound;
    public bool finalRoom;
    [SerializeField] GameObject portal;

    void Awake()
    {
        entrances = new List<Tuple<int, int>>();
        originalTiles = new List<TileBase>();
        enemiesSpawned = new List<Entity>();
    }

    void Start()
    {
        int count = UnityEngine.Random.Range(minCount, maxCount+1);

        for (int i = 0; i < count; i++)
        {
            float x = UnityEngine.Random.Range(transform.position.x - 4, transform.position.x + 4);
            float y = UnityEngine.Random.Range(transform.position.y - 4, transform.position.y + 4);
            var obj = Instantiate(enemies[UnityEngine.Random.Range(0, enemies.Count)],
                new Vector3(x, y, -0.1f), Quaternion.identity).GetComponent<Entity>();
            obj.GetComponent<Entity>().locked = true;
            enemiesSpawned.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active && !cleared)
        {
            List<Entity> newEnemies = new List<Entity>();
            foreach (Entity enemy in enemiesSpawned)
            {
                if (!enemy.IsDead())
                {
                    newEnemies.Add(enemy);
                }
            }

            enemiesSpawned = newEnemies;
            if (enemiesSpawned.Count == 0)
            {   
                doorOpenSound.Play();
                for (int i = 0; i < entrances.Count; i++)
                {
                    Tuple<int, int> entrance = entrances[i];
                    Vector3Int entrancePosition = new Vector3Int(entrance.Item1, entrance.Item2, 0);
                    groundTilemap.SetTile(entrancePosition, originalTiles[i]);
                    wallTilemap.SetTile(entrancePosition, null);
                }

                cleared = true;
                if (finalRoom) {
                    Instantiate(portal, transform.position + Vector3.back*2, Quaternion.identity);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !active)
        {
            doorShutSound.Play();
            active = true;
            foreach (var entrance in entrances)
            {
                Vector3Int entrancePosition = new Vector3Int(entrance.Item1, entrance.Item2, 0);
                originalTiles.Add(groundTilemap.GetTile(entrancePosition));
                groundTilemap.SetTile(entrancePosition, sideDoorTile);
                wallTilemap.SetTile(entrancePosition, topDoorTile);
            }

            foreach (var enemy in enemiesSpawned)
            {
                enemy.gameObject.GetComponent<Entity>().locked = false;
            }
        }
    }
}