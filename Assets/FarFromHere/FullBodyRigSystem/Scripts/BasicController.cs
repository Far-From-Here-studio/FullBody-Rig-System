using UnityEngine;

[ExecuteAlways]
public class BasicController : MonoBehaviour
{
    public bool AnimatorUpdateEditorMode;
    public bool AutoTurn;
    public float Speed;
    Animator animator = null;
    private LookAtTarget LookAt;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (!AutoTurn) return;
        animator = GetComponent<Animator>();
        LookAt = GetComponent<LookAtTarget>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(AnimatorUpdateEditorMode) animator.Update(Time.deltaTime);
        if (AutoTurn)
        {
            if (!animator) animator = GetComponent<Animator>();
            if (!LookAt) LookAt = GetComponent<LookAtTarget>();
            if (animator.runtimeAnimatorController && animator.isHuman && !LookAt.Isturning) animator.SetFloat("Speed", Speed);
        }
        animator.rootRotation = LookAt.turn();
    }
}
