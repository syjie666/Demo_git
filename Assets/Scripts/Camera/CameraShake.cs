using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("震动参数")]
    [Tooltip("震动持续时间")]
    public float shakeDuration = 0.5f;
    [Tooltip("最大震动强度")]
    public float maxShakeIntensity = 0.001f;
    [Tooltip("震动频率（每秒震动次数）")]
    public float shakeFrequency = 20f;
    [Tooltip("衰减曲线")]
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

    // 玩家受伤时调用这个方法
    public void Shake()
    {
        Debug.Log("触发相机震动");
        currentShakeTime = 0f;
        StartCoroutine(DoShake());
    }

    private IEnumerator DoShake()
    {
        while (currentShakeTime < shakeDuration)
        {
            currentShakeTime += Time.deltaTime;

            // 计算当前震动进度(0-1)
            float progress = currentShakeTime / shakeDuration;

            // 根据衰减曲线获取当前强度
            float currentIntensity = maxShakeIntensity * dampingCurve.Evaluate(progress);

            // 使用Perlin噪声生成更自然的随机值
            float time = Time.time * shakeFrequency;
            float x = (Mathf.PerlinNoise(time, 0f) * 2 - 1) * currentIntensity;
            float y = (Mathf.PerlinNoise(0f, time) * 2 - 1) * currentIntensity;

            // 计算新的偏移量
            Vector3 newOffset = new Vector3(x, y, 0f);

            // 平滑过渡到新位置
            transform.localPosition = originalPosition + Vector3.Lerp(lastShakeOffset, newOffset, 0.5f);
            lastShakeOffset = newOffset;

            yield return null;
        }

        // 平滑回到原位
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
