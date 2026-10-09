using UnityEngine;

public sealed class FireballVfx : MonoBehaviour
{
    private Material material;
    private Texture2D texture;

    private void Awake()
    {
        // Soft luminous particles layered into a yellow oval and orange flame tail.
        texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        for (int y = 0; y < 32; y++)
        for (int x = 0; x < 32; x++)
        {
            float radius = new Vector2((x - 15.5f) / 15.5f, (y - 15.5f) / 15.5f).magnitude;
            float alpha = Mathf.Pow(Mathf.Clamp01(1f - radius), 1.5f);
            texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
        }
        texture.Apply();
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        material = new Material(shader);
        material.mainTexture = texture;
        material.SetTexture("_BaseMap", texture);
        material.SetFloat("_Surface", 1);
        material.SetFloat("_Blend", 2);
        material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
        material.SetFloat("_ZWrite", 0);
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.renderQueue = 3000;
        CreateParticles("Oval flame", true, 100, 0.12f, 0.5f, new Color(1f, 0.45f, 0.02f));
        CreateParticles("Hot yellow core", true, 80, 0.1f, 0.3f, new Color(1f, 0.95f, 0.2f));
        CreateParticles("Short flame tail", false, 90, 0.18f, 0.35f, new Color(1f, 0.3f, 0.01f));
    }

    private void CreateParticles(string name, bool local, float rate, float life, float size, Color color)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(transform, false);
        ParticleSystem particles = child.AddComponent<ParticleSystem>();
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = particles.main;
        main.duration = 1;
        main.loop = true;
        main.startLifetime = life;
        main.startSpeed = local ? 0f : 0.3f;
        main.startSize3D = true;
        main.startSizeX = size;
        main.startSizeY = size * 1.6f;
        main.startSizeZ = size;
        main.startColor = color;
        main.simulationSpace = local ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
        main.maxParticles = 100;
        var emission = particles.emission;
        emission.rateOverTime = rate;
        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = local ? 0.035f : 0.06f;
        var sizeOverLife = particles.sizeOverLifetime;
        sizeOverLife.enabled = true;
        sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0));
        var colors = particles.colorOverLifetime;
        colors.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(new Color(1, 0.2f, 0), 1) },
            new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1) });
        colors.color = gradient;
        var noise = particles.noise;
        noise.enabled = !local;
        noise.strength = 0.12f;
        noise.frequency = 3;
        particles.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
        particles.Play();
    }

    private void OnDestroy()
    {
        if (material != null) Destroy(material);
        if (texture != null) Destroy(texture);
    }
}
