using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Matematicas
{
    //La idea de metodos de extension viene de:
    //https://www.youtube.com/watch?v=E7b3ZNmhbnU&t=12s 


    static public float CalcularRadio(Transform centro, Transform satelite)
    {
        //Realmente no es un metodo de extension, sino solo una funcion estatica
        float distanciaX = satelite.position.x - centro.position.x;
        float distanciaY = satelite.position.y - centro.position.y;
        float _radio = new Vector2(distanciaX, distanciaY).magnitude;
        return _radio;
    }

    static public Vector2 DireccionEntre(Vector2 pos1, Vector2 pos2)
    {
        return (pos2 - pos1).normalized;
    }


    //Codigo sacado de: https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
    public static float Map(float value, float minEscala1, float maxEscala1, float minEscala2, float maxEscala2)
    {
        return ((value - minEscala1) / (maxEscala1 - minEscala1)) * ((maxEscala2 - minEscala2) + minEscala2);
    }


    //Código sacado de ChatGPT y StackOverflow
    public static List<int> GenerarNumerosConsecutivosAleatorios(int min, int max)
    {
        List<int> numbers = new List<int>();
        // Llenar la lista con números del min al max
        for (int i = min; i <= max; i++) numbers.Add(i);
        DesordenarArreglo(numbers);
        return numbers;
    }


    //Sacado de: https://forum.unity.com/threads/randomize-array-in-c.86871/
    public static List<int> DesordenarArreglo(List<int> array)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int i = 0; i < array.Count; i++)
        {
            int temporal = array[i];
            int randomIndex = Random.Range(i, array.Count);
            array[i] = array[randomIndex];
            array[randomIndex] = temporal;
        }
        return array;
    }


    #region Movimiento Circular
    //Codigo sacado de: https://www.youtube.com/watch?v=BGe5HDsyhkY
    static public Vector2 PolaresToRectangulares(float radio, float angulo_rad, Vector3 centro)
    {
        #region Formulas
        /* Cordenadas Polares a rectangulares:
         * x = radio * cos(angulo)
         * y = radio * sin(angulo)
         * Se suma la posicion del gameObject centro para que si se mueve el centro, tambien lo haga la recta
         * 
         * Si se quiere un movimiento eliptico, debe haber un radioX, y un radioY que tomen en cuenta el ancho y largo de la elipisis
         */
        #endregion
        #region Explicacion: angulos
        /* Como el "sin" y "cos" son operaciones ciclicas, no importa el valor
         * que tome la variable de rotacion. Tambien, al multiplicarse con la 
         * velocidad, si no se mueve, el valor es cero y suma a la rotacion. */
        #endregion
        float posX = Mathf.Cos(angulo_rad) * radio + centro.x;
        float posY = Mathf.Sin(angulo_rad) * radio + centro.y;
        return new Vector2(posX, posY);
    }

    static public Vector2 DesplazarCoordenadas(Vector2 puntoInicial, Vector2 distancia, float angulo)
    {
        float desplazamientoX = Mathf.Cos(angulo) * distancia.x;
        float desplazamientoY = Mathf.Sin(angulo) * distancia.y;
        return puntoInicial + new Vector2(desplazamientoX, desplazamientoY);
    }

    static public float MoverAngulo(float angulo, float velocidad, int direccion)
    {
        return angulo + ((velocidad * direccion) * Time.fixedDeltaTime);
    }

    static public float RadianesEntre(Vector2 punto1, Vector2 punto2)
    {
        return Mathf.Atan2(punto2.y - punto1.y, punto2.x - punto1.x);
    }

    static public Vector2 AnguloToDireccion2D(float angulo)
    {
        float x = Mathf.Cos(angulo * Mathf.Deg2Rad); 
        float y = Mathf.Sin(angulo * Mathf.Deg2Rad); 
        return new Vector2(x, y);
    }
    #endregion

}

