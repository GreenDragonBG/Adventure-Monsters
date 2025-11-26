using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Checkpoint : MonoBehaviour
{
    private static Transform _playerSpawnPoint;
    private static GameObject _player;
    private static Animator _animator;
    private static Rigidbody2D _rb2d;
    private static bool _waitingForAnimation = false;
    private static float _animationDelay = 0.2f;
    private static float _timeStartedWaiting = 0f;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerSpawnPoint = _player.gameObject.transform;
        _animator = _player.GetComponent<Animator>();
        _rb2d = _player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Respawn"))
        {
            _player.transform.position = _playerSpawnPoint.position;
            _animator.SetBool("Teleport", false);
            _waitingForAnimation = true;
            _timeStartedWaiting = Time.time;
        }

        if (_waitingForAnimation && Time.time- _timeStartedWaiting>_animationDelay)
        {
            _rb2d.constraints=RigidbodyConstraints2D.FreezeRotation;
            _waitingForAnimation = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _playerSpawnPoint!=this.gameObject.transform)
        {
            _playerSpawnPoint= this.gameObject.transform;
        }
    }

    public static void Respawn()
    {
        _rb2d.constraints=RigidbodyConstraints2D.FreezeAll;
        _animator.SetBool("Teleport", true);
        _animator.SetTrigger("Damage");
    }
}
