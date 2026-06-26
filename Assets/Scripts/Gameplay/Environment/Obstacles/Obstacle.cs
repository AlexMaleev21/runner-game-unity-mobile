using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    [SerializeField] protected ObstacleType _type;

    public ObstacleType Type => _type;

    public void SetType(ObstacleType type)
    {
        _type = type;
    }

    public virtual void OnSpawn()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnDespawn()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
}
