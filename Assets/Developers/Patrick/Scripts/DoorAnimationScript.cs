using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimationScript : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField] 
    private GameObject _collisionBox;
    [SerializeField]
    private float _timeToCloseDoor;
    [SerializeField]
    private Transform _closedPosition; 
    [SerializeField]
    private Transform _openPosition;
    [SerializeField]
    private PathfindingManager _pathfinder;
    private float _currentAlpha;
    private float _closeTime;

    private Transform _imageTransform;

    private void Start()
    {
        _imageTransform = _renderer.gameObject.transform;
    }

    private void OnEnable()
    {
        _collisionBox.gameObject.SetActive(true);

        _pathfinder.CalculateGrid();

        _currentAlpha = 0.0f;
        _renderer.color = new Color(1.0f, 1.0f, 1.0f, _currentAlpha);
        _closeTime = Time.time;

        if (!_imageTransform)
        {
            _imageTransform = _renderer.gameObject.transform;
        }

        _imageTransform.position = _openPosition.position;

        StartCoroutine(CloseDoor());
    }

    public void CloseDoorCommand()
    {
        _collisionBox.gameObject.SetActive(false);

        _pathfinder.CalculateGrid();

        _currentAlpha = 1.0f;
        _renderer.color = new Color(1.0f, 1.0f, 1.0f, _currentAlpha);
        _closeTime = Time.time;

        if (!_imageTransform)
        {
            _imageTransform = _renderer.gameObject.transform;
        }

        _imageTransform.position = _closedPosition.position;

        StartCoroutine(OpenDoor());
    }

    IEnumerator CloseDoor()
    {
        float progress = 0.0f;
        while(progress < 1.0f)
        {
            progress = Mathf.Min((Time.time - _closeTime) / _timeToCloseDoor, 1.0f);

            _currentAlpha = progress;

            _renderer.color = new Color(1.0f, 1.0f, 1.0f, _currentAlpha);

            _imageTransform.position = Vector3.Lerp(_openPosition.position, _closedPosition.position, progress);

            yield return null;
        }
    }

    IEnumerator OpenDoor()
    {
        float progress = 0.0f;
        while (progress < 1.0f)
        {
            progress = Mathf.Min((Time.time - _closeTime) / _timeToCloseDoor, 1.0f);

            _currentAlpha = 1.0f - progress;

            _renderer.color = new Color(1.0f, 1.0f, 1.0f, _currentAlpha);

            _imageTransform.position = Vector3.Lerp(_closedPosition.position, _openPosition.position, progress);

            yield return null;
        }

        transform.parent.gameObject.SetActive(false);
    }
}
