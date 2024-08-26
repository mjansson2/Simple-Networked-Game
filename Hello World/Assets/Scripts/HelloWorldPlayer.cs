using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using TMPro;
using UnityEngine.UI;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        private float jumpForce = 100f;
        private Rigidbody rb;
        private Animator animator;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();   
            animator = GetComponent<Animator>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && IsOwner)
            {
                SubmitJumpServerRpc();
            }
        }

        [Rpc(SendTo.Server)]
        private void SubmitJumpServerRpc(RpcParams rpcParams = default)
        {
            SendJumpClientAndHostRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendJumpClientAndHostRpc(RpcParams rpcParams = default)
        {
            Jump();
        }

        private void Jump()
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }


}

