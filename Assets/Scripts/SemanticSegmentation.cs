using UnityEngine;

public class SemanticSegmentation : MonoBehaviour
{
    public Material buildingMat;
    public Material treeMat;
    public Material vehicleMat;
    public Material groundMat;

    void Start()
    {
        Renderer[] renderers =
            FindObjectsByType<Renderer>(FindObjectsSortMode.None);

        foreach (Renderer r in renderers)
        {
            string layerName =
                LayerMask.LayerToName(r.gameObject.layer);

            if (layerName == "Building")
            {
                r.material = buildingMat;
            }
            else if (layerName == "Vegetation")
            {
                r.material = treeMat;
            }
            else if (layerName == "Vehicle")
            {
                r.material = vehicleMat;
            }
            else if (layerName == "Ground")
            {
                r.material = groundMat;
            }
        }
    }
}