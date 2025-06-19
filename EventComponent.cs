using UnityEngine;
using System;

public class EventComponent : MonoBehaviour
{
    void Update()
    {
        if (GameHelpers.GetMovementComponent())
        {
            if (ConfigValues.speed.constantUpdating)
            {
                GameHelpers.GetMovementComponent().movementModifier = ConfigValues.speed.value;
            }
            if (ConfigValues.jump.constantUpdating)
            {
                GameHelpers.GetMovementComponent().jumpGravity = ConfigValues.jump.value;
            }
            if (ConfigValues.noFallDamage.constantUpdating && ConstantFields.GetFallDamageTime() != null)
            {
                CharacterMovement move = GameHelpers.GetMovementComponent();
                ConstantFields.GetFallDamageTime().SetValueDirect(__makeref(move), ConfigValues.noFallDamage.value ? 999.0f : 1.5f);
            }
        }
        if (GameHelpers.GetCharacterComponent())
        {
            if (ConfigValues.infiniteStamina.constantUpdating && ConstantFields.GetInfiniteStaminaField() != null)
            {
                ConstantFields.GetInfiniteStaminaField().SetValue(GameHelpers.GetCharacterComponent(), ConfigValues.infiniteStamina.value);
            }
            if (ConfigValues.statusLock.constantUpdating && ConstantFields.GetStatusLockField() != null)
            {
                ConstantFields.GetStatusLockField().SetValue(GameHelpers.GetCharacterComponent(), ConfigValues.statusLock.value);
            }
            if (ConfigValues.fly.value && GameHelpers.GetRagdollComponent() && GameHelpers.GetMovementComponent())
            {
                // Plugin.Logger.LogError("HERE1");
                var move = GameHelpers.GetMovementComponent();
                move.maxGravity = 0.0f;
                var ragdoll = GameHelpers.GetRagdollComponent();
                // ragdoll.ToggleCollision(false);
                // ragdoll.ToggleKinematic(true);
                if (ragdoll.partDict.ContainsKey(BodypartType.Hip))
                {
                    // Plugin.Logger.LogError("HERE2");
                    var hip = ragdoll.partDict[BodypartType.Hip].gameObject.transform;
                    var camForward = Camera.main.gameObject.transform.forward;
                    var camRight = Camera.main.gameObject.transform.right;
                    // camForward.y = 0.0f;
                    // camForward.Normalize();
                    // camRight.y = 0.0f;
                    // camRight.Normalize();
                    var time = Time.deltaTime;
                    var speed = ConfigValues.flySpeed.value;
                    if (Input.GetKey(KeyCode.W))
                    {
                        // Plugin.Logger.LogError("FOWARD");
                        hip.position += camForward * time * speed;
                    }
                    if (Input.GetKey(KeyCode.S))
                    {
                        hip.position -= camForward * time * speed;
                    }
                    if (Input.GetKey(KeyCode.A))
                    {
                        hip.position -= camRight * time * speed;
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        hip.position += camRight * time * speed;
                    }
                }
            }
        }
    }

    void OnGUI()
    {
    }

    void LateUpdate()
    {
    }

    void Start()
    {
        Action act = () =>
        {
        };
        // this.StartCoroutine(ButtonAPI.WaitForQMClone(act).WrapToIl2Cpp());
    }

}