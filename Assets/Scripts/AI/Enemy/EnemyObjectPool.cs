using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPool : MonoBehaviour
{
    public Transform enemyHolder;
    public Transform player;

    public GameObject BatPrefab;
    public List<GameObject> BatList = new List<GameObject>();
    public List<float> BatSpawnTime = new List<float>();
    public List<Vector3> BatSpawnLocation = new List<Vector3>();


    public GameObject DragonPrefab;
    public List<GameObject> DragonList = new List<GameObject>();
    public List<float> DragonSpawnTime = new List<float>();
    public List<Vector3> DragonSpawnLocation = new List<Vector3>();

    public GameObject OrcPrefab;
    public List<GameObject> OrcList = new List<GameObject>();
    public List<float> OrcSpawnTime = new List<float>();
    public List<Vector3> OrcSpawnLocation = new List<Vector3>();

    public GameObject CactusPrefab;
    public List<GameObject> CactusList = new List<GameObject>();
    public List<float> CactusSpawnTime = new List<float>();
    public List<Vector3> CactusSpawnLocation = new List<Vector3>();

    public GameObject FishmanPrefab;
    public List<GameObject> FishmanList = new List<GameObject>();
    public List<float> FishmanSpawnTime = new List<float>();
    public List<Vector3> FishmanSpawnLocation = new List<Vector3>();

    public GameObject BattleBeePrefab;
    public List<GameObject> BattleBeeList = new List<GameObject>();
    public List<float> BattleBeeSpawnTime = new List<float>();
    public List<Vector3> BattleBeeSpawnLocation = new List<Vector3>();

    public GameObject DragonFirePrefab;
    public List<GameObject> DragonFireList = new List<GameObject>();
    public List<float> DragonFireSpawnTime = new List<float>();
    public List<Vector3> DragonFireSpawnLocation = new List<Vector3>();

    int EnemyIDCounter = 0;

    private void Start()
    {
        int difficulty = GameSettingDataSingleton.Instance.DifficultyIndex + 2;
        for (int i = 0; i < 15 * difficulty; i++)
        {
            GameObject Bat = Instantiate(BatPrefab, enemyHolder);
            EnemyAI ai = Bat.GetComponent<EnemyAI>();
            ai.player = player;
            ai.EnemyID = EnemyIDCounter;
            EnemyIDCounter++;
            Bat.SetActive(false);
            BatList.Add(Bat);

            BatSpawnTime.Add(0f);
            Bat.transform.position = new Vector3(Random.Range(-3f, 5f), 0, Random.Range(40f, 80f));
            BatSpawnLocation.Add(Bat.transform.position);
        }

        for (int i = 0; i < 10 * difficulty; i++)
        {
            GameObject Dragon = Instantiate(DragonPrefab, enemyHolder);
            EnemyAI ai = Dragon.GetComponent<EnemyAI>();
            ai.player = player;
            ai.EnemyID = EnemyIDCounter;
            EnemyIDCounter++;
            Dragon.SetActive(false);
            DragonList.Add(Dragon);

            DragonSpawnTime.Add(Random.Range(0f, 10f));
            Dragon.transform.position = new Vector3(30f + Random.Range(-3f, 5f), 0, 40f + Random.Range(40f, 80f));
            DragonSpawnLocation.Add(Dragon.transform.position);
        }

        for (int i = 0; i < 8 * difficulty; i++)
        {
            GameObject Orc = Instantiate(OrcPrefab, enemyHolder);
            EnemyAI ai = Orc.GetComponent<EnemyAI>();
            ai.player = player;
            ai.EnemyID = EnemyIDCounter;
            EnemyIDCounter++;
            Orc.SetActive(false);
            OrcList.Add(Orc);

            OrcSpawnTime.Add(40f);
            Orc.transform.position = new Vector3(-10f + Random.Range(-3f, 5f), 0, -20f + Random.Range(-10f, 10f));
            OrcSpawnLocation.Add(Orc.transform.position);
        }

        for (int i = 0; i < 8 * difficulty; i++)
        {
            GameObject Cactus = Instantiate(CactusPrefab, enemyHolder);
            EnemyAI ai = Cactus.GetComponent<EnemyAI>();
            ai.player = player;
            ai.EnemyID = EnemyIDCounter;
            EnemyIDCounter++;
            Cactus.SetActive(false);
            CactusList.Add(Cactus);

            CactusSpawnTime.Add(50f);
            Cactus.transform.position = new Vector3(10f + Random.Range(-3f, 5f), 0, -15f + Random.Range(-10f, 10f));
            CactusSpawnLocation.Add(Cactus.transform.position);
        }

        for (int i = 0; i < 7 * difficulty; i++)
        {
            GameObject Fishman = Instantiate(FishmanPrefab, enemyHolder);
            EnemyAI ai = Fishman.GetComponent<EnemyAI>();
            ai.player = player;
            ai.EnemyID = EnemyIDCounter;
            EnemyIDCounter++;
            Fishman.SetActive(false);
            FishmanList.Add(Fishman);

            FishmanSpawnTime.Add(60f);
            Fishman.transform.position = new Vector3(0f + Random.Range(-3f, 5f), 0, -35f + Random.Range(-10f, 5f));
            FishmanSpawnLocation.Add(Fishman.transform.position);
        }

        for (int i = 0; i < 5 * difficulty; i++)
        {
            GameObject BattleBee = Instantiate(BattleBeePrefab, enemyHolder);
            EnemyAI ai = BattleBee.GetComponent<EnemyAI>();
            ai.player = player;
            ai.EnemyID = EnemyIDCounter;
            EnemyIDCounter++;
            BattleBee.SetActive(false);
            BattleBeeList.Add(BattleBee);

            BattleBeeSpawnTime.Add(100f);
            BattleBee.transform.position = new Vector3(-50f + Random.Range(-3f, 5f), 0, -35f + Random.Range(-20f, 5f));
            BattleBeeSpawnLocation.Add(BattleBee.transform.position);
        }

        for (int i = 0; i < 1 * difficulty; i++)
        {
            GameObject DragonFire = Instantiate(DragonFirePrefab, enemyHolder);
            EnemyAI ai = DragonFire.GetComponent<EnemyAI>();
            ai.player = player;
            ai.EnemyID = EnemyIDCounter;
            EnemyIDCounter++;
            DragonFire.SetActive(false);
            DragonFireList.Add(DragonFire);

            DragonFireSpawnTime.Add(500f);
            DragonFire.transform.position = new Vector3(55f + Random.Range(-3f, 5f), 0, -15f + Random.Range(-20f, 5f));
            DragonFireSpawnLocation.Add(DragonFire.transform.position);
        }
    }

    private void Update()
    {
        UpdateBatSpawn();
        UpdateDragonSpawn();
        UpdateOrcSpawn();
        UpdateCactusSpawn();
        UpdateFishmanSpawn();
        UpdateBattleBeeSpawn();
        UpdateDragonFireSpawn();
    }

    public GameObject GetBat()
    {
        for(int i = BatList.Count - 1; i >= 0; i--)
        {
            if(BatList[i].activeSelf == false)
            {
                return BatList[i];
            }
        }

        GameObject Bat = Instantiate(BatPrefab, enemyHolder);
        EnemyAI ai = Bat.GetComponent<EnemyAI>();
        ai.player = player;
        ai.EnemyID = EnemyIDCounter;
        EnemyIDCounter++;
        Bat.SetActive(false);
        BatList.Add(Bat);

        return Bat;
    }

    public GameObject GetDragon()
    {
        for (int i = DragonList.Count - 1; i >= 0; i--)
        {
            if (DragonList[i].activeSelf == false)
            {
                return DragonList[i];
            }
        }

        GameObject Dragon = Instantiate(DragonPrefab, enemyHolder);
        EnemyAI ai = Dragon.GetComponent<EnemyAI>();
        ai.player = player;
        ai.EnemyID = EnemyIDCounter;
        EnemyIDCounter++;
        Dragon.SetActive(false);
        DragonList.Add(Dragon);

        return Dragon;
    }

    public GameObject GetOrc()
    {
        for (int i = OrcList.Count - 1; i >= 0; i--)
        {
            if (OrcList[i].activeSelf == false)
            {
                return OrcList[i];
            }
        }

        GameObject Orc = Instantiate(OrcPrefab, enemyHolder);
        EnemyAI ai = Orc.GetComponent<EnemyAI>();
        ai.player = player;
        ai.EnemyID = EnemyIDCounter;
        EnemyIDCounter++;
        Orc.SetActive(false);
        OrcList.Add(Orc);

        return Orc;
    }

    public GameObject GetCactus()
    {
        for (int i = CactusList.Count - 1; i >= 0; i--)
        {
            if (CactusList[i].activeSelf == false)
            {
                return CactusList[i];
            }
        }

        GameObject Cactus = Instantiate(CactusPrefab, enemyHolder);
        EnemyAI ai = Cactus.GetComponent<EnemyAI>();
        ai.player = player;
        ai.EnemyID = EnemyIDCounter;
        EnemyIDCounter++;
        Cactus.SetActive(false);
        CactusList.Add(Cactus);

        return Cactus;
    }

    public GameObject GetFishman()
    {
        for (int i = FishmanList.Count - 1; i >= 0; i--)
        {
            if (FishmanList[i].activeSelf == false)
            {
                return FishmanList[i];
            }
        }

        GameObject Fishman = Instantiate(FishmanPrefab, enemyHolder);
        EnemyAI ai = Fishman.GetComponent<EnemyAI>();
        ai.player = player;
        ai.EnemyID = EnemyIDCounter;
        EnemyIDCounter++;
        Fishman.SetActive(false);
        FishmanList.Add(Fishman);

        return Fishman;
    }

    public GameObject GetBattleBee()
    {
        for (int i = BattleBeeList.Count - 1; i >= 0; i--)
        {
            if (BattleBeeList[i].activeSelf == false)
            {
                return BattleBeeList[i];
            }
        }

        GameObject BattleBee = Instantiate(BattleBeePrefab, enemyHolder);
        EnemyAI ai = BattleBee.GetComponent<EnemyAI>();
        ai.player = player;
        ai.EnemyID = EnemyIDCounter;
        EnemyIDCounter++;
        BattleBee.SetActive(false);
        BattleBeeList.Add(BattleBee);

        return BattleBee;
    }

    public GameObject GetDragonFire()
    {
        for (int i = DragonFireList.Count - 1; i >= 0; i--)
        {
            if (DragonFireList[i].activeSelf == false)
            {
                return DragonFireList[i];
            }
        }

        GameObject DragonFire = Instantiate(DragonFirePrefab, enemyHolder);
        EnemyAI ai = DragonFire.GetComponent<EnemyAI>();
        ai.player = player;
        ai.EnemyID = EnemyIDCounter;
        EnemyIDCounter++;
        DragonFire.SetActive(false);
        DragonFireList.Add(DragonFire);

        return DragonFire;
    }

    
    public void UpdateBatSpawn()
    {
        for (int i = BatList.Count - 1; i >= 0; i--)
        {
            if (BatList[i].activeSelf == false)
            {
                BatSpawnTime[i] += Time.deltaTime;
                if(BatSpawnTime[i] > 20f)
                {
                    GameObject Bat = BatList[i];
                    Bat.SetActive(true);
                    EnemyAI ai = Bat.GetComponent<EnemyAI>();
                    ai.enabled = true;
                    ai.player = player;
                    
                    ai.needReSpawn = true;
                    Bat.transform.position = BatSpawnLocation[i];
                    BatSpawnTime[i] = 0;
                }

            }
        }
    }

    public void UpdateDragonSpawn()
    {
        for (int i = DragonList.Count - 1; i >= 0; i--)
        {
            if (DragonList[i].activeSelf == false)
            {
                DragonSpawnTime[i] += Time.deltaTime;
                if (DragonSpawnTime[i] > 30f)
                {
                    GameObject Dragon = DragonList[i];
                    Dragon.SetActive(true);
                    EnemyAI ai = Dragon.GetComponent<EnemyAI>();
                    ai.enabled = true;
                    ai.player = player;

                    ai.needReSpawn = true;
                    Dragon.transform.position = DragonSpawnLocation[i];
                    DragonSpawnTime[i] = 0;
                }

            }
        }
    }

    public void UpdateOrcSpawn()
    {
        for (int i = OrcList.Count - 1; i >= 0; i--)
        {
            if (OrcList[i].activeSelf == false)
            {
                OrcSpawnTime[i] += Time.deltaTime;
                if (OrcSpawnTime[i] > 50f)
                {
                    GameObject Orc = OrcList[i];
                    Orc.SetActive(true);
                    EnemyAI ai = Orc.GetComponent<EnemyAI>();
                    ai.enabled = true;
                    ai.player = player;

                    ai.needReSpawn = true;
                    Orc.transform.position = OrcSpawnLocation[i];
                    OrcSpawnTime[i] = 0;
                }

            }
        }
    }

    public void UpdateCactusSpawn()
    {
        for (int i = CactusList.Count - 1; i >= 0; i--)
        {
            if (CactusList[i].activeSelf == false)
            {
                CactusSpawnTime[i] += Time.deltaTime;
                if (CactusSpawnTime[i] > 50f)
                {
                    GameObject Cactus = CactusList[i];
                    Cactus.SetActive(true);
                    EnemyAI ai = Cactus.GetComponent<EnemyAI>();
                    ai.enabled = true;
                    ai.player = player;

                    ai.needReSpawn = true;
                    Cactus.transform.position = CactusSpawnLocation[i];
                    CactusSpawnTime[i] = 0;
                }

            }
        }
    }

    public void UpdateFishmanSpawn()
    {
        for (int i = FishmanList.Count - 1; i >= 0; i--)
        {
            if (FishmanList[i].activeSelf == false)
            {
                FishmanSpawnTime[i] += Time.deltaTime;
                if (FishmanSpawnTime[i] > 20f)
                {
                    GameObject Fishman = FishmanList[i];
                    Fishman.SetActive(true);
                    EnemyAI ai = Fishman.GetComponent<EnemyAI>();
                    ai.enabled = true;
                    ai.player = player;

                    ai.needReSpawn = true;
                    Fishman.transform.position = FishmanSpawnLocation[i];
                    FishmanSpawnTime[i] = 0;
                }

            }
        }
    }

    public void UpdateBattleBeeSpawn()
    {
        for (int i = BattleBeeList.Count - 1; i >= 0; i--)
        {
            if (BattleBeeList[i].activeSelf == false)
            {
                BattleBeeSpawnTime[i] += Time.deltaTime;
                if (BattleBeeSpawnTime[i] > 100f)
                {
                    GameObject BattleBee = BattleBeeList[i];
                    BattleBee.SetActive(true);
                    EnemyAI ai = BattleBee.GetComponent<EnemyAI>();
                    ai.enabled = true;
                    ai.player = player;

                    ai.needReSpawn = true;
                    BattleBee.transform.position = BattleBeeSpawnLocation[i];
                    BattleBeeSpawnTime[i] = 0;
                }

            }
        }
    }

    public void UpdateDragonFireSpawn()
    {
        for (int i = DragonFireList.Count - 1; i >= 0; i--)
        {
            if (DragonFireList[i].activeSelf == false)
            {
                DragonFireSpawnTime[i] += Time.deltaTime;
                if (DragonFireSpawnTime[i] > 200f)
                {
                    GameObject DragonFire = DragonFireList[i];
                    DragonFire.SetActive(true);
                    EnemyAI ai = DragonFire.GetComponent<EnemyAI>();
                    ai.enabled = true;
                    ai.player = player;

                    ai.needReSpawn = true;
                    DragonFire.transform.position = DragonFireSpawnLocation[i];
                    DragonFireSpawnTime[i] = 0;
                }

            }
        }
    }
}
