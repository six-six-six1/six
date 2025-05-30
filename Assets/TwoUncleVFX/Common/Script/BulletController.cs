using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace VFXTools
{
    // �ӵ���������ö��
    public enum TowardType
    {
        Forward,  // ��ǰ�ƶ�
        Right     // �����ƶ�
    }

    // �ӵ��������࣬�����ӵ����ƶ�����ת���Ӿ�Ч��
    public class BulletController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float rotationSpeed = 100f;   // ��ת�ٶ�(��/��)
        public float movementSpeed = 10f;    // �ƶ��ٶ�(��λ/��)
        public float delayTime = 0f;         // ��ʼ�ӳ�ʱ��(��)
        public float time = 1f;             // �ӵ���������(��)
        public TowardType towardType = TowardType.Forward; // �ƶ���������
        public float maxDistance = 100f;    // ����ƶ�����

        [Header("Internal State")]
        private bool isPlay = false;        // �Ƿ񼤻��ӵ��˶�
        private float lastTime = 0f;         // ��ʱ��
        private Vector3 startPos;            // ��ʼλ��
        private Vector3 directionToCenter;   // �������ĵ�����
        private Vector3 scale;               // ��ʼ����ֵ
        private float curDistance = 0f;      // ��ǰ�ƶ�����

        [Header("Visual Effects")]
        private VisualEffect[] vfxs;         // �����Ӷ�����Ӿ���Ч���
        private TrailRenderer[] trails;      // �����Ӷ������β��Ⱦ���

        // ��ʼ��
        private void Start()
        {
            // ��ȡ�����Ӷ�����Ӿ����
            vfxs = GetComponentsInChildren<VisualEffect>(false);
            trails = GetComponentsInChildren<TrailRenderer>(false);

            // ��¼��ʼλ��
            startPos = transform.position;

            // �����ӵ�
            SetPlay(true);
        }

        // �����ӵ�����״̬(�첽)
        private async void SetPlay(bool play)
        {
            // ����ȡ��ע��ʵ���ӳټ���
            // await Task.Delay((int)(delayTime * 1000));
            isPlay = play;
        }

        // ÿ֡����
        private void Update()
        {
            // �ո���л�����״̬(������)
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isPlay = !isPlay;
            }

            // ���δ������ֱ�ӷ���
            if (!isPlay) return;

            // ����ʱ�������
            lastTime += Time.deltaTime;

            // ����Ƿ񳬹��������ڻ�������
            if (lastTime > time || curDistance > maxDistance)
            {
                ResetBullet();
                return;
            }

            // ����Ƿ����ӳ�ʱ����
            if (delayTime > lastTime)
            {
                return;
            }

            // ���³���
            UpdateRotation();

            // ���������ƶ��ӵ�
            MoveBullet();

            // �����ƶ�����
            curDistance += movementSpeed * Time.deltaTime;
        }

        // �����ӵ�״̬
        private void ResetBullet()
        {
            // ���浱ǰ����
            scale = transform.localScale;

            // ���������Ӿ���Ч
            foreach (var vfx in vfxs)
            {
                vfx.enabled = false;
            }

            // ����������βЧ��
            foreach (var trail in trails)
            {
                trail.enabled = false;
            }

            // ����λ�ú�����
            transform.localScale = Vector3.zero;
            transform.position = startPos;

            // ���ü�ʱ���;���
            lastTime = 0f;
            curDistance = 0f;

            // �ָ�����
            transform.localScale = scale;

            // ����Ϊ�Ǽ���״̬
            isPlay = false;

            // �ӳٺ����¼���
            DelayEnable();
        }

        // �����ӵ���ת
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

        // �ƶ��ӵ�
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

        // �ӳټ����ӵ�(�첽)
        public async void DelayEnable()
        {
            // �ȴ�500����
            await Task.Delay(500);

            // ���������Ӿ���Ч
            foreach (var vfx in vfxs)
            {
                vfx.enabled = true;
            }

            // ����������βЧ��
            foreach (var trail in trails)
            {
                trail.enabled = true;
            }

            // �����ӵ�
            isPlay = true;
        }
    }
}