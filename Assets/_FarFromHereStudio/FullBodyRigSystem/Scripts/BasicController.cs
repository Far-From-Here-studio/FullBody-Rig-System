using UnityEngine;

public class BasicController : MonoBehaviour
{
    public float Speed;
    Animator animator;
    [SerializeField] private LookAtTarget LookAt;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        LookAt = GetComponent<LookAtTarget>();
    }

    // Update is called once per frame
    void Update()
    {

        if(!LookAt.Isturning)animator.SetFloat("Speed", Speed);
    }
}
