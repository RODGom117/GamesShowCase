using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CamaUltu : MonoBehaviour
{
    GameObject camazotz;
    private float tiempoVisible = 5f;
    private float tiempoVisibleHorizontal = 3f;
    private float velocidadVertical = 5f;
    private float velocidadHorizontal = 15f;

    private void Start()
    {
        camazotz = transform.GetChild(0).gameObject;
        // Llamamos a la función para comenzar el movimiento y la invisibilidad
        StartCoroutine(MoverYDesaparecer());
    }

    private IEnumerator MoverYDesaparecer()
    {
        // Movimiento hacia adelante
        float tiempoInicio = Time.time;
        GetComponent<NavMeshAgent>().enabled = false;
        while (Time.time - tiempoInicio < tiempoVisible)
        {
            transform.Translate(Vector3.up * velocidadVertical * Time.deltaTime);
            yield return null;
        }

        // Hacemos invisible el objeto
        camazotz.SetActive(false);

        // Esperamos un segundo antes de volver a hacer visible el objeto
        yield return new WaitForSeconds(0.5f);

        // Movimiento lateral 1
        transform.position = new Vector3(-5f, 0f, 0f); // Posición inicial en el lado izquierdo
        camazotz.SetActive(true); // Hacemos visible el objeto

        GetComponent<NavMeshAgent>().enabled = true;

        // Movimiento de lado a lado
        tiempoInicio = Time.time;
        while (Time.time - tiempoInicio < tiempoVisibleHorizontal)
        {
            transform.Translate(Vector3.right * velocidadHorizontal * Time.deltaTime);
            yield return null;
        }

        //Hacer invisible el objeto
        camazotz.SetActive(false);

        //Esperar un segundo antes de volver a hacer visible el objeto
        yield return new WaitForSeconds(0.5f);

        //Movimiento lateral 2
        transform.position = new Vector3(5f, 0f, 0f); // Posición inicial en el lado derecho
        tiempoInicio = Time.time;
        camazotz.SetActive(true); // Hacemos visible el objeto
        while (Time.time - tiempoInicio < tiempoVisibleHorizontal)
        {
            transform.Translate(Vector3.left * velocidadHorizontal * Time.deltaTime);
            yield return null;
        }

        // Hacemos invisible el objeto
        camazotz.SetActive(false);

        // Esperamos un segundo antes de volver a hacer visible el objeto
        yield return new WaitForSeconds(0.5f);

        // Movimiento hacia atrás
        transform.position = new Vector3(0f, 0f, 5f); // Posición inicial en el lado derecho
        tiempoInicio = Time.time;
        camazotz.SetActive(true); // Hacemos visible el objeto
        while (Time.time - tiempoInicio < tiempoVisible)
        {
            transform.Translate(Vector3.forward * velocidadVertical * Time.deltaTime);
            yield return null;
        }

    }
}
