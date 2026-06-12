using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SemanticSegmentation : MonoBehaviour
{
    [Header("Segmentation Materials")]
    public Material buildingMat;
    public Material treeMat;
    public Material vehicleMat;
    public Material groundMat;

    [Header("Target Camera")]
    [Tooltip("Camera chạy semantic pass. Bỏ trống = Camera trên cùng GameObject.")]
    public Camera semanticCamera;

    struct Entry
    {
        public Renderer renderer;
        public Material[] originals;
        public Material[] swap;
    }

    readonly List<Entry> _cache = new();
    bool _swapped;

    void OnEnable()
    {
        if (semanticCamera == null) semanticCamera = GetComponent<Camera>();
        if (semanticCamera == null)
        {
            Debug.LogError($"[{nameof(SemanticSegmentation)}] No Camera assigned and no Camera component on '{name}'. Disabling.", this);
            enabled = false;
            return;
        }

        BuildCache();
        Camera.onPreCull += HandlePreCull;
        Camera.onPostRender += HandlePostRender;
    }

    void OnDisable()
    {
        Camera.onPreCull -= HandlePreCull;
        Camera.onPostRender -= HandlePostRender;
        if (_swapped) RestoreOriginals();
        _cache.Clear();
    }

    void BuildCache()
    {
        _cache.Clear();
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        foreach (Renderer r in renderers)
        {
            Material segMat = PickSegMaterial(r.gameObject.layer);
            if (segMat == null) continue;

            Material[] originals = r.sharedMaterials;
            Material[] swap = new Material[originals.Length];
            for (int i = 0; i < swap.Length; i++) swap[i] = segMat;

            _cache.Add(new Entry { renderer = r, originals = originals, swap = swap });
        }
    }

    Material PickSegMaterial(int layer)
    {
        string layerName = LayerMask.LayerToName(layer);
        return layerName switch
        {
            "Building" => buildingMat,
            "Vegetation" => treeMat,
            "Vehicle" => vehicleMat,
            "Ground" => groundMat,
            _ => null,
        };
    }

    void HandlePreCull(Camera cam)
    {
        if (cam != semanticCamera || _swapped) return;
        for (int i = 0; i < _cache.Count; i++)
        {
            Entry e = _cache[i];
            if (e.renderer != null) e.renderer.sharedMaterials = e.swap;
        }
        _swapped = true;
    }

    void HandlePostRender(Camera cam)
    {
        if (cam != semanticCamera || !_swapped) return;
        RestoreOriginals();
    }

    void RestoreOriginals()
    {
        for (int i = 0; i < _cache.Count; i++)
        {
            Entry e = _cache[i];
            if (e.renderer != null) e.renderer.sharedMaterials = e.originals;
        }
        _swapped = false;
    }
}
