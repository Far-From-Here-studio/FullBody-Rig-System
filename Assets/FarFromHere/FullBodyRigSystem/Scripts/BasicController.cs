using UnityEngine;

[ExecuteAlways]
public class BasicController : MonoBehaviour
{
    public bool AutoTurn;
    public float Speed;
    Animator animator;
    private LookAtTarget LookAt;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (!AutoTurn) return;
        animator = GetComponent<Animator>();
        LookAt = GetComponent<LookAtTarget>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!AutoTurn) return;
        if(!LookAt) LookAt = GetComponent<LookAtTarget>();
        if(!animator) animator = GetComponent<Animator>();
        if (!LookAt.Isturning) animator.SetFloat("Speed", Speed);
    }
}
