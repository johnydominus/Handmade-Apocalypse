using UnityEngine;

[CreateAssetMenu(fileName = "SphereIconConfig", menuName = "Game/Sphere Icon Config")]
public class SphereIconConfig : ScriptableObject
{
    public Sprite medicineIcon;
    public Sprite diplomacyIcon;
    public Sprite agricultureIcon;
    public Sprite educationAndScienceIcon;
    public Sprite ecologyIcon;
    public Sprite astronauticsIcon;

    public Sprite GetIconForSphere(SphereType sphereType)
    {
        return sphereType switch
        {
            SphereType.Medicine => medicineIcon,
            SphereType.Diplomacy => diplomacyIcon,
            SphereType.Agriculture => agricultureIcon,
            SphereType.EducationAndScience => educationAndScienceIcon,
            SphereType.Ecology => ecologyIcon,
            SphereType.Astronautics => astronauticsIcon,
            _ => null,
        };
    }
}
