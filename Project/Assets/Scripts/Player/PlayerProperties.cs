using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProperties", menuName = "ScriptableObjects/PlayerProperties", order = 1)]
public class PlayerProperties : ScriptableObject
{
    public float speed = 10f;

    public float airSpeed = 2f;
    public float jumpForce = 100f;
    public float gravityForce = 100f;
    public float minJumpingTime = 0.5f;
    public float maxJumpingTime = 1.5f;
}
