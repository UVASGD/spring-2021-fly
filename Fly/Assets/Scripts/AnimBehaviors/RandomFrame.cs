using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFrame : StateMachineBehaviour
{
    bool initialized = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!initialized) {
            animator.Play(stateInfo.shortNameHash, layerIndex, Random.Range(0.0f, 1.0f));
            initialized = true;
        }
    }
}
