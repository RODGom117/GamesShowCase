using UnityEngine;


[CreateAssetMenu(menuName = "HeroAttackObject")]
public class HeroAttackObject : ScriptableObject
{
    //Takes in the animation that is supposed to be played.
    public AnimatorOverrideController animatorOV;
    //Damage variable of attack.
    public float damage = 5;
    public float areaOfEffect = 3.0f;
    public float cooldown = 5.0f;
    public string name = "default";
    public string description = "default";

    [Header("Only used for Abilities that have a duration, leave at 0 if not applicable")]
    public float duration = 0.0f;

    
    //Only for Rosa
    [Header("Only used for Rosa, leave at 0 if not Rosa")]
    public float madnessValue = 0.0f;
}
