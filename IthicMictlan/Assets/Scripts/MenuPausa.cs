using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    public GameObject canvasMenuPausa;

    void Start()
    {
        // Asegúrate de que el canvas esté desactivado al inicio
        if (canvasMenuPausa != null)
        {
            canvasMenuPausa.SetActive(false);
        }
    }

    void Update()
    {
        // Verifica si se presionó la tecla 'Escape'
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si el canvas está activado, desactívalo; si está desactivado, actívalo
            if (canvasMenuPausa != null)
            {
                canvasMenuPausa.SetActive(!canvasMenuPausa.activeSelf);
            }

            // Puedes poner aquí lógica adicional, como pausar el juego si el menú está activo
            // Time.timeScale = (canvasMenuPausa.activeSelf) ? 0f : 1f;
        }
    }
}

