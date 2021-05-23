using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;

    public Button buttonAdd;
    public Button buttonLess;
    public Button buttondApostar;
    public Button buttonAllin;

    public Text finalMessage;
    public Text probMessage;
    public Text textSaldoActual;
    public Text textSaldoEnJuego;
    public int[] values = new int[52];
    public List<Sprite> deckInGame = new List<Sprite>();
    int cardIndex = 0;
    private int dinero;
    private int saldoEnJuego;

    enum ResultGame : int
    {
        Tie = 0,
        PlayerWin = 1,
        PlayerLose = 2,
        BlackjackPlayerWin = 3,
        BlackjackPlayerLose = 4
    }

    private bool jugadorGana;


    private void Awake()
    {
        InitCardValues();
        dinero = 1000;
        saldoEnJuego = 0;
        actualizarTextoSaldo();
    }

    //actualizamos el saldo del jugador
    private void actualizarTextoSaldo()
    {
        textSaldoActual.text = dinero.ToString();
        textSaldoEnJuego.text = saldoEnJuego.ToString();
    }

    //empezamos la partida
    private void Start()
    {
        ShuffleCards();
        Apuesta(true);
    }
    
    //boton para apostar
    public void buttonApostar()
    {
        //comprobamos el saldo en juego y empezamos la partida
        if (saldoEnJuego > 0)
        {
            Apuesta(false);
            StartGame();
        }

    }

    //momento de apostar
    public void Apuesta(bool state)
    {
        //hacemos los bontones interactuables
        buttonAllin.interactable = state;
        buttonAdd.interactable = state;
        buttonLess.interactable = state;
        buttondApostar.interactable = state;

        //hacemos los botones desinteractuables
        playAgainButton.interactable = !state;
        hitButton.interactable = !state;
        stickButton.interactable = !state;
        
        //SI STATE ES TRYE
        if (state == true)
        {
            //MENSAJE PARA APOSTAR 
            finalMessage.color = Color.white;
            finalMessage.text = "Haga su apuesta ahora";
            //LIMPIAMOS LA MESA
            player.GetComponent<CardHand>().Clear();
            dealer.GetComponent<CardHand>().Clear();
            //IGUALAMOS CARTAS REPARTIDAS A 0
            cardIndex = 0;
        }
        //SI ES FALSO
        else
        {
            finalMessage.text = "";
        }



    }

    //añadimos 15 a la apuesta
    public void ButtonAdd15()
    {
        if (dinero > 0)
        {
            dinero = dinero - 10;
            saldoEnJuego = saldoEnJuego + 10;
            actualizarTextoSaldo();
        }

    }

    //añadimos 30 a la apuesta
    public void ButtonAdd30()
    {
        if (dinero > 0)
        {
            dinero = dinero - 30;
            saldoEnJuego = saldoEnJuego + 30;
            actualizarTextoSaldo();
        }

    }

    //añadir todo el dinero
    public void buttonAllIn()
    {
        if (dinero > 0)
        {

            saldoEnJuego = saldoEnJuego + dinero;
            dinero = 0;
            actualizarTextoSaldo();
        }

    }

    //Quitamlos 10 a la apuesta
    public void ButtonMenos15()
    {
        if (saldoEnJuego > 0)
        {
            dinero = dinero + 10;
            saldoEnJuego = saldoEnJuego - 10;
            actualizarTextoSaldo();
        }

    }

    //Quitamos 30 a la apuesta
    public void ButtonMenos30()
    {
        if (saldoEnJuego > 0)
        {
            dinero = dinero + 50;
            saldoEnJuego = saldoEnJuego - 50;
            actualizarTextoSaldo();
        }

    }
    

    //iniciamos los valores de las cartas
    private void InitCardValues()
    {
        //Lo hacemos de 13 en 13 en cada palo a rellenar(52 cartas en total)
        for (int i = 0; i < 52; i = i + 13)
        {
            rellenarPalo(i);
        }

    }



    //rellenamos el palo de cartas
    private void rellenarPalo(int posInicio)
    {
        //iniciamos la posicion
        int initPos = posInicio;
        //recorremos el palo
        for (int i = 1; i <= 13; i++)
        {
            //si es mayor que 10
            if (i > 10)
            {
                values[initPos] = 10;
            }
            //si no es mayor que 10
            else
            {
                values[initPos] = i;
            }
            //sumamos una a la posicion inicial
            initPos++;
        }

    }
    //METODO SHUFFLECARDS
    private void ShuffleCards()
    {
        //LIMPIAMOS MESA
        deckInGame.Clear();
        //RECORREMOS EN BUCLE LA LONGITUD DE LAS CARTAS
        for (int i = 0; i < faces.Length; i++)
        {
            //AÑADIMOS LA IMAGEN SEGUN SU POSICION
            deckInGame.Add(faces[i]);
        }

        Sprite spriteTmp;
        int n = deckInGame.Count;
        //Ordena aleatoriamente los valores de la lista
        //Recorre todas las posiciones de la lista y intercambia cada casilla por otra aleatoria
        //mientras la cuenta sea mayor que 1
        while (n > 1)
        {
            //restamos uno a n
            n--;
            int k = Random.Range(0, n + 1);
            spriteTmp = deckInGame[k];
            deckInGame[k] = deckInGame[n];
            deckInGame[n] = spriteTmp;
        }
    }

    //cogemos el numero de la carta
    private int GetNumberFromSprite(Sprite sprite)
    {
        //iniciamos semaforo en true
        bool semaforo = true;
        //el numero a -1
        int number = -1;
        for (int i = 0; i < faces.Length && semaforo; i++)
        {
            if (faces[i] == sprite)
            {
                //el numero sera la posicion
                number = values[i];
                //ponemos el semaforo en false
                semaforo = false;
            }
        }
        //devolvemos el numero
        return number;
    }

    //metodo comprobar 21
    void ComprobarBlackJack()
    {
        //si el jugador tiene 21 gana
        if (player.GetComponent<CardHand>().points == 21)
        {
            EndGame(ResultGame.BlackjackPlayerWin);
        }
        //si el dealer tiene 21 gana
        else if (dealer.GetComponent<CardHand>().points == 21)
        {
            EndGame(ResultGame.BlackjackPlayerLose);

        }
    }


    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */

        //el dealer recibe una nueva carta
        dealer.GetComponent<CardHand>().Push(deckInGame[cardIndex], GetNumberFromSprite(deckInGame[cardIndex]));
        //sumamos una carta repartida
        cardIndex++;
        //calculamos los puntos
        Debug.Log("puntos dealer: " + dealer.GetComponent<CardHand>().points);
        //llamamos al metodo de probabilidades
        CalcularProbabilidades();
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */

        //el jugador recibe una nueva carta
        player.GetComponent<CardHand>().Push(deckInGame[cardIndex], GetNumberFromSprite(deckInGame[cardIndex]));
        //sumamos una carta repartida
        cardIndex++;
        //llamamos al metodo de probabilidades
        CalcularProbabilidades();
        //calculamos los puntos
        Debug.Log("puntos del jugador: " + player.GetComponent<CardHand>().points);

    }

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */

        if (player.GetComponent<CardHand>().points == 21)
        {
            EndGame(ResultGame.PlayerWin);
        }
        else if (player.GetComponent<CardHand>().points > 21)
        {
            EndGame(ResultGame.PlayerLose);
        }

    }
    //enmpezamos el juego
    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
        }

        ComprobarBlackJack();
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */



        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */

        //comprobamos si el dealer tiene menos de 16 y repartimos una carta
        if (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
        }
        //comprobamos victoria
        ComprobarVictoriaFinal();

    }

    //comprobamos quien gana
    private void ComprobarVictoriaFinal()
    {
        //si tienen los mismo puntos empate
        if (dealer.GetComponent<CardHand>().points == player.GetComponent<CardHand>().points)
        {
            EndGame(ResultGame.Tie);
        }
        //si el dealer se pasa
        else if (dealer.GetComponent<CardHand>().points > 21)
        {
            EndGame(ResultGame.PlayerWin);
        }
        //si tiene el dealer mas puntos que el jugador sin pasarse
        else if (dealer.GetComponent<CardHand>().points > player.GetComponent<CardHand>().points)
        {
            EndGame(ResultGame.PlayerLose);
        }
        ////si tiene el jugador mas puntos que el dealer sin pasarse
        else
        {
            EndGame(ResultGame.PlayerWin);
        }
    }

    //Deshabilitamos botones
    private void InteractButtons(bool state)
    {
        hitButton.interactable = state;
        stickButton.interactable = state;
        if (state == false)
        {
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        }

    }


    //fin del juego
    private void EndGame(ResultGame result)
    {
        //mostramos el resultado por mensaje
        bool lose = true;
        if (result == ResultGame.Tie)
        {
            finalMessage.text = "Tablas";
            finalMessage.color = Color.red;
        }
        else if (result == ResultGame.PlayerWin)
        {
            finalMessage.text = "El jugador gana la mano";
            lose = false;
        }
        else if (result == ResultGame.PlayerLose)
        {
            finalMessage.text = "El dealer gana la mano";
        }
        else if (result == ResultGame.BlackjackPlayerLose)
        {
            finalMessage.text = "Blackjack!! El dealer gana la mano";
        }
        else if (result == ResultGame.BlackjackPlayerWin)
        {
            finalMessage.text = "Blackjack!! El jugador gana la mano";
            lose = false;

        }

        if (lose == false)
        {
            //si gana recibe el doble de dinero apostado
            dinero = dinero + (saldoEnJuego * 2);
            finalMessage.color = Color.yellow;
        }
        else
        {
            finalMessage.color = Color.red;
        }
        saldoEnJuego = 0;
        actualizarTextoSaldo();
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        InteractButtons(false);

        //si el jugador se queda sin dinero
        if (dinero == 0)
        {
            finalMessage.text = "Pierdes!! No tienes dinero para jugar";
            playAgainButton.interactable = false;
        }

    }


    //volver a jugar
    public void PlayAgain()
    {

        Apuesta(true);
        ShuffleCards();
        //StartGame();
    }


    
    //calculamos probabilidades
    private void CalcularProbabilidades()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */

        // si el dealer tiene una o mas cartas
        if (dealer.GetComponent<CardHand>().cards.Count >= 1)
        {
            //texto de probabilidades
            string textProb = "";
            float probDealerMasPuntuacion = 0.0f;//float
            //calculamos la prob del dealer sin la carta inicial
            int puntuacionDealerSinPrimCarta = dealer.GetComponent<CardHand>().points - dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().value;

            //numeros para superar la puntuacion del dealer
            List<int> numerosSuperarDealer = NumerosParaSuperarValorConcreto(puntuacionDealerSinPrimCarta, player.GetComponent<CardHand>().points);
            for (int i = 0; i < numerosSuperarDealer.Count; i++)
            {
                //Prob de que la carta en juego sea uno de los numeros que supera el valor del jugador
                probDealerMasPuntuacion += ProbSacarValor(numerosSuperarDealer[i]);
            }
            textProb += "Prob dealer mayor puntuación: " + (probDealerMasPuntuacion * 100).ToString("0.00") + "%\n";
            //prob valor cercano a 21
            float probObtenerValorCercano21 = 0.0f;
            //numeros entre 17 a 21 seran igual al numero de points que lleve en la mano
            List<int> NumeroEntre17y21 = NumerosEntre17y21(player.GetComponent<CardHand>().points);
            //recorremos las posibilidades
            for (int i = 0; i < NumeroEntre17y21.Count; i++)
            {
                //usamos el metodo probsacarvalor para un numero entre 17 y 21
                probObtenerValorCercano21 += ProbSacarValor(NumeroEntre17y21[i]);
            }
            textProb += "Prob carta entre 17 y 21: " + (probObtenerValorCercano21 * 100).ToString("0.00") + "%\n";

            //prob valor mayor a 21 en la mano actual
            float probObtenerValorMayor21 = 0.0f;
            //numero carta siguiente mayor a 21
            List<int> numerosMayor21 = NumerosParaSuperarValorConcreto(player.GetComponent<CardHand>().points, 21);
            for (int i = 0; i < numerosMayor21.Count; i++)
            {
                probObtenerValorMayor21 += ProbSacarValor(numerosMayor21[i]);
            }
            textProb += "Prob de pasarse con la siguiente carta: " + (probObtenerValorMayor21 * 100).ToString("0.00") + "%";

            //mostramos por pantalla las probabilidades
            probMessage.text = textProb;
        }



    }

    private float ProbSacarValor(int valor)
    {

        //iniciamos una variable que nos dice cuantas cartas quedan en el mazo segun las cartas repartidas
        int numeroCartasEnElMazo = (deckInGame.Count - cardIndex) + 1;
        //inicializamos un contador a 0
        int contadorCarta = 0;
        List<Sprite> copyDeck = new List<Sprite>();
        for (int i = cardIndex; i < deckInGame.Count; i++)
        {
            copyDeck.Add(deckInGame[i]);
        }

        //Añadimos a los calculos la carta del dealer porque no sabemos que carta es
        copyDeck.Add(dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().front);

        for (int i = 0; i < copyDeck.Count; i++)
        {
            if (GetNumberFromSprite(copyDeck[i]) == valor)
            {
                //añadamos uno al contador
                contadorCarta++;
            }
        }
        //el resultado sera la division del contador de cartas con el numero de cartas en el mazo
        float res = (float)contadorCarta / (float)numeroCartasEnElMazo;
        //devolvemos el resultado
        return res;
    }

    private List<int> NumerosParaSuperarValorConcreto(int valorInicial, int valorConcreto)
    {
        //iniciamos una lista de enteros con los valores
        List<int> valores = new List<int>();
        //recorremos la lista 
        for (int i = 1; i <= 10; i++)
        {
            //si el valor inicial mas la posicion es mayor que el valor concreto
            if (valorInicial + i > valorConcreto)
            {
                //añadimos el valor
                valores.Add(i);
            }
        }
        //devolvemos los valores
        return valores;
    }

    private List<int> NumerosEntre17y21(int valorInicial)
    {
        //iniciamos una lista de enteros con los valores
        List<int> valores = new List<int>();
        //recorremos la lista
        for (int i = 1; i <= 10; i++)
        {
            //si el valor inicial mas la posicion está ente 17 y 21
            if (valorInicial + i >= 17 && valorInicial + i <= 21)
            {
                //añadimos el valor a la lista
                valores.Add(i);
            }
        }
        //devolvemos los valores
        return valores;
    }

}
