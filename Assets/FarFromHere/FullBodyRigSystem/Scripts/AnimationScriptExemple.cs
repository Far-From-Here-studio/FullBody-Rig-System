using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public struct AnimationJob : IAnimationJob
{
    public int userData;

    public void ProcessRootMotion(AnimationStream stream)
    {
        // This method is called during the root motion process pass.
    }

    public void ProcessAnimation(AnimationStream stream)
    {
        // This method is called during the animation process pass.
        Debug.Log(string.Format("Value of the userData: {0}", userData));
    }
}

[RequireComponent(typeof(Animator))]
public class AnimationScriptExample : MonoBehaviour
{
    PlayableGraph m_Graph;
    AnimationScriptPlayable m_AnimationScriptPlayable;

    void OnEnable()
    {
        m_Graph = PlayableGraph.Create("AnimationScriptExample");
        var output = AnimationPlayableOutput.Create(m_Graph, "ouput", GetComponent<Animator>());

        var animationJob = new AnimationJob();
        m_AnimationScriptPlayable = AnimationScriptPlayable.Create(m_Graph, animationJob);

        output.SetSourcePlayable(m_AnimationScriptPlayable);
        m_Graph.Play();
    }

    void Update()
    {
        var animationJob = m_AnimationScriptPlayable.GetJobData<AnimationJob>();
        ++animationJob.userData;
        m_AnimationScriptPlayable.SetJobData(animationJob);
    }

    void OnDisable()
    {
        m_Graph.Destroy();
    }
}