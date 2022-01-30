using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProperties", menuName = "ScriptableObjects/PlayerProperties", order = 1)]
public class PlayerProperties : ScriptableObject
{
    [HeaderAttribute("LogicSize")]
    public float height = 1f;
    public float width = 0.3f;

    [HeaderAttribute("Speeds")]
    public float speed = 3000f;
    public float airSpeed = 1200f;

    [HeaderAttribute("Jump")]
    public float jumpForce = 5000f;
    public float jumpImpulsion = 10f;
    public float minJumpingTime = 0.25f;
    public float maxJumpingTime = 0.4f;

    [HeaderAttribute("SideJump")]
    public float sideJumpForce = 4000f;
    public float sideJumpImpulsion = 8f;
    public float minSideJumpingTime = 0.15f;
    public float maxSideJumpingTime = 0.3f;

    [HeaderAttribute("Gravity")]
    public float gravityForce = 2400f;
    public float frictionGravityRatio = 0.8f;

    [HeaderAttribute("Help")]
    public float coyoteTime = 0.2f;
    public float permaTime = 1f;
}
