using System.Collections;
using System.Collections.Generic;
using Destructible2D;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnMonsters : MonoBehaviour
{

    [SerializeField] private Sprite[] monsters;
    [SerializeField] private GameObject monsterSpritePrefab;
    [SerializeField] private List<GameObject> monsterSprites = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        BuildAllMonsters();
        monsterSprites[0].GetComponent<D2dDestructibleSprite>().Indestructible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BuildAllMonsters()
    {
        InstantiateMonsters();

        SetupMonsters();
    }

    private void InstantiateMonsters()
    {
        //Needs to be instantiated backwards as they layer on top of each other, newer on top
        for (int i = 0; i < monsters.Length; i++)
        {
            GameObject monsterSprite = Instantiate(monsterSpritePrefab, transform);
            monsterSprite.transform.position = new Vector3(0f, 0f, i);
            monsterSprite.GetComponent<SpriteRenderer>().sprite = monsters[i];
            monsterSprite.GetComponent<D2dDestructibleSprite>().Shape = monsters[i];
            monsterSprite.GetComponent<D2dDestructibleSprite>().Rebuild();
            monsterSprite.name = monsters[i].name;
            
            monsterSprite.GetComponent<D2dSnapshot>().Clear();
            
            D2dSnapshotData data = new D2dSnapshotData();
            data.Save(monsterSprite.GetComponent<D2dDestructibleSprite>());
            monsterSprite.GetComponent<D2dSnapshot>().Data = data;
            
            monsterSprite.GetComponent<D2dDestructibleSprite>().HealSnapshot = monsterSprite.GetComponent<D2dSnapshot>();

            monsterSprites.Add(monsterSprite);
        }
    }
    
    private void SetupMonsters()
    {
        for (int i = 0; i < monsterSprites.Count; i++)
        {
            GameObject currentMonster = monsterSprites[i];
            GameObject nextMonster = null;

            if (i + 1 < monsterSprites.Count)
            {
                nextMonster = monsterSprites[i+1];
            }
            
            
            D2dRequirements destructableRequirements = currentMonster.AddComponent<D2dRequirements>();
            destructableRequirements.AlphaRatio = true;
            destructableRequirements.AlphaRatioMin = 0f;
            destructableRequirements.AlphaRatioMax = 0.4f;
            destructableRequirements.OnRequirementsMet.AddListener(() =>
            {
                if (nextMonster)
                {
                    nextMonster.GetComponent<D2dDestructibleSprite>().Indestructible = false;
                    FindObjectOfType<CurrencyManager>().MoneyReward(1); //TODO 1 * Round
                }
            });

            D2dRequirements inDestructableRequirements = currentMonster.AddComponent<D2dRequirements>();
            inDestructableRequirements.AlphaRatio = true;
            inDestructableRequirements.AlphaRatioMin = 0.5f;
            inDestructableRequirements.AlphaRatioMax = 1f;
            inDestructableRequirements.OnRequirementsMet.AddListener(() =>
            {
                if (nextMonster) nextMonster.GetComponent<D2dDestructibleSprite>().Indestructible = true;
            });
        }
    }
    
}
