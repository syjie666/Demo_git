using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("�𶯲���")]
    [Tooltip("�𶯳���ʱ��")]
    public float shakeDuration = 0.5f;
    [Tooltip("�����ǿ��")]
    public float maxShakeIntensity = 0.001f;
    [Tooltip("��Ƶ�ʣ�ÿ���𶯴�����")]
    public float shakeFrequency = 20f;
    [Tooltip("˥������")]
    public AnimationCurve dampingCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(0.5f, 0.5f),
        new Keyframe(1f, 0f)
    );

    private Vector3 originalPosition;
    private float currentShakeTime;
    private Vector3 lastShakeOffset;

    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    

    void OnEnable()
    {

        
        if (EventCenter.instance == null)
            Debug.Log("lllll");
        EventCenter.instance.OnCameraShake += Shake;
    }
    void OnDisable()
    {
        EventCenter.instance.OnCameraShake -= Shake;
    }

    // �������ʱ�����������
    public void Shake()
    {
        Debug.Log("���������");
        currentShakeTime = 0f;
        StartCoroutine(DoShake());
    }

    private IEnumerator DoShake()
    {
        while (currentShakeTime < shakeDuration)
        {
            currentShakeTime += Time.deltaTime;

            // ���㵱ǰ�𶯽���(0-1)
            float progress = currentShakeTime / shakeDuration;

            // ����˥�����߻�ȡ��ǰǿ��
            float currentIntensity = maxShakeIntensity * dampingCurve.Evaluate(progress);

            // ʹ��Perlin�������ɸ���Ȼ�����ֵ
            float time = Time.time * shakeFrequency;
            float x = (Mathf.PerlinNoise(time, 0f) * 2 - 1) * currentIntensity;
            float y = (Mathf.PerlinNoise(0f, time) * 2 - 1) * currentIntensity;

            // �����µ�ƫ����
            Vector3 newOffset = new Vector3(x, y, 0f);

            // ƽ�����ɵ���λ��
            transform.localPosition = originalPosition + Vector3.Lerp(lastShakeOffset, newOffset, 0.5f);
            lastShakeOffset = newOffset;

            yield return null;
        }

        // ƽ���ص�ԭλ
        float returnTime = 0f;
        Vector3 startPosition = transform.localPosition;
        while (returnTime < 0.1f)
        {
            returnTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPosition, originalPosition, returnTime / 0.1f);
            yield return null;
        }

        transform.localPosition = originalPosition;
        lastShakeOffset = Vector3.zero;
    }
}
