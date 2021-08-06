using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlingController : MonoBehaviour
{
    [SerializeField] private Transform slingSeat;
    [SerializeField] private Transform slingLookTransform;
    [SerializeField] private Transform slingCharacterTransform;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private LineRenderer predictionLineRenderer;

    [SerializeField] private float maximumPullDistance = 2.5f;
    [SerializeField] private float slingForceMultiplier = 100;
    [SerializeField] private Rigidbody slingSeatRigidbody;
    [SerializeField] private Rigidbody characterRigidbody;

    private Vector3 _slingSeatStartPosition;
    private Vector3 _dragStartPosition;
    private Vector3 _dragLastPosition;
    private Scene _activeScene;
    private Scene _predictionScene;
    private PhysicsScene _activePhysicsScene;
    private PhysicsScene _predictionPhysicsScene;
    private GameObject _characterClone;
    private Rigidbody _characterCloneRigidbody;
    private bool _hasFlew;
    private CapsuleCollider _playerCollider;

    private void Awake()
    {
        _activeScene = SceneManager.GetActiveScene();
        _activePhysicsScene = _activeScene.GetPhysicsScene();

        var createSceneParameters = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
        _predictionScene = SceneManager.CreateScene("Prediction Scene", createSceneParameters);
        _predictionPhysicsScene = _predictionScene.GetPhysicsScene();
        Physics.autoSimulation = false;

        _playerCollider = characterTransform.gameObject.GetComponent<CapsuleCollider>();
        _playerCollider.enabled = false;

        _slingSeatStartPosition = slingSeat.transform.position;
        _characterClone = Instantiate(characterTransform.gameObject);
        _characterClone.GetComponent<PlayerHandler>().enabled = false;
        _characterCloneRigidbody = _characterClone.GetComponent<Rigidbody>();

        var renderers = _characterClone.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var renderer in renderers)
            renderer.enabled = false;

        SceneManager.MoveGameObjectToScene(_characterClone, _activeScene);
    }

    private void Update()
    {
        if (_hasFlew) return;

        var mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        var mousePosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        if (Input.GetMouseButtonDown(0))
        {
            _dragStartPosition = mouseScreenPosition;
            slingSeatRigidbody.velocity = Vector3.zero;
            characterRigidbody.velocity = Vector3.zero;
            _characterCloneRigidbody.velocity = Vector3.zero;
            slingSeatRigidbody.useGravity = false;
            characterRigidbody.useGravity = false;
            _characterCloneRigidbody.useGravity = false;
            _playerCollider.enabled = false;

            characterAnimator.Play("Seated");
        }

        if(Input.GetMouseButton(0))
        {
            _dragLastPosition = mouseScreenPosition;
            characterAnimator.Play("Seated Aiming");

            var dragDifference = mainCamera.ScreenToWorldPoint(_dragStartPosition) - mainCamera.ScreenToWorldPoint(_dragLastPosition);
            if (dragDifference.magnitude > maximumPullDistance)
                dragDifference = dragDifference.normalized * maximumPullDistance;

            slingSeat.position = _slingSeatStartPosition + dragDifference;
            slingSeat.LookAt(slingLookTransform);

            characterTransform.position = slingCharacterTransform.position;
            characterTransform.rotation = slingSeat.rotation;

            _characterClone.transform.position = slingCharacterTransform.position;
            _characterClone.transform.rotation = slingCharacterTransform.rotation;

            _characterCloneRigidbody.velocity = Vector3.zero;
            _characterCloneRigidbody.useGravity = true;

            var force = mainCamera.ScreenToWorldPoint(_dragLastPosition) - mainCamera.ScreenToWorldPoint(_dragStartPosition);
            if (force.magnitude > maximumPullDistance)
                force = force.normalized * maximumPullDistance;

            _characterCloneRigidbody.AddForce(force * slingForceMultiplier);

            predictionLineRenderer.positionCount = 100;
            for(var i = 0; i < predictionLineRenderer.positionCount; i++)
            {
                _predictionPhysicsScene.Simulate(Time.fixedDeltaTime);
                predictionLineRenderer.SetPosition(i, _characterClone.transform.position);
            }
            _characterCloneRigidbody.useGravity = false;
        }

        if(Input.GetMouseButtonUp(0))
        {
            _hasFlew = true;
            _playerCollider.enabled = true;

            var dragDifference = mainCamera.ScreenToWorldPoint(_dragLastPosition) - mainCamera.ScreenToWorldPoint(_dragStartPosition);
            if (dragDifference.magnitude > maximumPullDistance)
                dragDifference = dragDifference.normalized * maximumPullDistance;

            slingSeatRigidbody.AddForce(dragDifference * slingForceMultiplier);
            characterRigidbody.AddForce(dragDifference * slingForceMultiplier);
            slingSeatRigidbody.useGravity = true;
            characterRigidbody.useGravity = true;

            characterAnimator.Play("Falling");
        }
    }

    private void FixedUpdate()
    {
        slingSeatRigidbody.velocity *= 0.95f;
        if (_activeScene.IsValid())
            _activePhysicsScene.Simulate(Time.fixedDeltaTime);
    }
}
