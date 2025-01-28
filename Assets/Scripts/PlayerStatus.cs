using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public bool hasWeapon = false;
    public bool hasGrapple = false;
    public int knifeDamage = 1; // You can adjust the damage amount as needed

    public void AcquireWeapon()
    {
        hasWeapon = true;
    }

    public void AcquireGrapple()
    {
        hasGrapple = true;
    }
}
