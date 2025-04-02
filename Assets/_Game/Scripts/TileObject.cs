using DG.Tweening;
using System;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public TileObjectType Type { get; private set; }

    public void SetType(TileObjectType type)
    {
        Type = type;
        GetComponent<SpriteRenderer>().color = GetColorByType(type);
    }

    public void SetDestroySequence(float destroyDuration)
    {
        ParticleManager.Instance.SpawnParticle(transform.position, Quaternion.identity, GetDarkerColor());

        transform.DOScale(Vector3.zero, destroyDuration)
        .OnComplete(() => Destroy(gameObject));
    }

    private Color GetDarkerColor()
    {
        Color baseColor = GetColorByType(Type);
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        v = Mathf.Clamp01(v * 0.92f);
        return Color.HSVToRGB(h, s, v);
    }

    private Color GetColorByType(TileObjectType type)
    {
        return type switch
        {
            TileObjectType.Red => Color.red,
            TileObjectType.Blue => Color.blue,
            TileObjectType.Green => Color.green,
            TileObjectType.Yellow => Color.yellow,
            _ => Color.white
        };
    }
}