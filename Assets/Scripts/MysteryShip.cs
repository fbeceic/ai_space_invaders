using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class MysteryShip : MonoBehaviour
{
    public float speed = 5f;
    public float cycleTime = 30f;
    public int score = 300;

    private Vector2 leftDestination;
    private Vector2 rightDestination;
    private int direction = -1;
    private bool spawned;

    public AudioClip bossLoopSound;
    public AudioClip bossDeathSound;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        leftDestination = new Vector2(leftEdge.x - 10f, transform.position.y);
        rightDestination = new Vector2(rightEdge.x + 10f, transform.position.y);

        Despawn(false);
    }

    private void Update()
    {
        if (!spawned)
        {
            return;
        }

        if (direction == 1)
        {
            MoveRight();
        }
        else
        {
            MoveLeft();
        }
    }

    private void MoveRight()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x >= rightDestination.x)
        {
            Despawn(false);
            _audioSource.Stop();
        }
    }

    private void MoveLeft()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= leftDestination.x)
        {
            Despawn(false);
            _audioSource.Stop();
        }
    }

    private void Spawn()
    {
        _audioSource.Play();

        direction *= -1;

        if (direction == 1)
        {
            transform.position = leftDestination;
        }
        else
        {
            transform.position = rightDestination;
        }

        spawned = true;
    }

    private void Despawn(bool playSound)
    {
        if (playSound)
        {
            _audioSource.Stop();
            StartCoroutine(StopSourceAfterDelay(0.2f));
        }

        spawned = false;

        if (direction == 1)
        {
            transform.position = rightDestination;
        }
        else
        {
            transform.position = leftDestination;
        }

        Invoke(nameof(Spawn), cycleTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            Despawn(true);
            GameManager.Instance.OnMysteryShipKilled(this);
        }
    }

    private IEnumerator StopSourceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _audioSource.PlayOneShot(bossDeathSound);
    }
}