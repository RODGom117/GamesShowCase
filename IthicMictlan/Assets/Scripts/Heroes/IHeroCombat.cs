using System.Collections;

namespace Heroes
{
    public interface IHeroCombat
    {
        IEnumerator IgnacioBuffBeam(float duration);

        IEnumerator IgnacioBuffUltimate(float strength, float duration);
    }
}