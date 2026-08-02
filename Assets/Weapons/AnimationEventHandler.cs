using System;
using UnityEngine;

namespace Player.Weapons
{
    public class AnimationEventHandler : MonoBehaviour
    {
        public event Action AttackAction;
        public event Action AttackFinished;

        public void AttackActionTrigger()
        {
            AttackAction?.Invoke();
        }

        public void AnimationFinishedTrigger()
        {
            AttackFinished?.Invoke();
        }
    }
}
