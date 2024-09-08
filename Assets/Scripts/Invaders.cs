using System.Linq;
using Txt2Img;
using Txt2Img.ThemedTxt2Img;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.UI;

public class Invaders : MonoBehaviour
{
    [Header("Invaders")] public Invader[] prefabs = new Invader[5];
    public AnimationCurve speed = new AnimationCurve();

    private Vector3 direction = Vector3.right;
    private Vector3 initialPosition;

    [Header("Grid")] public int rows = 5;
    public int columns = 11;
    public float xSpacing = 4.0f;
    public float ySpacing = 3.0f;

    [Header("Missiles")] public Projectile missilePrefab;
    public float missileSpawnRate = 1f;
    
    public GameManager GameManager;

    private void Awake()
    {
        initialPosition = transform.position;

        CreateInvaderGrid();
    }

    private void CreateInvaderGrid()
    {
        for (var i = 0; i < rows; i++)
        {
            var width = (xSpacing * (columns - 1));
            var height = (ySpacing * (rows - 1));

            var centerOffset = new Vector2(-width * 0.5f, -height * 0.5f);
            var rowPosition = new Vector3(centerOffset.x, (ySpacing * i) + centerOffset.y, 0f);

            for (var j = 0; j < columns; j++)
            {
                var prefabToInstantiate = prefabs[i];

                var invader = Instantiate(prefabToInstantiate, transform);

                var position = rowPosition + new Vector3(xSpacing * j, 0f, 0f);
                invader.transform.localPosition = position;
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), missileSpawnRate, missileSpawnRate);
    }

    private void MissileAttack()
    {
        int amountAlive = GetAliveCount();

        if (amountAlive == 0)
        {
            return;
        }

        foreach (Transform invader in transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (Random.value < (1f / amountAlive))
            {
                Instantiate(missilePrefab, invader.position, Quaternion.identity);
                GameManager.Instance.audioSource.PlayOneShot(GameManager.Instance.enemyShootSound);
                break;
            }
        }
    }

    private void Update()
    {
        var totalCount = rows * columns;
        var amountAlive = GetAliveCount();
        var amountKilled = totalCount - amountAlive;
        var percentKilled = (float)amountKilled / (float)totalCount;

        var speed = this.speed.Evaluate(percentKilled);
        transform.position += speed * Time.deltaTime * direction;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        foreach (Transform invader in transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (direction == Vector3.right && invader.position.x >= (rightEdge.x - 1f))
            {
                AdvanceRow();
                break;
            }

            if (direction == Vector3.left && invader.position.x <= (leftEdge.x + 1f))
            {
                AdvanceRow();
                break;
            }
        }
    }

    private void AdvanceRow()
    {
        direction = new(-direction.x, 0f, 0f);

        Vector3 position = transform.position;
        position.y -= 1f;
        transform.position = position;
    }

    public void ResetInvaders()
    {
        direction = Vector3.right;
        transform.position = initialPosition;

        foreach (Transform invader in transform)
        {
            invader.gameObject.SetActive(true);
        }
    }

    public int GetAliveCount() => transform.Cast<Transform>().Count(invader => invader.gameObject.activeSelf);
}