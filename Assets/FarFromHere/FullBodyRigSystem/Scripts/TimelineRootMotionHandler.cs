using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Timeline;

[RequireComponent(typeof(Animator))]
public class TimelineRootMotionHandler : MonoBehaviour
{
    public PlayableDirector timelineDirector;
    private Animator animator;

    private Vector3 cumulativeRootMotionPosition;
    private Quaternion cumulativeRootMotionRotation = Quaternion.identity;

    private PlayableGraph rootMotionGraph;
    private AnimationPlayableOutput rootMotionOutput;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (!timelineDirector)
        {
            Debug.LogError("TimelineDirector not assigned!");
            return;
        }

        // Create a PlayableGraph for root motion evaluation
        rootMotionGraph = PlayableGraph.Create("RootMotionGraph");
        rootMotionGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        rootMotionOutput = AnimationPlayableOutput.Create(rootMotionGraph, "RootMotionOutput", animator);
    }

    private void Update()
    {
        if (timelineDirector.state == PlayState.Playing)
        {
            var rootMotionOffset = ExtractRootMotionOffset();
            cumulativeRootMotionPosition += rootMotionOffset.position;
            cumulativeRootMotionRotation *= rootMotionOffset.rotation;
        }
    }

    private void OnAnimatorMove()
    {
        // Apply cumulative root motion adjustments to the animator
        if (animator)
        {
            animator.ApplyBuiltinRootMotion();
            transform.position += cumulativeRootMotionPosition;
            transform.rotation *= cumulativeRootMotionRotation;

            // Reset cumulative motion for next frame
            cumulativeRootMotionPosition = Vector3.zero;
            cumulativeRootMotionRotation = Quaternion.identity;
        }
    }

    private (Vector3 position, Quaternion rotation) ExtractRootMotionOffset()
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        var timelineAsset = timelineDirector.playableAsset as TimelineAsset;
        if (timelineAsset != null)
        {
            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is AnimationTrack animationTrack)
                {
                    foreach (var clip in animationTrack.GetClips())
                    {
                        var animClip = clip.animationClip;
                        if (animClip)
                        {
                            float clipTime = (float)(timelineDirector.time - clip.start);
                            if (clipTime >= 0 && clipTime <= animClip.length)
                            {
                                var (clipPosition, clipRotation) = EvaluateRootMotion(animClip, clipTime);
                                position += clipPosition;
                                rotation *= clipRotation;
                            }
                        }
                    }
                }
            }
        }

        return (position, rotation);
    }

    private (Vector3 position, Quaternion rotation) EvaluateRootMotion(AnimationClip clip, float time)
    {
        // Create an AnimationClipPlayable for the clip
        var clipPlayable = AnimationClipPlayable.Create(rootMotionGraph, clip);
        rootMotionOutput.SetSourcePlayable(clipPlayable);

        // Manually evaluate the graph at the desired time
        clipPlayable.SetTime(time);
        rootMotionGraph.Evaluate();

        // Extract root motion from the Animator
        Vector3 rootPosition = animator.deltaPosition;
        Quaternion rootRotation = animator.deltaRotation;

        // Cleanup PlayableGraph
        rootMotionGraph.DestroyPlayable(clipPlayable);

        return (rootPosition, rootRotation);
    }

    private void OnDestroy()
    {
        // Destroy the PlayableGraph when the object is destroyed
        if (rootMotionGraph.IsValid())
        {
            rootMotionGraph.Destroy();
        }
    }
}
