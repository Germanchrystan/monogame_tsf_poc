using tsybulya.Components.Collisions;
using tsybulya.Scripts;
using tsybulya.structures;
using tsybulya.GameObjects;

namespace entities.piece_end
{
  public class CheckCollision : Script
  {
    public override void Start() { }
    public override void Update(float dt) { }
    public override void HandleEnterTriggerCollision(CollisionBox collider)
    {
      if (collider.Tag == "STAFF_CONNECTOR")
      {
        Transform colliderTransform = collider.Transform;
        SendMessageToParent(new Message()
        {
          Component = this,
          Name = "COLLIDED_WITH_STAFF_CONNECTOR",
          Metadata = $"{colliderTransform.Position.X},{colliderTransform.Position.Y},{colliderTransform.Size.X},{colliderTransform.Size.Y}"
        });
      }
    }
  }
}