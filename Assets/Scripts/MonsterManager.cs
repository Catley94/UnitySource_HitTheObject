using System.Collections;
using System.Collections.Generic;
using Destructible2D;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterManager : MonoBehaviour
{

    [FormerlySerializedAs("monsters")] [SerializeField] private Sprite[] monsterSpritePool;
    [SerializeField] private GameObject monsterSpritePrefab;
    [SerializeField] private List<GameObject> monsterGameObjects = new List<GameObject>();
    [SerializeField] private int defeatedMonsters = 0;
    [SerializeField] private int round = 1;
    [SerializeField] private int nextMonsterID = 0;
    [SerializeField] private int baseCoinsPerMonster = 1;
    [SerializeField] private int score = 0;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int numOfMonstersPerRound = 6;
    [SerializeField] private int numOfMonstersPerRoundToRemove = 6;

    private float zValue = 0f;

    private int initialMonsterCount = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        BuildAllMonsters();
        monsterGameObjects[0].GetComponent<D2dDestructibleSprite>().Indestructible = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool atleastOneIsDestructable = false;
        foreach (Transform monster in transform)
        {
            if (!monster.GetComponent<D2dDestructible>().Indestructible)
            {
                atleastOneIsDestructable = true;
            }
        }

        if (!atleastOneIsDestructable)
        {
            Debug.LogWarning("No destructable objects found, setting first object to destructable");
            transform.GetChild(0).GetComponent<D2dDestructible>().Indestructible = false;
        }
    }

    public int GetRound()
    {
        return round;
    }

    private void BuildAllMonsters()
    {
        InstantiateMonsters(3);
    }

    private void InstantiateMonsters(int numOfMonsters) //int numOfMonsters
    {
        for (int i = 0; i < numOfMonsters; i++)
        {
            GameObject monsterSprite = Instantiate(monsterSpritePrefab, transform);
            monsterSprite.transform.position = new Vector3(0f, 0f, zValue);
            
            monsterSprite.GetComponent<SpriteRenderer>().sprite = monsterSpritePool[nextMonsterID];
            
            ResizePrefabFromSprite(monsterSprite);

            monsterSprite.GetComponent<D2dDestructibleSprite>().Shape = monsterSpritePool[nextMonsterID];
            monsterSprite.GetComponent<D2dDestructibleSprite>().Rebuild();
            monsterSprite.name = monsterSpritePool[nextMonsterID].name + " " + nextMonsterID;
            
            monsterSprite.GetComponent<D2dSnapshot>().Clear();
            
            D2dSnapshotData data = new D2dSnapshotData();
            data.Save(monsterSprite.GetComponent<D2dDestructibleSprite>());
            monsterSprite.GetComponent<D2dSnapshot>().Data = data;
            
            monsterSprite.GetComponent<D2dDestructibleSprite>().HealSnapshot = monsterSprite.GetComponent<D2dSnapshot>();

            monsterGameObjects.Add(monsterSprite);
            zValue++;
            
            if (nextMonsterID < monsterSpritePool.Length - 1)
            {
                nextMonsterID++;
            }
            else
            {
                nextMonsterID = 0;
            }
        }
        SetupMonsters();
    }

    private static void ResizePrefabFromSprite(GameObject monsterSprite)
    {
        float spriteWidth = monsterSprite.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        Camera mainCamera = Camera.main;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        float widthScale = cameraWidth / spriteWidth;
        // Set the scale
        monsterSprite.transform.localScale = new Vector3(widthScale, widthScale, 1);
    }

    private void SetupMonsters()
    {
        for (int i = 0; i < monsterGameObjects.Count; i++)
        {
            GameObject currentMonster = monsterGameObjects[i]; //TODO: Currently loops through all monsters and adds requirements even if they already have
            GameObject nextMonster = null;
            
            /*
             *  First iteration:
             *      monsterGameObjects.Count = 3;
             *      all currentMonsters will not have D2dRequirements
             *          Setup D2dRequirements for currentMonster to look at nextMonster
             *
             *  Second iteration:
             *      monsterGameObjects.Count = 4;
             *      if currentMonster does have D2dRequirements && i != mosnterGameObjects.Count - 2 , skip / continue
             *      
             */
            
            
            
            
            if (currentMonster.GetComponent<D2dRequirements>() != null && i != monsterGameObjects.Count - 2)
            {
                continue;
            }
            
            if (i + 1 < monsterGameObjects.Count)
            {
                nextMonster = monsterGameObjects[i+1];
                
                SetupDestructibleRequirements(currentMonster, nextMonster);

                SetupIndestructibleRequirements(currentMonster, nextMonster);
            }


            
        }
    }

    private void SetupIndestructibleRequirements(GameObject currentMonster, GameObject nextMonster)
    {
        D2dRequirements inDestructibleRequirements = currentMonster.AddComponent<D2dRequirements>();
        inDestructibleRequirements.AlphaRatio = true;
        inDestructibleRequirements.AlphaRatioMin = 0.5f;
        inDestructibleRequirements.AlphaRatioMax = 1f;
        SetupIndestructibleListener(nextMonster, inDestructibleRequirements);
    }

    private static void SetupIndestructibleListener(GameObject nextMonster, D2dRequirements inDestructibleRequirements)
    {
        inDestructibleRequirements.OnRequirementsMet.AddListener(() =>
        {
            if (nextMonster) nextMonster.GetComponent<D2dDestructibleSprite>().Indestructible = true;
        });
    }

    private void SetupDestructibleRequirements(GameObject currentMonster, GameObject nextMonster)
    {
        // Debug.Log($"{currentMonster.name} is looking at {nextMonster.name}");
        D2dRequirements destructibleRequirements = currentMonster.AddComponent<D2dRequirements>();
        destructibleRequirements.AlphaRatio = true;
        destructibleRequirements.AlphaRatioMin = 0f;
        destructibleRequirements.AlphaRatioMax = 0.4f;
        SetupDestructibleListener(nextMonster, destructibleRequirements);
    }

    private void IncreaseScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    private void SetupDestructibleListener(GameObject nextMonster, D2dRequirements destructibleRequirements)
    {
        destructibleRequirements.OnRequirementsMet.AddListener(() =>
        {
            if (nextMonster)
            {
                nextMonster.GetComponent<D2dDestructibleSprite>().Indestructible = false;
                FindObjectOfType<CurrencyManager>().MoneyReward(baseCoinsPerMonster * round); //TODO 1 * Round
                if (defeatedMonsters > 0 && defeatedMonsters % numOfMonstersPerRound == 0)
                {
                    round++;
                    for (int i = 0; i < numOfMonstersPerRoundToRemove; i++)
                    {
                        // Debug.Log($"Destroying {monsterGameObjects[0].name}");
                        Destroy(monsterGameObjects[0]);
                        monsterGameObjects.RemoveAt(0);
                    }
                }
                //Add new monster to list and Instantiate monster
                defeatedMonsters++;
                IncreaseScore();
            }
            InstantiateMonsters(1);
        });
    }
}
