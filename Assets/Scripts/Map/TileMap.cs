using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [Header("Map Objects")]
    public GameObject TilePrefab;
    public GameObject ForestPrefab;

    [Header("Map Materials")]
    public Material MaterialMountain;
    public Material MaterialHill;
    public Material MaterialPlains;
    public Material MaterialRiver;

    [Header("Map Meshes")]
    public Mesh MeshMountain;
    public Mesh MeshHill;
    public Mesh MeshPlains;
    public Mesh MeshRiver;

    [Header("Other GameObjects")]
    public EndScreen endScreen;
    public GameMaster gameMaster;
    public Camera mainCamera;

    [Header("Player")]
    public GameObject playerGO;
    private Player player;

    [Header("Other GameObjects")]
    float tan60 = 0.32f;             //0.32004038938f
    float cameraStartHeight = 5f;    //pozicija kamere kod kreiranja mape

    [Header("NoiseMap elements")]
    public float mapScale = 2.1f;
    private int mapRings;
    private int mapSize;
    private Vector2 seedOffset;

    [Header("Description Box")]
    public DescriptionScript descScript;

    //tileovi i tile-GO linkovi
    private Tile[,] tiles;
    public Dictionary<GameObject, Tile> gameObjectToTile;

    //lista unita
    private List<Unit> units = new List<Unit>();

    //seed
    private System.Random seededRND;

    //vrijednosti za rotaciju polja za razbijanje repeticije i vector3 za unite na Hill polju
    private float[] tileRandomRotation = { 0f, 60f, 120f, 180f, 240f, 300f };
    private Vector3 addedelevation = new Vector3(0, 0.3f, 0);

    //vrijednosti za generiranje polja sa različitim uzvisinama (0f - 1f)
    private float mountainTreshold = 0.65f;
    private float hillTreshold = 0.55f;
    private float riverTreshold = 0.3f;
    private float forestTreshold = 0.55f;

    //varijable korištene za kreiranje neprijatenja na mapi
    private int[] monsterSpawnChance = { 200, 60, 20, 4 };
    private int[] monsterSpawnChanceAddition = { 0, 35, 45, 49 };

    //pathfinding
    private Node[,] graph;
    private bool isMovementEnabled = true;
    private float turnTime = 0.1f;

    //portal
    private Tile portalTile;

    //items
    string[] items = { "None", "Heal1", "Heal2", "Heal3", "Damage1", "Damage2", "MaxHP", "Reveal", "RevealAll" };

    void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        int seed = GameValues.mapSeed;


        //TESTNI PODACI
        //seed = Random.Range(0, 99999999);
        //seed = 55555555;
        //GameValues.mapRings = 6;



        seededRND = new System.Random(seed);
        Debug.Log("Seed: " + seed);
        seedOffset = new Vector2(seed % 10000, seed / 10000);
        mapRings = GameValues.mapRings;
        mapSize = mapRings * 2 + 1;

        GenerateTiles();
        GameObject.Find("Main Camera").GetComponent<CameraAndControlsScript>().SetCameraRange(GetRange());

        GenerateComplexitySeed();
        GenerateForestSeed();
        GeneratePortal();

        GenerateGraph();

        UpdateTiles();
        HideTiles();


        SetPlayer();
        RevealTilesAroundPlayer();

        PopulateWithMonsters();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            player.TakeDamage(50);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (Tile t in tiles)
            {
                if (t != null)
                {
                    t.tileGO.SetActive(true);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseItem(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseItem(4);
            Debug.Log(player.ReturnDamage());
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UseItem(5);
            Debug.Log(player.ReturnDamage());
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UseItem(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            UseItem(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            UseItem(8);
        }
    }

    private void GenerateTiles()
    {
        tiles = new Tile[mapSize, mapSize];
        gameObjectToTile = new Dictionary<GameObject, Tile>();

        for (int q = 0; q < mapSize; q++)
        {
            //int r1 = mapRings - q;
            //int r1 = Mathf.Max(0, q - mapSize);
            //int r2 = Mathf.Min(q, mapSize);
            int r1 = Mathf.Max(0, mapRings - q); ;
            int r2 = Mathf.Min((mapSize - q) + mapRings, mapSize);
            for (int r = r1; r < r2; r++)
            {
                //stvaranje novog tilea i instanciranje
                Tile t = new Tile(this, q, r);
                //GameObject tileGO = Instantiate(TilePrefab, t.Position(), Quaternion.Euler(0, RandomEulerRotationOf60(), 0), this.transform);
                GameObject tileGO = Instantiate(TilePrefab, t.Position(), Quaternion.identity, this.transform);
                tileGO.transform.GetChild(0).transform.rotation = Quaternion.Euler(-90, RandomEulerRotationOf60(), 0);
                tileGO.name = string.Format("{0, -4}, {1, -4} (GO)", q, r);
                tileGO.tag = "Tile";
                //tileGO.name = q + ", " + r + "(GO)";

                //spremanje tilea
                tiles[q, r] = t;
                tiles[q, r].tileGO = tileGO;
                gameObjectToTile[tileGO] = t;
            }
        }
    }

    private void GenerateComplexitySeed()
    {
        float[,] complexity = NoiseGenerator.GenerateHeightMap(mapSize, mapScale, seedOffset);
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (tiles[x, y] != null)
                {
                    tiles[x, y].elevation = complexity[x, y];
                }
            }
        }
    }

    private void GenerateForestSeed()
    {
        Vector2 forestOffset = seedOffset + new Vector2(200, 200);
        float[,] forests = NoiseGenerator.GenerateHeightMap(mapSize, mapScale, forestOffset);
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (tiles[x, y] != null)
                {
                    if (forests[x, y] >= forestTreshold && tiles[x, y].elevation < mountainTreshold)
                    {
                        tiles[x, y].hasForest = true;
                    }
                }
            }
        }
    }

    private void UpdateTiles()
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile t = tiles[x, y];
                if (t != null)
                {
                    GameObject tileGO = t.tileGO;
                    MeshRenderer mr = tileGO.GetComponentInChildren<MeshRenderer>();
                    MeshFilter mf = tileGO.GetComponentInChildren<MeshFilter>();

                    // Planina
                    if (t.elevation >= mountainTreshold)
                    {
                        mr.material = MaterialMountain;
                        mf.mesh = MeshMountain;
                        //mr.material.mainTexture = TextureMountain;

                    }
                    // Brijeg
                    else if (t.elevation < mountainTreshold && t.elevation >= hillTreshold)
                    {
                        mr.material = MaterialHill;
                        mf.mesh = MeshHill;
                        //mr.material.mainTexture = TextureHill;
                    }
                    // Rijeka
                    else if (t.elevation < riverTreshold)
                    {
                        mr.material = MaterialRiver;
                        mf.mesh = MeshRiver;
                        //mr.material.mainTexture = TextureRiver;
                    }
                    // Ravnica
                    else
                    {
                        mr.material = MaterialPlains;
                        mf.mesh = MeshPlains;

                        // nope
                        //mr.material.mainTexture = TexturePlains;
                        // pokušaj crtanja granice forum.unity.com/threads/combining-merging-multiple-alpha-textures-into-a-new-one.858136/
                        /*
                        Color[] colors1 = TexturePlains.GetPixels();
                        Color[] colors2 = BorderTest.GetPixels();
                        int numColors = colors1.Length;
                        Color[] colorsCombined = new Color[numColors];
                        for (int i = 0; i < numColors; i++)
                        {
                            colorsCombined[i] = colors1[i] * colors2[i];đ
                        }
                        Texture2D comboTexture = new Texture2D(512, 512);
                        comboTexture.SetPixels(colorsCombined);
                        comboTexture.Apply();
                        mr.material.EnableKeyword("_MainTex");
                        mr.material.SetTexture("_MainTex", comboTexture);
                        */
                    }

                    if (t.hasForest == true)
                    {
                        //mr.material =
                        //GameObject tileForest = (GameObject)Instantiate(ForestPrefab, t.position(), Quaternion.identity, tileToGameObjectMap[t].transform);
                        //MeshRenderer mr_f = tileForest.GetComponentInChildren<MeshRenderer>();
                        //MeshFilter mf_f = tileForest.GetComponentInChildren<MeshFilter>();

                        //mr_f.material = MaterialTrees;
                        //mf_f.mesh = MeshTrees;

                        //if (t.elevation > mountainTreshold && t.HasForest == true)
                        //{
                        //    t.HasForest = false;
                        //}
                        //else 
                        if (t.elevation > hillTreshold)
                        {
                            GameObject.Instantiate(ForestPrefab, tileGO.transform.position + addedelevation, Quaternion.Euler(-90, Random.Range(0, 360), 0), tileGO.transform);
                        }
                        else
                        {
                            GameObject.Instantiate(ForestPrefab, tileGO.transform.position, Quaternion.Euler(-90, Random.Range(0, 360), 0), tileGO.transform);
                        }
                    }
                }
            }
        }
    }

    private void GeneratePortal()
    {
        //postavljanje portala
        Tile tile;
        do
        {
            tile = GetExistingTileBySeed();
        }
        while (tile.elevation >= mountainTreshold);
        tile.hasPortal = true;
        portalTile = tile;
        Instantiate(Resources.Load("Prefabs/POI/Portal"), tile.tileGO.transform);
        tile.hasForest = false;
    }

    private void GenerateGraph()
    {
        //inicijaliziranje arrayja
        graph = new Node[mapSize, mapSize];

        //inicijaliziranje svakog čvora (node)
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }

        //spajanje čvorova
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (tiles[x, y] != null)
                {
                    //moramo za svaki susjed provjeriti da li postoji (jer u 2d kvadratnom gridu stvaramo mapu u obliku pravilnog šesterokuta)
                    //također provjeravamo da li je susjed planina, ako jest onda ga ne stavljamo kao susjeda
                    //ako provjevamo tile koji je planina onda je svejedno jer se neprijatelj ne može u ni kojem slučaju naći na tom polju
                    if (y > 0 && tiles[x, y - 1] != null && tiles[x, y - 1].elevation < mountainTreshold)
                    {
                        graph[x, y].neighbours.Add(graph[x, y - 1]);
                    }
                    if (x < mapSize - 1 && y > 0 && tiles[x + 1, y - 1] != null && tiles[x + 1, y - 1].elevation < mountainTreshold)
                    {
                        graph[x, y].neighbours.Add(graph[x + 1, y - 1]);
                    }
                    if (x < mapSize - 1 && tiles[x + 1, y] != null && tiles[x + 1, y].elevation < mountainTreshold)
                    {
                        graph[x, y].neighbours.Add(graph[x + 1, y]);
                    }
                    if (y < mapSize - 1 && tiles[x, y + 1] != null && tiles[x, y + 1].elevation < mountainTreshold)
                    {
                        graph[x, y].neighbours.Add(graph[x, y + 1]);
                    }
                    if (x > 0 && y < mapSize - 1 && tiles[x - 1, y + 1] != null && tiles[x - 1, y + 1].elevation < mountainTreshold)
                    {
                        graph[x, y].neighbours.Add(graph[x - 1, y + 1]);
                    }
                    if (x > 0 && tiles[x - 1, y] != null && tiles[x - 1, y].elevation < mountainTreshold)
                    {
                        graph[x, y].neighbours.Add(graph[x - 1, y]);
                    }
                }
            }
        }
    }

    private void GeneratePath(Unit unit)
    {
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();

        Node source = graph[unit.tile.Q, unit.tile.R];
        Node target = graph[player.tile.Q, player.tile.R];
        dist[source] = 0;
        prev[source] = null;

        //inicijaliziranje svih nodeova na infinite
        foreach (Node v in graph)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            unvisited.Add(v);
        }

        while (unvisited.Count > 0)
        {
            Node u = null;
            foreach (Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            if (u == target)
            {
                break;
            }

            unvisited.Remove(u);

            foreach (Node v in u.neighbours)
            {
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + CostToEnterTile(v.x, v.y);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        //ili smo pronašli ili nema rute do cilja
        if (prev[target] == null)
        {
            //nema rute izmedju ishodišnog i odredišnog polja
            return;
        }

        List<Node> currentPath = new List<Node>();
        Node curr = target;

        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }

        currentPath.Reverse();

        //unit.currentPath = currentPath;
        unit.targetTile = tiles[currentPath[1].x, currentPath[1].y];
    }

    private float CostToEnterTile(int q, int r)
    {
        if (tiles[q, r].unit != null)
        {
            return float.PositiveInfinity;
        }
        else
        {
            return 1f;
        }
    }

    // Nasumično izabran broj [0, 60, 120, 180, 240, 300]
    private float RandomEulerRotationOf60()
    {
        //return tileRandomRotation[rnd.Next(0, 5)];
        return tileRandomRotation[Random.Range(0, 5)];
    }

    private Tile GetExistingTileBySeed()
    {
        int x, y;
        do
        {
            x = seededRND.Next(0, mapSize);
            y = seededRND.Next(0, mapSize);
        }
        while (tiles[x, y] == null);
        return tiles[x, y];
    }

    private bool IsNeighbour(Tile tile1, Tile tile2)
    {
        if (tile1 != null || tile2 != null)
        {
            //+1 -1
            //+1 0
            //0 +1
            //-1 +1
            //-1 0
            //0 -1
            if (tile1.Q + 1 == tile2.Q && tile1.R - 1 == tile2.R ||
                tile1.Q + 1 == tile2.Q && tile1.R == tile2.R ||
                tile1.Q == tile2.Q && tile1.R + 1 == tile2.R ||
                tile1.Q - 1 == tile2.Q && tile1.R + 1 == tile2.R ||
                tile1.Q - 1 == tile2.Q && tile1.R == tile2.R ||
                tile1.Q == tile2.Q && tile1.R - 1 == tile2.R)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void SetPlayer()
    {
        player = playerGO.GetComponent<Player>();
        Tile setTile;
        do
        {
            setTile = GetExistingTileBySeed();
        }
        while (setTile.elevation > mountainTreshold || TooClose(setTile, portalTile));
        //while (setTile.elevation > mountainTreshold);

        playerGO.transform.position = setTile.tileGO.transform.position;
        player.tile = setTile;

        // postavi kameru kod igrača
        if (player.tile.elevation >= hillTreshold) playerGO.transform.position += addedelevation;
        mainCamera.transform.position = new Vector3(playerGO.transform.position.x, cameraStartHeight, player.transform.position.z + (cameraStartHeight * 2 * -tan60));
    }

    public void MovePlayer(GameObject targetGO)
    {
        if (isMovementEnabled)
        {
            Tile targetTile = gameObjectToTile[targetGO];

            if (IsNeighbour(targetTile, player.tile))
            {
                if (targetTile.unit == null)
                {
                    if (targetTile.elevation < mountainTreshold)
                    {
                        gameMaster.WalkingSound();
                        string debugOutput = "";
                        if (player.tile != null) debugOutput += "Player movement: " + player.tile.Q + ", " + player.tile.R;

                        playerGO.transform.position = targetGO.transform.position;
                        player.tile = targetTile;
                        if (player.tile.elevation >= hillTreshold) playerGO.transform.position += addedelevation;

                        if (player.tile != null) debugOutput += "    --->    " + player.tile.Q + ", " + player.tile.R;
                        Debug.Log(debugOutput);
                        GameValues.movesTotal++;

                        RevealTilesAroundPlayer();
                        NextTurn();
                    }
                }
                else
                {
                    AttackTile(targetTile);
                }
            }

            CheckPortalAndItem();
        }
    }

    private void CheckPortalAndItem()
    {
        Tile playerTile = player.tile;
        if (playerTile.itemID != 0)
        {
            UseItem(playerTile.itemID);
            playerTile.itemID = 0;
        }
        if (playerTile.hasPortal == true)
        {
            gameMaster.NextMap();
        }
    }

    private void NextTurn()
    {
        StartCoroutine(MoveUnits());
    }

    private void AttackTile(Tile targetTile)
    {
        descScript.AddLine("You hit " + targetTile.unit.ReturnName() + " for " + player.ReturnDamage() + " damage!");
        targetTile.unit.TakeDamage(player.ReturnDamage());
        GameValues.damageDealtTotal += player.ReturnDamage();

        new WaitForSecondsRealtime(0.2f);
        NextTurn();
    }

    private void PopulateWithMonsters()
    {
        int numOfMonsters = Mathf.FloorToInt(Mathf.Pow(((10 * mapRings) / 33), 2) + 4) + seededRND.Next(3);
        //DODATI PROVJERU DA LI SE NEPRIJATELJ SPAWNA U BLIZINI IGRAČA
        for (int i = 0; i < numOfMonsters; i++)
        {
            Tile tile;
            do
            {
                tile = GetExistingTileBySeed();
            }
            //while (tile.elevation >= mountainTreshold || TooCloseToPlayer(tile));
            while (tile.elevation >= mountainTreshold || TooClose(tile, player.tile)) ;

            Vector3 pos = tile.tileGO.transform.position;
            if (tile.elevation >= hillTreshold)
            {
                pos += addedelevation;
            }

            switch (ChooseEnemy())
            {
                case 0:
                    {
                        //knight
                        GameObject newUnitGO = (GameObject)Instantiate(Resources.Load("Prefabs/Enemy/Knight"), pos, Quaternion.Euler(0, -110, 0), tile.tileGO.transform);
                        Unit newUnit = new UnitKnight(tile, newUnitGO);
                        tile.unit = newUnit;
                        units.Add(newUnit);
                        break;
                    }
                case 1:
                    {
                        //spider
                        GameObject newUnitGO = (GameObject)Instantiate(Resources.Load("Prefabs/Enemy/Spider"), pos, Quaternion.Euler(0, -110, 0), tile.tileGO.transform);
                        Unit newUnit = new UnitSpider(tile, newUnitGO);
                        tile.unit = newUnit;
                        units.Add(newUnit);
                        break;
                    }
                case 2:
                    {
                        //golem
                        GameObject newUnitGO = (GameObject)Instantiate(Resources.Load("Prefabs/Enemy/Golem"), pos, Quaternion.Euler(-90, -110, 0), tile.tileGO.transform);
                        Unit newUnit = new UnitGolem(tile, newUnitGO);
                        tile.unit = newUnit;
                        units.Add(newUnit);
                        break;
                    }
                default:
                    {
                        //demon
                        GameObject newUnitGO = (GameObject)Instantiate(Resources.Load("Prefabs/Enemy/Demon"), pos, Quaternion.Euler(-90, -110, 0), tile.tileGO.transform);
                        Unit newUnit = new UnitDemon(tile, newUnitGO);
                        tile.unit = newUnit;
                        units.Add(newUnit);
                        break;
                    }
            }
        }

        //ispis u konzolu svih neprijatelja i njihovih pozicija
        int num = 0;
        string listUnitsT = "";
        foreach (Unit u in units)
        {
            num++;
            listUnitsT += " | " + u.ReturnName() + "(" + u.tile.Q + ", " + u.tile.R + ")";
        }
        Debug.Log(listUnitsT + " || Total: " + num);
    }

    private bool TooClose(Tile tile, Tile toWhat)
    {
        //naveća apsolutna udaljenost između igračevog polja i ciljanog polja treba biti 2. odnosno onoliko tileova koliko treba biti prazno. ako je unutar, znači da je odabran tile preblizu igrača
        if (Mathf.Abs(toWhat.Q - tile.Q) > 2 || Mathf.Abs(toWhat.R - tile.R) > 2 || Mathf.Abs(toWhat.S - tile.S) > 2)
        {
            return false;
        }

        return true;
    }

    private int ChooseEnemy()
    {
        int choice = 0;
        int sumTotal = 0;
        int multiplier = GameValues.levelsTotal <= 4 ? GameValues.levelsTotal : 4;
        int[] newValues = { 0, 0, 0, 0 };
        for (int i = 0; i < monsterSpawnChance.Length; i++)
        {
            newValues[i] = monsterSpawnChance[i] + (multiplier * monsterSpawnChanceAddition[i]);
        }

        for (int i = 0; i < monsterSpawnChance.Length; i++)
        {
            sumTotal += newValues[i];
        }
        int rndNum = seededRND.Next(0, sumTotal);

        while (rndNum > newValues[choice])
        {
            rndNum -= newValues[choice];
            choice++;
        }
        return choice;
    }

    public void DestroyGO(GameObject GO, Unit u)
    {
        CreateItem(u.tile);
        descScript.AddLine(u.ReturnName() + " was defeated!");
        units.Remove(u);
        Destroy(GO);
    }

    private Vector3 GetRange()
    {
        float minX = 99999;
        float maxX = -99999;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Tile t = tiles[x, y];
                if (t == null) continue;
                if (t.tileGO.transform.position.x > maxX)
                {
                    maxX = t.tileGO.transform.position.x;
                }
                if (t.tileGO.transform.position.x < minX)
                {
                    minX = t.tileGO.transform.position.x;
                }
            }
        }
        float maxZ = tiles[1, mapSize - 1].tileGO.transform.position.z;
        return new Vector3(minX, maxX, maxZ);
    }

    private IEnumerator MoveUnits()
    {
        DisableAttackMove();
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursor"), Vector2.zero, CursorMode.Auto);
        foreach (Unit u in units)
        {
            if (Mathf.Abs(u.tile.Q - player.tile.Q) <= 3 && Mathf.Abs(u.tile.R - player.tile.R) <= 3 && Mathf.Abs(u.tile.S - player.tile.S) <= 3)
            {
                if (IsNeighbour(u.tile, player.tile))
                {
                    Debug.Log(u.ReturnName() + " is attacking player!");
                    int damage = u.DoDamage();
                    player.TakeDamage(damage);
                    descScript.AddLine(u.ReturnName() + " hit you for " + damage + " damage!");
                    gameMaster.GenericHit();

                    yield return new WaitForSeconds(turnTime);
                }
                else
                {
                    Debug.Log(u.ReturnName() + " is moving towards the player!");
                    GeneratePath(u);

                    Vector3 currPos = u.unitGO.transform.position;
                    if (u.targetTile == null) Debug.Log("null");
                    Vector3 newPos = u.targetTile.tileGO.transform.position;
                    if (u.targetTile.elevation >= hillTreshold)
                    {
                        newPos += addedelevation;
                    }

                    float lerpTime = 0;

                    while (lerpTime < turnTime)
                    {
                        u.unitGO.transform.position = Vector3.Lerp(currPos, newPos, (lerpTime / turnTime));
                        lerpTime += Time.deltaTime;
                        yield return null;
                    }
                    u.unitGO.transform.position = newPos;
                    u.unitGO.transform.SetParent(u.targetTile.tileGO.transform);

                    u.tile.unit = null;     //staro polje
                    u.tile = u.targetTile;  //postavljanje novog polja
                    u.tile.unit = u;        //novo polje
                    u.targetTile = null;    //brisanje ciljanog polja

                    yield return null;
                }
            }
            else
            {
                Debug.Log(u.ReturnName() + " wanders around..");
                do
                {
                    u.targetTile = GetRandomNeighbourSeed(u.tile);
                }
                while (u.targetTile.elevation >= mountainTreshold);

                Vector3 currPos = u.unitGO.transform.position;
                Vector3 newPos = u.targetTile.tileGO.transform.position;
                if (u.targetTile.elevation >= hillTreshold)
                {
                    newPos += addedelevation;
                }

                float lerpTime = 0;

                while (lerpTime < turnTime)
                {
                    u.unitGO.transform.position = Vector3.Lerp(currPos, newPos, (lerpTime / turnTime));
                    lerpTime += Time.deltaTime;
                    yield return null;
                }
                u.unitGO.transform.position = newPos;
                u.unitGO.transform.SetParent(u.targetTile.tileGO.transform);

                u.tile.unit = null;     //staro polje
                u.tile = u.targetTile;  //postavljanje novog polja
                u.tile.unit = u;        //novo polje
                u.targetTile = null;    //brisanje ciljanog polja

                yield return null;
            }

            //Debug.Log(u.tile.Q + ", " + u.tile.R);
            //Debug.Log(u.targetTile.Q + ", " + u.targetTile.R);
        }
        Debug.Log("------------------------------------------------------------------------");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        EnableAttackMove();
    }

    private Tile GetRandomNeighbourSeed(Tile t)
    {
        Tile returnTile = null;
        int loc = seededRND.Next(1, 7);
        switch (loc)
        {
            case 1:
                {
                    //Debug.Log(t.Q + ", " + t.R);
                    if (t.R - 1 < 0) break;
                    returnTile = tiles[t.Q, t.R - 1];
                    break;
                }
            case 2:
                {
                    //Debug.Log(t.Q + ", " + t.R);
                    if (t.Q + 1 >= mapSize || t.R - 1 < 0) break;
                    returnTile = tiles[t.Q, t.R - 1];
                    break;
                }
            case 3:
                {
                    //Debug.Log(t.Q + ", " + t.R);
                    if (t.Q + 1 >= mapSize) break;
                    returnTile = tiles[t.Q + 1, t.R];
                    break;
                }
            case 4:
                {
                    //Debug.Log(t.Q + ", " + t.R);
                    if (t.R + 1 >= mapSize) break;
                    returnTile = tiles[t.Q, t.R + 1];
                    break;
                }
            case 5:
                {
                    //Debug.Log(t.Q + ", " + t.R);
                    if (t.Q - 1 < 0 || t.R + 1 >= mapSize) break;
                    returnTile = tiles[t.Q - 1, t.R + 1];
                    break;
                }
            case 6:
                {
                    //Debug.Log(t.Q + ", " + t.R);
                    if (t.Q - 1 < 0) break;
                    returnTile = tiles[t.Q - 1, t.R];
                    break;
                }
        }

        if (returnTile == null || returnTile.unit != null) return t;
        else return returnTile;
    }

    public void EnableAttackMove()
    {
        isMovementEnabled = true;
        Debug.Log("enabled");
    }

    public void DisableAttackMove()
    {
        isMovementEnabled = false;
        Debug.Log("disabled");
    }

    public void PlayerDeath()
    {
        UI_Script uiscript = GameObject.Find("Canvas").GetComponent<UI_Script>();
        uiscript.DisableControls();
        endScreen.ShowEndScreen();
    }

    private void HideTiles()
    {
        foreach (Tile t in tiles)
        {
            if (t != null) t.tileGO.SetActive(false);
        }
    }

    private void RevealTilesAroundPlayer()
    {
        foreach (Tile t in tiles)
        {
            if (t != null && Mathf.Abs(t.Q - player.tile.Q) <= 2 && Mathf.Abs(t.R - player.tile.R) <= 2 && Mathf.Abs(t.S - player.tile.S) <= 2)
            {
                t.tileGO.SetActive(true);
            }
        }
    }

    private void CreateItem(Tile t)
    {
        if (t.itemID == 0)
        {
            GameObject item = (GameObject)Instantiate(Resources.Load("Prefabs/Item/Item"), t.tileGO.transform);
            int itemID = seededRND.Next(1, items.Length);

            t.itemID = itemID;
            Debug.Log(t.Q + ", " + t.R);
            Debug.Log(t.tileGO.transform.position);
            item.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Items/" + items[itemID]);
            item.transform.position += new Vector3(0, 1.25f, 0);

            Color32[] texColors = item.GetComponent<SpriteRenderer>().sprite.texture.GetPixels32();
            int total = texColors.Length;
            float r = 0;
            float g = 0;
            float b = 0;

            for (int i = 0; i < total; i++)
            {
                r += texColors[i].r;
                g += texColors[i].g;
                b += texColors[i].b;
            }
            item.GetComponentInChildren<Light>().color = new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 0);
            item.transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void UseItem(int item)
    {
        //"None", "Heal1", "Heal2", "Heal3", "Damage1", "Damage2", "MaxHP", "Reveal", "RevealAll"
        gameMaster.PickupSound();
        switch (item)
        {
            case 1:
                {
                    //Heal1
                    player.Heal(12);
                    descScript.AddLine("Healed for 12 hp!");
                    break;
                }
            case 2:
                {
                    //Heal2
                    player.Heal(33);
                    descScript.AddLine("Healed for 33 hp!");
                    break;
                }
            case 3:
                {
                    //Heal3
                    player.Heal(100);
                    descScript.AddLine("Healed for 100 hp!");
                    break;
                }
            case 4:
                {
                    //Damage1
                    player.IncreaseDamage(5);
                    descScript.AddLine("Damage increased by 5!");
                    break;
                }
            case 5:
                {
                    //Damage2
                    player.IncreaseDamage(10);
                    descScript.AddLine("Damage increased by 10!");
                    break;
                }
            case 6:
                {
                    //MaxHP
                    player.IncreaseMaxHealth(15);
                    descScript.AddLine("Maximum health increased by 15! You now have " + player.ReturnHealth() + "/" + player.ReturnMaxHealth());
                    break;
                }
            case 7:
                {
                    //Reveal
                    portalTile.tileGO.SetActive(true);
                    descScript.AddLine("Location of the portal has been revealed!");
                    break;
                }
            case 8:
                {
                    //RevealAll
                    foreach (Tile t in tiles)
                    {
                        if (t != null) t.tileGO.SetActive(true);
                    }
                    descScript.AddLine("You can see the whole map!");
                    break;
                }
        }

        Destroy(player.tile.tileGO.transform.Find("Item(Clone)").gameObject);
        player.tile.itemID = 0;
    }

    //public void OldMapGen()
    //{
    //    //postavljanje rijeka
    //    GenerateComplexityRandom(-hillTreshold, 20);
    //    //postavljanje brežuljka
    //    GenerateComplexityRandom(0.4f, 40);
    //    //postavljanje planina
    //    GenerateComplexityRandom(1f, 10);
    //    //postavljanje šume
    //    GenerateForestRandom(200);
    //}

    //public void GenerateTilesCenteredOnZeroZero()
    //{
    //    tiles = new Tile[mapSize, mapSize];
    //    tileToGameObject = new Dictionary<Tile, GameObject>();
    //    gameObjectToTile = new Dictionary<GameObject, Tile>();

    //    for (int q = -mapRings; q <= mapRings; q++)
    //    {
    //        int r1 = Mathf.Max(-mapRings, -q - mapRings);
    //        int r2 = Mathf.Min(mapRings, -q + mapRings);

    //        for (int r = r1; r <= r2; r++)
    //        {
    //            //stvaranje novog tilea i instanciranje
    //            Tile t = new Tile(this, q, r);
    //            //GameObject tileGO = Instantiate(TilePrefab, t.Position(), Quaternion.Euler(0, RandomEulerRotationOf60(), 0), this.transform);
    //            GameObject tileGO = Instantiate(TilePrefab, t.Position(), Quaternion.identity, this.transform);
    //            tileGO.transform.GetChild(0).transform.rotation = Quaternion.Euler(-90, RandomEulerRotationOf60(), 0);
    //            tileGO.name = string.Format("{0, -4}, {1, -4} (GO)", q, r);
    //            tileGO.tag = "Tile";
    //            //tileGO.name = q + ", " + r + "(GO)";

    //            //spremanje tilea
    //            tiles[q + mapRings, r + mapRings] = t;
    //            tileToGameObject[t] = tileGO;
    //            gameObjectToTile[tileGO] = t;
    //        }
    //    }

    //    //smanjeni broj batcheva (tokom testiranja je bilo 300-400 bez, a oko 9 sa time) OSTAVITI AKO SE TILEOVI NE MIČU
    //    //StaticBatchingUtility.Combine(this.gameObject);
    //}

    //public void GenerateComplexityRandom(float elevation, int frequency)
    //{
    //    Mathf.Clamp(elevation, -1f, 1f);
    //    Mathf.Clamp(frequency, 0, 100);

    //    for (int tileNum = 0; tileNum < frequency; tileNum++)
    //    {
    //        Tile tile;
    //        do
    //        {
    //            tile = GetRandomExistingTile();
    //        }
    //        while (tile.elevation != 0f);
    //        tile.elevation = elevation;

    //        //nasumično uzimanje 4 nasumična susjeda i postavljanje na isti tip terena za prirodniji izgled terena, odnosno da tileovi nebudu
    //        //for (int neigbourNum = rnd.Next(1, 4); neigbourNum != 0; neigbourNum--)
    //        for (int neigbourNum = Random.Range(1, 3); neigbourNum != 0; neigbourNum--)
    //        {
    //            //Tile neighbourTile = GetNeighbour(tile, rnd.Next(1, 7));
    //            Tile neighbourTile = GetNeighbour(tile, Random.Range(1, 6));
    //            neighbourTile.elevation = elevation;
    //        }
    //    }
    //}

    //public void GenerateForestRandom(int frequency = 30)
    //{
    //    for (int forestNum = 0; forestNum < frequency; forestNum++)
    //    {
    //        //if (rnd.Next(100) < 70)
    //        if (Random.Range(0, 99) < 70)
    //        {
    //            //Tile tX = GetRandomExistingTile();
    //            //Debug.Log(tX.Q + ", " + tX.R);
    //            GetRandomExistingTile().HasForest = true;
    //        }
    //    }
    //}

    //public void CreateUnitOn(Unit unit, GameObject prefab, int x, int y)
    //{
    //    //if (units == null)
    //    //{
    //    //    units = new HashSet<Unit>();
    //    //    unitToGameObjectMap = new Dictionary<Unit, GameObject>();
    //    //}

    //    Tile onTile = GetTile(x, y);

    //    GameObject spawnTileGO = tileToGameObject[onTile];
    //    unit.SetTile(onTile);
    //    GameObject UnitGO = Instantiate(prefab, spawnTileGO.transform.position, Quaternion.identity, spawnTileGO.transform);
    //    unit.Transition += UnitGO.GetComponent<UnitTransition>().Transition;

    //    unit.Transition(onTile, onTile);    //temp

    //    units.Add(unit);
    //    unitToGameObjectMap[unit] = UnitGO;
    //}

    //public Tile GetRandomExistingTile()
    //{
    //    int x, y;
    //    do
    //    {
    //        //x = rnd.Next(tiles.GetLength(0));
    //        //y = rnd.Next(tiles.GetLength(1));
    //        x = Random.Range(0, tiles.GetLength(0) - 1);
    //        y = Random.Range(0, tiles.GetLength(1) - 1);
    //    }
    //    while (tiles == null || tiles[x, y] == null);
    //    return tiles[x, y];
    //}

    //public Tile GetTile(int x, int y)
    //{
    //    if (tiles == null)
    //    {
    //        Debug.LogError("Polje nije instancirano");
    //        return null;
    //    }
    //    else if (tiles[x, y] == null)
    //    {
    //        //ako polje nije popunjeno
    //        //Debug.LogError("Polje nije popunjeno za tu lokaciju");
    //        return null;
    //    }
    //    else
    //    {
    //        //Debug.Log((x-mapRings) + ", " + (y-mapRings));
    //        return tiles[x, y];
    //    }
    //}

    //public Tile GetNeighbour(Tile tile, int loc = 0)
    //{
    //    if (loc > 6 || loc < 1)
    //    {
    //        //loc = rnd.Next(7);
    //        loc = Random.Range(0, 7);
    //    }
    //    int q_rel = 0, r_rel = 0;
    //    switch (loc)
    //    {
    //        case 1:
    //            q_rel = tile.Q;
    //            r_rel = tile.R + 1;
    //            break;
    //        case 2:
    //            q_rel = tile.Q + 1;
    //            r_rel = tile.R;
    //            break;
    //        case 3:
    //            q_rel = tile.Q + 1;
    //            r_rel = tile.R - 1;
    //            break;
    //        case 4:
    //            q_rel = tile.Q;
    //            r_rel = tile.R - 1;
    //            break;
    //        case 5:
    //            q_rel = tile.Q - 1;
    //            r_rel = tile.R;
    //            break;
    //        case 6:
    //            q_rel = tile.Q - 1;
    //            r_rel = tile.R + 1;
    //            break;
    //    }

    //    q_rel += mapRings;
    //    r_rel += mapRings;

    //    if (q_rel > 20 || q_rel < 0 ||
    //        r_rel > 20 || r_rel < 0 ||
    //        tiles == null || tiles[q_rel, r_rel] == null)
    //        return tile;
    //    else
    //        return tiles[q_rel, r_rel];
    //}

    //public Vector3 GetTilePos(int x, int y)
    //{
    //    Tile t = GetTile(x, y);

    //    return t.Position();
    //}
}