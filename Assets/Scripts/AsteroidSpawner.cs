using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;

    public float intervalo = 2f;

    public float limiteX = 9f;
    public float limiteY = 5f;

    void Start()
    {
        InvokeRepeating(
            nameof(CrearAsteroide),
            1f,
            intervalo
        );
    }

    void CrearAsteroide()
    {
        Vector2 posicion;

        int lado = Random.Range(0, 4);

        switch (lado)
        {
            case 0:
                posicion = new Vector2(
                    -limiteX,
                    Random.Range(
                        -limiteY,
                        limiteY
                    )
                );
                break;

            case 1:
                posicion = new Vector2(
                    limiteX,
                    Random.Range(
                        -limiteY,
                        limiteY
                    )
                );
                break;

            case 2:
                posicion = new Vector2(
                    Random.Range(
                        -limiteX,
                        limiteX
                    ),
                    limiteY
                );
                break;

            default:
                posicion = new Vector2(
                    Random.Range(
                        -limiteX,
                        limiteX
                    ),
                    -limiteY
                );
                break;
        }

        Instantiate(
            asteroidPrefab,
            posicion,
            Quaternion.identity
        );
    }
} 