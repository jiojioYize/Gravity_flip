using UnityEngine;

namespace GravityFlip.Level
{
    /// <summary>
    /// Decides whether the player is on a walkable platform surface (top/bottom for current gravity),
    /// not brushing a vertical side. Contact normals differ by which body receives the callback,
    /// so alignment uses the absolute dot product. Cast along gravity is used when contacts are missing.
    /// </summary>
    public static class MovingPlatformContact
    {
        public const float MinSupportAlignment = 0.5f;
        public const float SupportCastDistance = 0.12f;

        private static readonly RaycastHit2D[] CastHits = new RaycastHit2D[4];

        public static bool HasWalkableSupport(
            Collision2D collision,
            Vector2 gravityDirection,
            Collider2D playerCollider,
            Collider2D platformCollider)
        {
            if (collision == null || playerCollider == null || platformCollider == null)
            {
                return false;
            }

            Vector2 supportDirection = -gravityDirection.normalized;

            if (collision.contactCount > 0)
            {
                for (int i = 0; i < collision.contactCount; i++)
                {
                    ContactPoint2D contact = collision.GetContact(i);
                    float alignment = Mathf.Abs(Vector2.Dot(contact.normal, supportDirection));
                    if (alignment >= MinSupportAlignment)
                    {
                        return true;
                    }
                }
            }

            return HasSupportViaCast(playerCollider, platformCollider, gravityDirection);
        }

        private static bool HasSupportViaCast(
            Collider2D playerCollider,
            Collider2D platformCollider,
            Vector2 gravityDirection)
        {
            ContactFilter2D contactFilter = new ContactFilter2D
            {
                useTriggers = false
            };
            contactFilter.SetLayerMask(Physics2D.AllLayers);

            int hitCount = playerCollider.Cast(
                gravityDirection.normalized,
                contactFilter,
                CastHits,
                SupportCastDistance);

            for (int i = 0; i < hitCount; i++)
            {
                if (CastHits[i].collider == platformCollider)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
