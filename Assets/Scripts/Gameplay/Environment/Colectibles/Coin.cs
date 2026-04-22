using UnityEngine;
using Zenject;

public class Coin : MonoBehaviour
{
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Coin trigger: other tag = {other.tag}, position = {transform.position}");
        if (other.CompareTag("Player"))
        {
            _signalBus.Fire(new CoinCollectedSignal());
            gameObject.SetActive(false);
        }
    }
}

public struct CoinCollectedSignal { }
