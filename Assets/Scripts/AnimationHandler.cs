using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private Animation _animation;

    void OnEnable()
    {
        _animation = gameObject.GetComponent<Animation>();
        _animation.Play();
        _animation.playAutomatically = true;
    }
}