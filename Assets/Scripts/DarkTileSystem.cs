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
    public AudioClip darkTileExpandSound; // 黑雾扩展音效
    public AudioClip darkTileClearSound;  // 清除黑雾音效
    private AudioSource audioSource;
    [Range(0, 1)] public float clearSoundVolume = 0.8f;
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();


    private void Awake()
    {
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }

        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
      
        }
        else
        {
            Destroy(this);
            return;
        }

        // 添加AudioSource组件
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void ExpandDarkTiles(int expandCount,bool isUnlimited = false)
    {
        List<Vector3Int> newDarkTiles = new List<Vector3Int>();

        Vector3Int playerHex = baseMap.WorldToCell(PlayerController.Instance.transform.position);

        // 先遍历地图中所有的 darkTile 位置
        List<Vector3Int> allDarkPositions = new List<Vector3Int>();
        foreach (var pos in baseMap.cellBounds.allPositionsWithin)
        {
            if (baseMap.GetTile(pos) == darkTile)
            {
                allDarkPositions.Add(pos);
            }
        }
        // 按照距离玩家的远近进行排序（近的排前面）
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

                // 检查六个方向的邻居
                for (int i = 0; i < 6; i++)
                {

                    Vector3Int correctDir = neighborDirs[i];

                    Vector3Int neighbor = pos + correctDir;
                    // 如果邻居正好是玩家所在位置，并且该格子还为 normalTile，则优先选中
                    if (neighbor == playerHex && baseMap.GetTile(neighbor) == normalTile)
                    {
                        bestNeighbor = neighbor;
                        foundNeighbor = true;
                        break; // 直接确定向玩家方向扩展
                    }
 

                    // 否则，对于仍为 normalTile 的邻居，计算其到玩家格子的距离
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

                // 如果找到合适的邻居，并且没有重复添加且未超过扩展数量
                if (foundNeighbor && !newDarkTiles.Contains(bestNeighbor) && (newDarkTiles.Count < expandCount || isUnlimited))//isUnlimited表示没有限制
                {
                    bool isIntercept = BlockPillarSystem.Instance.IsIntercept(bestNeighbor, direction);
                    if (!isIntercept)
                    {
                        newDarkTiles.Add(bestNeighbor);
                    }
                }

            }
        }

        // 应用新的黑暗地块
        foreach (var tilePos in newDarkTiles)
        {
            baseMap.SetTile(tilePos, darkTile);
        }
        if (newDarkTiles.Count > 0 && darkTileExpandSound != null)
        {
            audioSource.PlayOneShot(darkTileExpandSound);

            // 可选：根据生成的黑雾数量调整音量
            float volume = Mathf.Clamp(newDarkTiles.Count / 10f, 0.1f, 1f);
            audioSource.PlayOneShot(darkTileExpandSound, volume);
        }
        CheckPlayerCaught();
    }

    private Vector3Int GetHexNeighbor(Vector3Int position, int direction)
    {
        // 六边形网格的六个方向
        Vector3Int[] hexDirections = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),   // 上
            new Vector3Int(1, -1, 0),  // 左上
            new Vector3Int(1, 1, 0),  // 右上
            new Vector3Int(-1, 0, 0),  // 下
            new Vector3Int(0, -1, 0),  // 左下
            new Vector3Int(0, 1, 0)    // 右下
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
        // 使用协程实现延迟清除
        StartCoroutine(ClearDarkTileWithDelay(position, delay));
    }

    /// <summary>
    /// 延迟清除黑雾地块的协程
    /// </summary>
    private IEnumerator ClearDarkTileWithDelay(Vector3Int position, float delay)
    {
        // 等待指定的延迟时间
        yield return new WaitForSeconds(delay);

        // 验证 Tilemap 和 darkTile 是否已赋值
        if (tilemap == null || darkTile == null)
        {
            Debug.LogError("Tilemap 或 darkTile 未配置！");
            yield break;
        }

        // 检查目标位置是否是黑雾地块
        if (baseMap.GetTile(position) == darkTile)
        {
            // 播放清除音效
            if (darkTileClearSound != null)
            {
                audioSource.PlayOneShot(darkTileClearSound, clearSoundVolume);
            }

            // 恢复原始地块（如果已存储）
            if (originalTiles.TryGetValue(position, out TileBase original))
            {
                baseMap.SetTile(position, original);
                originalTiles.Remove(position);
                Debug.Log($"已清除黑雾地块：{position}，恢复为 {original.name}");
            }
            else
            {
                // 默认恢复为预设的普通地块
                baseMap.SetTile(position, normalTile);
                Debug.LogWarning($"位置 {position} 无原始记录，恢复为默认地块");
            }

            // 强制刷新显示
            baseMap.RefreshTile(position);

            // 播放地块清除特效
            EffectManager.Instance.PlayTileDestroyEffect(baseMap.GetCellCenterWorld(position));
        }
    }
    // 在DarkTileSystem中添加Init方法
    public void Init()
    {
        // 重置原始地块记录
        originalTiles.Clear();

        // 如果需要，可以在这里添加初始黑色地块的生成逻辑
        // 例如：ExpandDarkTiles(initialDarkTileCount);
    }
}
