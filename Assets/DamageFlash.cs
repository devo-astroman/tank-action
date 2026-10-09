using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class DamageFlash : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float duration = 0.18f;
    [SerializeField] private Color highlight = new Color(1f, 0.85f, 0.1f);
    private readonly List<Material> materials = new List<Material>();
    private readonly List<Color> originalColors = new List<Color>();
    private float remaining;
    public bool IsFlashing => remaining > 0f;

    private void Awake()
    {
        // Renderer.materials gives this instance its own materials, preserving the prefab assets.
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
        {
            foreach (Material material in renderer.materials)
            {
                materials.Add(material);
                originalColors.Add(material.color);
            }
        }
    }

    public void Flash()
    {
        remaining = duration;
        foreach (Material material in materials) material.color = highlight;
    }

    private void Update()
    {
        if (remaining <= 0f) return;
        remaining -= Time.deltaTime;
        if (remaining <= 0f) Restore();
    }

    private void Restore()
    {
        remaining = 0f;
        for (int i = 0; i < materials.Count; i++)
            if (materials[i] != null) materials[i].color = originalColors[i];
    }

    private void OnDisable() => Restore();
    private void OnDestroy()
    {
        foreach (Material material in materials) if (material != null) Destroy(material);
    }
}
