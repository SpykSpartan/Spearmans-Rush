using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    [Header("Footstep Audio")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;

    [Header("Step Timing")]
    public float walkStepRate = 0.55f;
    public float sprintStepRate = 0.35f;

    private float nextStepTime;
    private PlayerMovement movement;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!movement.controller.isGrounded) return;

        float speed = movement.GetCurrentSpeed();

        if (speed > 0.1f)
        {
            float stepRate = movement.IsSprinting() ? sprintStepRate : walkStepRate;

            if (Time.time >= nextStepTime)
            {
                PlayFootstep();
                nextStepTime = Time.time + stepRate;
            }
        }
    }

    private void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];

        footstepSource.PlayOneShot(clip);
    }
}
