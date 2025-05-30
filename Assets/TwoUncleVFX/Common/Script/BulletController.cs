using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace VFXTools
{
    // 子弹朝向类型枚举
    public enum TowardType
    {
        Forward,  // 向前移动
        Right     // 向右移动
    }

    // 子弹控制器类，管理子弹的移动、旋转和视觉效果
    public class BulletController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float rotationSpeed = 100f;   // 旋转速度(度/秒)
        public float movementSpeed = 10f;    // 移动速度(单位/秒)
        public float delayTime = 0f;         // 初始延迟时间(秒)
        public float time = 1f;             // 子弹生命周期(秒)
        public TowardType towardType = TowardType.Forward; // 移动方向类型
        public float maxDistance = 100f;    // 最大移动距离

        [Header("Internal State")]
        private bool isPlay = false;        // 是否激活子弹运动
        private float lastTime = 0f;         // 计时器
        private Vector3 startPos;            // 初始位置
        private Vector3 directionToCenter;   // 朝向中心的向量
        private Vector3 scale;               // 初始缩放值
        private float curDistance = 0f;      // 当前移动距离

        [Header("Visual Effects")]
        private VisualEffect[] vfxs;         // 所有子对象的视觉特效组件
        private TrailRenderer[] trails;      // 所有子对象的拖尾渲染组件

        // 初始化
        private void Start()
        {
            // 获取所有子对象的视觉组件
            vfxs = GetComponentsInChildren<VisualEffect>(false);
            trails = GetComponentsInChildren<TrailRenderer>(false);

            // 记录初始位置
            startPos = transform.position;

            // 激活子弹
            SetPlay(true);
        }

        // 设置子弹激活状态(异步)
        private async void SetPlay(bool play)
        {
            // 可以取消注释实现延迟激活
            // await Task.Delay((int)(delayTime * 1000));
            isPlay = play;
        }

        // 每帧更新
        private void Update()
        {
            // 空格键切换激活状态(调试用)
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isPlay = !isPlay;
            }

            // 如果未激活则直接返回
            if (!isPlay) return;

            // 更新时间计数器
            lastTime += Time.deltaTime;

            // 检查是否超过生命周期或最大距离
            if (lastTime > time || curDistance > maxDistance)
            {
                ResetBullet();
                return;
            }

            // 检查是否还在延迟时间内
            if (delayTime > lastTime)
            {
                return;
            }

            // 更新朝向
            UpdateRotation();

            // 根据类型移动子弹
            MoveBullet();

            // 更新移动距离
            curDistance += movementSpeed * Time.deltaTime;
        }

        // 重置子弹状态
        private void ResetBullet()
        {
            // 保存当前缩放
            scale = transform.localScale;

            // 禁用所有视觉特效
            foreach (var vfx in vfxs)
            {
                vfx.enabled = false;
            }

            // 禁用所有拖尾效果
            foreach (var trail in trails)
            {
                trail.enabled = false;
            }

            // 重置位置和缩放
            transform.localScale = Vector3.zero;
            transform.position = startPos;

            // 重置计时器和距离
            lastTime = 0f;
            curDistance = 0f;

            // 恢复缩放
            transform.localScale = scale;

            // 设置为非激活状态
            isPlay = false;

            // 延迟后重新激活
            DelayEnable();
        }

        // 更新子弹旋转
        private void UpdateRotation()
        {
            directionToCenter = transform.forward;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // 移动子弹
        private void MoveBullet()
        {
            switch (towardType)
            {
                case TowardType.Forward:
                    transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
                    break;
                case TowardType.Right:
                    transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
                    break;
            }
        }

        // 延迟激活子弹(异步)
        public async void DelayEnable()
        {
            // 等待500毫秒
            await Task.Delay(500);

            // 启用所有视觉特效
            foreach (var vfx in vfxs)
            {
                vfx.enabled = true;
            }

            // 启用所有拖尾效果
            foreach (var trail in trails)
            {
                trail.enabled = true;
            }

            // 激活子弹
            isPlay = true;
        }
    }
}