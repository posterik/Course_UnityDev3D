﻿
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketScript : MonoBehaviour
{
    // Component references
    private Rigidbody rigidBody;

    // Modifiable variables
    [SerializeField] private float rotationSpeed = 0;
    [SerializeField] private float thrustPower = 0;

    private AudioSource audioSource;
    [SerializeField] private AudioClip EngineSound;
    [SerializeField] private AudioClip DeathSound;
    [SerializeField] private AudioClip VictorySound;

    enum State {Alive, Dying, Transcending};
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (state != State.Dying) {
            Thrust();
            Rotation();
        }
    }

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space)) {
            rigidBody.AddRelativeForce(Vector3.up * thrustPower);
            if (!audioSource.isPlaying) {
                audioSource.PlayOneShot(EngineSound);
            }
        } else {
            if (state != State.Transcending) {
                audioSource.Stop();
            }
        }
    }

    private void Rotation() {
        rigidBody.freezeRotation = true;
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            Debug.Log("Left and/or Right pressed");
        } else if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
        } else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * Time.deltaTime * rotationSpeed);
        }
        rigidBody.freezeRotation = false;
    }

    void OnCollisionEnter(Collision collision) {

        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag) {
            case "Friendly":
                Debug.Log("Friendly");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            case "Fuel":
                Debug.Log("Power Up");
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(VictorySound);
        Invoke("LoadNextScene", 1f); // parameter time
    }
    private void StartDeathSequence() {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(DeathSound);
        Invoke("LoadNextScene", 1f);  // parameter time
    }

    private void LoadNextScene() { 
        switch (state) {
            case State.Alive:
                // Should not come here?
                Debug.Log("Loading scene while alive, should not happen.");
                break;
            case State.Dying:
                state = State.Alive;
                SceneManager.LoadScene(0);
                break;
            case State.Transcending:
                state = State.Alive;
                SceneManager.LoadScene(1);
                break;
            default:
                //Should never be the case
                Debug.LogError("Unkown state in LoadNextScene");
                break;
        }
    }
}
