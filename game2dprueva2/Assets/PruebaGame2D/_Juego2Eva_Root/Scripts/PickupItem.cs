using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private float popupDelay = 0.3f;

    private bool pickedUp;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;

        Player player = other.GetComponent<Player>();
        if (player == null) return;

        pickedUp = true;

        // Dar habilidad
        player.UnlockAbility(itemData.ability);

        // 👉 Lanzamos la coroutine PRIMERO
        StartCoroutine(PickupSequence());
    }

    private IEnumerator PickupSequence()
    {
        // Ocultar visualmente el item
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSecondsRealtime(popupDelay);

        if (ItemPopupUI.Instance != null)
            ItemPopupUI.Instance.ShowItem(itemData);
        else
            Debug.LogError("❌ NO HAY ItemPopupUI EN ESCENA");

        // 👉 Ahora sí, lo desactivamos
        gameObject.SetActive(false);
    }
}
