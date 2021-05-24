using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

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
    
    public Text textProbabilidad;
    public Text textProbabilidad2;
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




    //STATS INICIALES
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

    //añadimos 10 a la apuesta
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
            //si no es mayor que 10 vladra su posicion
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
            //AÑADIMOS LA CARTA SEGUN SU POSICION
            deckInGame.Add(faces[i]);
        }

        Sprite spriteTmp;
        int n = deckInGame.Count;



        //mientras la cuenta sea mayor que 1
        while (n > 1)
        {
            //restamos uno a n
            n--;
            //Ordena de manera aleatoria los valores de la lista
            int k = Random.Range(0, n + 1);
            //Recorre todas las posiciones de la lista y intercambia cada casilla por otra aleatoria
            spriteTmp = deckInGame[k];
            deckInGame[k] = deckInGame[n];
            deckInGame[n] = spriteTmp;
        }
    }

    //cogemos el numero de la carta
    private int GetNumberFromSprite(Sprite sprite)
    {
        //iniciamos semaforo en true
        bool light = true;
        //el numero a -1
        int number = -1;
        for (int i = 0; i < faces.Length && light; i++)
        {
            if (faces[i] == sprite)
            {
                //el numero sera la posicion
                number = values[i];
                //ponemos el semaforo en false
                light = false;
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
        CalculateProbabilities();
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
        CalculateProbabilities();
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

    //plantarse
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
            //no puedes volver a jugar
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

    //METODO PROB DE QUE SALGAN NUMEROS 17 A 21
    private float Prob17a21(int valor)
    {
        //CALCULAMOS EL NUMERO DE CARTAS QUE HAY EN EL MAZO
        int numeroCartasMazo = (deckInGame.Count - cardIndex) + 1;
        //INICIAMOS CONTADOR CARTAS A 0
        int contadorCartas = 0;
        //HACEMOS UNA LISTA COPIANDO NUESTRO DECK
        List<Sprite> copyDeck = new List<Sprite>();
        for (int i = cardIndex; i < deckInGame.Count; i++)
        {
            //PONEMOS EL DECK SEGUN LA PARTIDA
            copyDeck.Add(deckInGame[i]);
        }

        //AÑADIMOS LA CARTA DEL DEALER QUE NO SABEMOS CUAL ES
        copyDeck.Add(dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().front);

        for (int i = 0; i < copyDeck.Count; i++)
        {
            //SI ES IGUAL AL VALOR QUE QUEREMOS LA CARTA DEL DECK
            if (GetNumberFromSprite(copyDeck[i]) == valor)
            {
                //AÑADIMOS UNA UNIDAD AL CONTADOR
                contadorCartas++;
            }
        }
        //EL RESULTADO SERA LA DIVISION ENTRE EL CONTADOR DE CARTAS Y EL NUMERO DE CARTAS QUE QUEDAN EN EL MAZO
        float res = (float)contadorCartas / (float)numeroCartasMazo;
        //DEVOLVEMOS EL RESULTADO
        return res;
    }


    private float Mas21()
    {

        int cartasMas21 = 13 - (21 - player.GetComponent<CardHand>().points);
        float probMas21 = cartasMas21 / 13f;

        

        //CASO MAS DE 100%
        if (probMas21 > 1)
        {
            probMas21 = 1;
        }else if(probMas21 < 0)
        {
            probMas21 = 0;
        }

        textProbabilidad2.text = "Prob mas de 21 siguiente carta: " + Math.Round(probMas21 * 100).ToString() + "%";
        return probMas21;
    }

    


    //HACEMOS LISTA DE NUMEROS PARA SACAR ENTRE 17 Y 21 Y LE PASAMOS EL VALOR INICIAL
    private List<int> SiguienteCartaEntre17y21(int valorInicial)
    {
        List<int> valores = new List<int>();
        //RECORREMOS HASTA LA DECIMA POSICION
        for (int i = 1; i <= 10; i++)
        {
            //SI EL VALOR INICIAL MAS LA SIGUIENTE CARTA SUMA ENTRE 17 A 21
            if (valorInicial + i >= 17 && valorInicial + i <= 21)
            {
                //AÑADIMOS LOS VALORES A LA LISTA CREADA
                valores.Add(i);
            }
        }
        //DEVOLVEMOS LOS VALORES
        return valores;
    }



    //METODO PROBABILIDADES
    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
        if (dealer.GetComponent<CardHand>().cards.Count >= 1)
        {

            
            //
           

            int punuacionDealerSinCartaInicial = dealer.GetComponent<CardHand>().points - dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().value;

            

            float probObtenerValorCercano21 = 0.0f;
            //lista de numeros siguientes que pueden darnos un valor entre 17 y 21
            List<int> NumeroEntre17y21 = SiguienteCartaEntre17y21(player.GetComponent<CardHand>().points);
            for (int i = 0; i < NumeroEntre17y21.Count; i++)
            {
                //llamamos al metodo anterior
                probObtenerValorCercano21 += Prob17a21(NumeroEntre17y21[i]);
            }
            //lo mostramos en el text box

            textProbabilidad.text = "Probabilidad sacar entre 17 y 21 con la siguiente carta: " + (probObtenerValorCercano21 * 100).ToString("0.00") + "%";


           
        }
        Mas21();


    }

    /////////////////////
    
    
    


}
   
