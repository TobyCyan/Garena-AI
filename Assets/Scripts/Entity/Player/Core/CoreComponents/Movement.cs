using UnityEngine;

namespace Player.Core
{
    public class Movement : CoreComponent
    {
        public Rigidbody2D Rigidbody { get; private set; }
        public Vector2 CurrentVelocity { get; private set; }
        public int FacingDirection { get; private set; } = 1;
        public bool CanSetVelocity { get; set; } = true;

        protected override void Awake()
        {
            base.Awake();
            Rigidbody = GetComponentInParent<Rigidbody2D>();
        }

        public override void LogicUpdate()
        {
            if (Rigidbody != null)
            {
                CurrentVelocity = Rigidbody.linearVelocity;
            }
        }

        public void SetVelocityZero()
        {
            SetVelocity(Vector2.zero);
        }

        public void SetVelocityX(float velocity)
        {
            SetVelocity(new Vector2(velocity, CurrentVelocity.y));
        }

        public void SetVelocityY(float velocity)
        {
            SetVelocity(new Vector2(CurrentVelocity.x, velocity));
        }

        public void SetVelocity(float velocity, Vector2 direction)
        {
            SetVelocity(direction.normalized * velocity);
        }

        public void SetVelocity(Vector2 velocity)
        {
            if (!CanSetVelocity || Rigidbody == null)
            {
                return;
            }

            Rigidbody.linearVelocity = velocity;
            CurrentVelocity = velocity;
        }

        public void CheckIfShouldFlip(int horizontalInput)
        {
            if (horizontalInput != 0 && horizontalInput != FacingDirection)
            {
                Flip();
            }
        }

        public Vector2 GetWorldPoint(Vector2 localOffset)
        {
            localOffset.x *= FacingDirection;
            Transform root = Core != null && Core.Root != null ? Core.Root.transform : transform;
            return (Vector2)root.position + localOffset;
        }

        private void Flip()
        {
            FacingDirection *= -1;
            Transform root = Core != null && Core.Root != null ? Core.Root.transform : transform;
            Vector3 scale = root.localScale;
            scale.x = Mathf.Abs(scale.x) * FacingDirection;
            root.localScale = scale;
        }
    }
}
