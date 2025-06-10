using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DarkTileSystem : MonoBehaviour
{
    [Header("Tilemap References")]
    public Tilemap tilemap;
    public Tilemap baseMap;
    public TileBase darkTile;
    public TileBase normalTile;
    public static DarkTileSystem Instance;

    [Header("Audio Settings")]
    public AudioClip darkTileExpandSound; // ������չ��Ч
    public AudioClip darkTileClearSound;  // ���������Ч
    private AudioSource audioSource;
    [Range(0, 1)] public float clearSoundVolume = 0.8f;
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();


    private void Awake()
    {
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }

        // ����ģʽ��ʼ��
        if (Instance == null)
        {
            Instance = this;
      
        }
        else
        {
            Destroy(this);
            return;
        }

        // ���AudioSource���
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void ExpandDarkTiles(int expandCount,bool isUnlimited = false)
    {
        List<Vector3Int> newDarkTiles = new List<Vector3Int>();

        Vector3Int playerHex = baseMap.WorldToCell(PlayerController.Instance.transform.position);

        // �ȱ�����ͼ�����е� darkTile λ��
        List<Vector3Int> allDarkPositions = new List<Vector3Int>();
        foreach (var pos in baseMap.cellBounds.allPositionsWithin)
        {
            if (baseMap.GetTile(pos) == darkTile)
            {
                allDarkPositions.Add(pos);
            }
        }
        // ���վ�����ҵ�Զ���������򣨽�����ǰ�棩
        allDarkPositions.Sort((a, b) => Vector3Int.Distance(a, playerHex).CompareTo(Vector3Int.Distance(b, playerHex)));

        foreach (var pos in allDarkPositions)
        {
            if (baseMap.GetTile(pos) == darkTile)
            {
                Vector3Int bestNeighbor = Vector3Int.zero;
                float minDistance = float.MaxValue;
                bool foundNeighbor = false;

                Vector3Int[] neighborDirs = HexGridSystem.Instance.GetNeighborDirectionsForPosition(pos);
                Vector3Int direction = Vector3Int.zero;

                // �������������ھ�
                for (int i = 0; i < 6; i++)
                {

                    Vector3Int correctDir = neighborDirs[i];

                    Vector3Int neighbor = pos + correctDir;
                    // ����ھ��������������λ�ã����Ҹø��ӻ�Ϊ normalTile��������ѡ��
                    if (neighbor == playerHex && baseMap.GetTile(neighbor) == normalTile)
                    {
                        bestNeighbor = neighbor;
                        foundNeighbor = true;
                        break; // ֱ��ȷ������ҷ�����չ
                    }
 

                    // ���򣬶�����Ϊ normalTile ���ھӣ������䵽��Ҹ��ӵľ���
                    if (baseMap.GetTile(neighbor) == normalTile)
                    {
                        float distance = Vector3Int.Distance(neighbor, playerHex);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            direction = correctDir;
                            bestNeighbor = neighbor;
                            foundNeighbor = true;
                        }
                    }
                }

                // ����ҵ����ʵ��ھӣ�����û���ظ������δ������չ����
                if (foundNeighbor && !newDarkTiles.Contains(bestNeighbor) && (newDarkTiles.Count < expandCount || isUnlimited))//isUnlimited��ʾû������
                {
                    bool isIntercept = BlockPillarSystem.Instance.IsIntercept(bestNeighbor, direction);
                    if (!isIntercept)
                    {
                        newDarkTiles.Add(bestNeighbor);
                    }
                }

            }
        }

        // Ӧ���µĺڰ��ؿ�
        foreach (var tilePos in newDarkTiles)
        {
            baseMap.SetTile(tilePos, darkTile);
        }
        if (newDarkTiles.Count > 0 && darkTileExpandSound != null)
        {
            audioSource.PlayOneShot(darkTileExpandSound);

            // ��ѡ���������ɵĺ���������������
            float volume = Mathf.Clamp(newDarkTiles.Count / 10f, 0.1f, 1f);
            audioSource.PlayOneShot(darkTileExpandSound, volume);
        }
        CheckPlayerCaught();
    }

    private Vector3Int GetHexNeighbor(Vector3Int position, int direction)
    {
        // �������������������
        Vector3Int[] hexDirections = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),   // ��
            new Vector3Int(1, -1, 0),  // ����
            new Vector3Int(1, 1, 0),  // ����
            new Vector3Int(-1, 0, 0),  // ��
            new Vector3Int(0, -1, 0),  // ����
            new Vector3Int(0, 1, 0)    // ����
        };

        return position + hexDirections[direction];
    }

    private void CheckPlayerCaught()
    {   
        Vector3Int playerHex = baseMap.WorldToCell(PlayerController.Instance.transform.position);
        if (baseMap.GetTile(playerHex) == darkTile)
        {
            GameManager.Instance.GameOver(false);
        }
    }

    public void ClearDarkTile(Vector3Int position, float delay = 1f)
    {
        // ʹ��Э��ʵ���ӳ����
        StartCoroutine(ClearDarkTileWithDelay(position, delay));
    }

    /// <summary>
    /// �ӳ��������ؿ��Э��
    /// </summary>
    private IEnumerator ClearDarkTileWithDelay(Vector3Int position, float delay)
    {
        // �ȴ�ָ�����ӳ�ʱ��
        yield return new WaitForSeconds(delay);

        // ��֤ Tilemap �� darkTile �Ƿ��Ѹ�ֵ
        if (tilemap == null || darkTile == null)
        {
            Debug.LogError("Tilemap �� darkTile δ���ã�");
            yield break;
        }

        // ���Ŀ��λ���Ƿ��Ǻ���ؿ�
        if (baseMap.GetTile(position) == darkTile)
        {
            // ���������Ч
            if (darkTileClearSound != null)
            {
                audioSource.PlayOneShot(darkTileClearSound, clearSoundVolume);
            }

            // �ָ�ԭʼ�ؿ飨����Ѵ洢��
            if (originalTiles.TryGetValue(position, out TileBase original))
            {
                baseMap.SetTile(position, original);
                originalTiles.Remove(position);
                Debug.Log($"���������ؿ飺{position}���ָ�Ϊ {original.name}");
            }
            else
            {
                // Ĭ�ϻָ�ΪԤ�����ͨ�ؿ�
                baseMap.SetTile(position, normalTile);
                Debug.LogWarning($"λ�� {position} ��ԭʼ��¼���ָ�ΪĬ�ϵؿ�");
            }

            // ǿ��ˢ����ʾ
            baseMap.RefreshTile(position);

            // ���ŵؿ������Ч
            EffectManager.Instance.PlayTileDestroyEffect(baseMap.GetCellCenterWorld(position));
        }
    }
    // ��DarkTileSystem�����Init����
    public void Init()
    {
        // ����ԭʼ�ؿ��¼
        originalTiles.Clear();

        // �����Ҫ��������������ӳ�ʼ��ɫ�ؿ�������߼�
        // ���磺ExpandDarkTiles(initialDarkTileCount);
    }
}
