using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _moveTime,_maxOffsetY;

    private float currentRotateAngle;
    private float rotateSpeed;

    private bool canMove;
    private bool canShoot;

    [SerializeField]
    private AudioClip _moveClip, _pointClip, _scoreClip, _loseClip;

    [SerializeField]
    private GameObject _explosionPrefab;
    private void Awake()
    {
        currentRotateAngle = 0f;
        canShoot = false;
        canMove = false;
        rotateSpeed = 90f / _moveTime;
    }

    private void OnEnable()
    {
        GameManager.Instance.GameStarted += GameStarted;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameStarted -= GameStarted;
    }

    private void GameStarted()
    {
        canMove = true;
        canShoot = true;
    }

    private void Update()
    {
        if(canShoot && Input.GetMouseButtonDown(0))
        {
            rotateSpeed *= -1f;
            AudioManager.Instance.PlaySound(_moveClip);
        }
    }


    private void FixedUpdate()
    {
        if (!canMove) return;

        currentRotateAngle += rotateSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0,0,currentRotateAngle);

        if(currentRotateAngle < 0f)
        {
            currentRotateAngle = 360f;
        }
        if(currentRotateAngle > 360f)
        {
            currentRotateAngle = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.Tags.SCORE))
        {
            GameManager.Instance.UpdateScore();
            AudioManager.Instance.PlaySound(_scoreClip);
            collision.gameObject.GetComponent<Obstacle>().OnGameEnded();
        }

        if(collision.CompareTag(Constants.Tags.OBSTACLE))
        {
            Destroy(Instantiate(_explosionPrefab,transform.GetChild(0).position,Quaternion.identity), 3f);
            Destroy(Instantiate(_explosionPrefab,transform.GetChild(1).position,Quaternion.identity), 3f);
            AudioManager.Instance.PlaySound(_loseClip);
            GameManager.Instance.EndGame();
            Destroy(gameObject);
        }
    }
}