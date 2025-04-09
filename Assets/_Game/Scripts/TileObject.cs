using DG.Tweening;
using System;
using UnityEngine;

[Serializable]
public abstract class TileObject : MonoBehaviour
{
    public Tile Tile { get; private set; }
    public abstract bool IsSpecial { get; } 
    public abstract bool IsMatchable { get; }

    protected SpriteRenderer spriteRenderer;
    
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Initialize(Tile tile)
    {
        Tile = tile;
    }

    public abstract void OnSwap(Tile otherTile);

    public virtual void OnClick()
    {
    }
    public virtual void Activate()
    {
    }
    public abstract bool Matches(TileObject other);

    public virtual void DestroyTile(float destroyDuration = 0.25f)
    {
        ParticleManager.Instance.SpawnParticle(transform.position, Quaternion.identity, GetDarkerColor());

        transform.DOScale(Vector3.zero, destroyDuration)
        .OnComplete(() => Destroy(gameObject));
    }

    private Color GetDarkerColor()
    {
        Color baseColor = spriteRenderer.color;
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        v = Mathf.Clamp01(v * 0.92f);
        return Color.HSVToRGB(h, s, v);
    }
}