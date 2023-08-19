using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string name="Shotgun";
    public int damage = 10;
    public float range = 100f;
    ///Aici sunt toate chestiile pentru arme, nu e complicat

    public float fireRate = 0f;

    public int maxBullets = 20;
    [HideInInspector]
    public int bullets;

    public float reloadTime = 1f;

    public GameObject graphics;

    public PlayerWeapon()
    {
        bullets=maxBullets;
    }

}
