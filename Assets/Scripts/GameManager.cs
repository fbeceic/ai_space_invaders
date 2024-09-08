using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject explosion;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;

    private Player player;
    private Invaders invaders;
    private MysteryShip mysteryShip;

    private int score;
    private int lives;

    public AudioClip enemyShootSound;
    public AudioClip enemyDeathSound;
    public AudioClip playerShootSound;
    public AudioClip playerDeathSound;
        
    public AudioSource audioSource;
    
    public int Score => score;
    public int Lives => lives;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        invaders = FindObjectOfType<Invaders>();
        mysteryShip = FindObjectOfType<MysteryShip>();
        audioSource = GetComponent<AudioSource>();
        
        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return)) {
            NewGame();
        }
    }

    private void NewGame()
    {
        gameOverUI.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        invaders.ResetInvaders();
        invaders.gameObject.SetActive(true);

        Respawn();
    }

    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = 0f;
        player.transform.position = position;
        player.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
        invaders.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(4, '0');
    }

    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        livesText.text = this.lives.ToString();
    }

    public void OnPlayerKilled(Player player)
    {
        audioSource.PlayOneShot(playerDeathSound);
        
        SetLives(lives - 1);

        var explosionObject = Instantiate(explosion, player.gameObject.transform.position, Quaternion.identity);
        StartCoroutine(DestroyAfterAnim(explosionObject));
        
        player.gameObject.SetActive(false);

        if (lives > 0) {
            Invoke(nameof(NewRound), 1f);
        } else {
            GameOver();
        }
    }
    
    private IEnumerator DestroyAfterAnim(GameObject explosionObject)
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(explosionObject);
    }

    public void OnInvaderKilled(Invader invader)
    {
        audioSource.PlayOneShot(enemyDeathSound);
        
        var explosionObject = Instantiate(explosion, invader.gameObject.transform.position, Quaternion.identity);
        StartCoroutine(DestroyAfterAnim(explosionObject));

        invader.gameObject.SetActive(false);

        SetScore(score + invader.score);

        if (invaders.GetAliveCount() == 0) {
            NewRound();
        }
    }

    public void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        SetScore(score + mysteryShip.score);
    }

    public void OnBoundaryReached()
    {
        if (invaders.gameObject.activeSelf)
        {
            invaders.gameObject.SetActive(false);

            OnPlayerKilled(player);
        }
    }

}
