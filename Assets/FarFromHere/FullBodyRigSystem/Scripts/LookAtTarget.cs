using UnityEngine;

[ExecuteAlways]
public class LookAtTarget : MonoBehaviour
{
    public Transform target;
    public float speed = 0.1f;
    public float RotationThreshold = 1;
    Animator animator = null;
    float _speed = 0;
    float rotationDirection;

    //Make a special turn method for Generic models later on, using RootmotionNode injection
    bool GenericModel;

    public bool Isturning;
    private float currentspeed;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();      
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if(target == null) return;
        animator.rootRotation = turn();
        transform.rotation = turn();
    }

    void LateUpdate()
    {
        if (target == null) return;
        animator.rootRotation = turn();
        transform.rotation = turn();
    }

    public Quaternion turn()
    {
        Vector3 relativePos = transform.InverseTransformDirection(target.position - transform.position);

        // Compute the new y-angle
        float newYAngle = Mathf.Atan2(relativePos.x, relativePos.z) * Mathf.Rad2Deg;

        // Determine the direction of rotation (-1 for left, 1 for right)
        rotationDirection = Mathf.Lerp(rotationDirection, (newYAngle - transform.eulerAngles.y) < 0 ? -1f : 1f, Time.deltaTime * 10);


        // Check if the character is turning to get walk animation accorded to turining state and direction
        if (Mathf.Abs(newYAngle) > RotationThreshold)
        {
            Isturning = true;
            if(animator.runtimeAnimatorController)
            {
                _speed = Mathf.Lerp(_speed, 0.51f, Time.deltaTime * 25);
                animator.SetFloat("Speed", _speed);
                animator.SetFloat("FrontBack", rotationDirection);
            }

        }
        else { Isturning = false; }

        // Build a new rotation using the existing x, z Euler angles and the computed y-angle
        Quaternion rotation = Quaternion.Euler(0f, transform.eulerAngles.y + newYAngle, 0f);
        rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);
        return rotation;
        // Smoothen the rotation
       
    }
}
